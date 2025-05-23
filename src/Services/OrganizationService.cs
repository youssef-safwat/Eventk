using Entites;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ServiceContracts.DTOs;
using ServiceContracts.DTOs.Event;
using ServiceContracts.DTOs.Organization;
using ServiceContracts.Helpers;
using ServiceContracts.ServicesContracts;
namespace Services
{
    public class OrganizationService(ApplicationDbContext applicationDbContext , TypeAdapterConfig typeAdapterConfig) : IOrganizationService
    {
        private readonly ApplicationDbContext _context = applicationDbContext;
        private readonly TypeAdapterConfig _config  = typeAdapterConfig;
        public async Task<ServiceResponse<IEnumerable<OrganizationSummaryResponse>>> GetOrganizations(GetOrganizationsRequest organizationsRequest, string? userId)
        {

            var organizations = await _context.Organization.AsNoTracking()
                .OrderByDescending(o => o.FollowersCount)
                .Select(o => o.Adapt<OrganizationSummaryResponse>(_config))
                .ToListAsync();

            if (userId is not null)
            {
                foreach (var organizationSummary in organizations)
                {
                    organizationSummary.IsFollowed = await _context.Follow
                        .AnyAsync(f => f.UserId == userId && f.OrganizationId == organizationSummary.OrganizationId);
                }
            }
            var result = organizations.Where(o => o.IsFollowed == organizationsRequest.IsFollowing).ToList();
            return new ServiceResponse<IEnumerable<OrganizationSummaryResponse>>
            {
                Data = result,
                IsSuccess = true,
                StatusCode = ServiceStatus.Success
            };

        }
        public async Task<ServiceResponse<Response>> FollowOrganization(int organizationId, string userId)
        {
           
            var organization = await _context.Organization.FirstOrDefaultAsync(o => o.OrganizationId == organizationId);
            if(organization is null)
            {
                return new ServiceResponse<Response>
                {
                    IsSuccess = false,
                    StatusCode = ServiceStatus.NotFound,
                    Data = new Response
                    {
                        Message = "Organization not found",
                        Status = "Failed"
                    }
                };
            }
            var follower = await _context.Follow.FirstOrDefaultAsync(f => f.UserId == userId && f.OrganizationId == organizationId);

            if(follower is not null)
            {
                return new ServiceResponse<Response>
                {
                    IsSuccess = false,
                    StatusCode = ServiceStatus.Conflict,
                    Data = new Response
                    {
                        Message = "You already following this organization",
                        Status = "Failed"
                    }
                };
            }
            var follow = new Follow
            {
                OrganizationId = organizationId,
                UserId = userId
            };
            _context.Follow.Add(follow);
            try
            {
                await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE [Organization] SET[FollowersCount] = [FollowersCount] + 1 WHERE[OrganizationId] = {organizationId}");
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return new ServiceResponse<Response>
                {
                    IsSuccess = false,
                    StatusCode = ServiceStatus.InternalServerError,
                    Data = new Response
                    {
                        Message = "Failed to follow organization",
                        Status = "Failed"
                    }
                };
            }

            return new ServiceResponse<Response>
            {
                IsSuccess = true,
                StatusCode = ServiceStatus.Success,
                Data = new Response
                {
                    Message = "Followed Successfully",
                    Status = "Success"
                }
            };
        }
        public async Task<ServiceResponse<OrganizationDetailsResponse>> GetOrganization(int id, string? userId , int pageNumber , int pageSize)
        {
            if(pageNumber <= 0 || pageSize<= 0)
            {
                return new ServiceResponse<OrganizationDetailsResponse>
                {
                    IsSuccess = false,
                    StatusCode = ServiceStatus.BadRequest,
                    Data = null
                };
            }
            var events =  _context.Events
                .Where(e => e.OrganizationID == id)
                .Select(e => e.Adapt<EventSummaryResponse>(_config));



            var pagedEvents = await PaginatedList<EventSummaryResponse>.CreateAsync(events, pageNumber, pageSize);

            var organization = await _context.Organization.Where(o => o.OrganizationId == id)
                .Select(o => new OrganizationDetailsResponse
                {
                    OrganizationId = o.OrganizationId,
                    Description = o.Description,
                    Logo = o.Logo,
                    Name = o.Name,
                    Events = pagedEvents,
                    FollowersCount = o.FollowersCount,
                    Links = o.Links.Select(l => new OrganizationLinksReponse { LinkName = l.LinkName, LinkUrl = l.LinkUrl}).ToList(),
                    IsFollowed = userId == null ? null : o.Followers.Any(f => f.UserId == userId && f.OrganizationId == id)
                }).FirstOrDefaultAsync();

            if (organization is null)
            {
                return new ServiceResponse<OrganizationDetailsResponse>
                {
                    IsSuccess = false,
                    StatusCode = ServiceStatus.NotFound,
                    Data = null
                };
            }
            return new ServiceResponse<OrganizationDetailsResponse>()
            { IsSuccess = true, StatusCode = ServiceStatus.Success, Data = organization };
        }
        public async Task<ServiceResponse<Response>> UnFollowOrganization(int organizationId, string? userId)
        {
            var organization = await _context.Organization.FirstOrDefaultAsync(Organization => Organization.OrganizationId == organizationId);
              
            if (organization is null)
            {
                return new ServiceResponse<Response>
                {
                    IsSuccess = false,
                    StatusCode = ServiceStatus.NotFound,
                    Data = new Response
                    {
                        Message = "Organization not found",
                        Status = "Failed"
                    }
                };
            }
            var follow = await _context.Follow.FirstOrDefaultAsync(f => f.UserId == userId && f.OrganizationId == organizationId);
            if (follow is null)
            {
                return new ServiceResponse<Response>
                {
                    IsSuccess = false,
                    StatusCode = ServiceStatus.Conflict,
                    Data = new Response
                    {
                        Message = "you already unfollowed this organization",
                        Status = "Failed"
                    }
                };
            }

            try
            {
                await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE [Organization] SET[FollowersCount] = [FollowersCount] - 1 WHERE[OrganizationId] = {organizationId}");
                _context.Follow.Remove(follow);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return new ServiceResponse<Response>
                {
                    IsSuccess = false,
                    StatusCode = ServiceStatus.InternalServerError,
                    Data = new Response
                    {
                        Message = "Failed to unfollow organization",
                        Status = "Failed"
                    }
                };
            }
            return new ServiceResponse<Response>
            {
                IsSuccess = true,
                StatusCode = ServiceStatus.Success,
                Data = new Response
                {
                    Message = "Unfollowed Successfully",
                    Status = "Success"
                }
            };
        }
        public async Task<ServiceResponse<Response>> UpdateOrganization(UpdateOrganizationRequest request)
        {
            var organization = await _context.Organization.FirstOrDefaultAsync(o => o.OrganizationId == request.OrganizationId);
            
            if (organization is null)
            {
                return new ServiceResponse<Response>
                {
                    IsSuccess = false,
                    StatusCode = ServiceStatus.NotFound,
                    Data = new Response
                    {
                        Message = "Organization not found",
                        Status = "Failed"
                    }
                };
            }
            
            organization.Name = request.Name;
            organization.Description = request.Description;
            
            if (request.Logo != null && request.Logo.Length > 0)
            {
                // Save the logo file and update the organization's Logo property
                var fileName = $"{Guid.NewGuid()}_{request.Logo.FileName}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "organizations", fileName);
                
                // Ensure directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.Logo.CopyToAsync(stream);
                }
                
                // Store the relative URL
                organization.Logo = new Uri($"/uploads/organizations/{fileName}", UriKind.Relative);
            }
            
            await _context.SaveChangesAsync();
            
            return new ServiceResponse<Response>
            {
                IsSuccess = true,
                StatusCode = ServiceStatus.Success,
                Data = new Response
                {
                    Message = "Organization updated successfully",
                    Status = "Success"
                }
            };
        }
        public async Task<ServiceResponse<OrganizationDetailsResponse>> GetOrganizationByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new ServiceResponse<OrganizationDetailsResponse>
                {
                    IsSuccess = false,
                    StatusCode = ServiceStatus.BadRequest,
                    Data = null
                };
            }
            var organization = await _context.Organization
                .Where(o => o.UserId == userId)
                .Select(o => new OrganizationDetailsResponse
                {
                    OrganizationId = o.OrganizationId,
                    Description = o.Description,
                    Logo = o.Logo,
                    Name = o.Name,
                    FollowersCount = o.FollowersCount,
                    Links = o.Links.Select(l => new OrganizationLinksReponse { LinkName = l.LinkName, LinkUrl = l.LinkUrl }).ToList()
                })
                .FirstOrDefaultAsync();
            
            if (organization is null)
            {
                return new ServiceResponse<OrganizationDetailsResponse>
                {
                    IsSuccess = false,
                    StatusCode = ServiceStatus.NotFound,
                    Data = null
                };
            }
            
            return new ServiceResponse<OrganizationDetailsResponse>
            {
                IsSuccess = true,
                StatusCode = ServiceStatus.Success,
                Data = organization
            };
        }
    }
}
