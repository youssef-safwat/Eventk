using Entites;
using Mapster;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using ServiceContracts.DTOs;
using ServiceContracts.DTOs.Event;
using ServiceContracts.Helpers;
using ServiceContracts.ServicesContracts;
namespace Services
{
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;
        private readonly TypeAdapterConfig _config;
        public EventService(ApplicationDbContext applicationDbContext, TypeAdapterConfig config)
        {
            _context = applicationDbContext;
            _config = config;
        }
        public async Task<ServiceResponse<PaginatedList<EventSummaryResponse>>> GetEvents(EventFilters filters, string? userId)
        {

            var events = _context.Events
                .AsNoTracking()
                .Include(e => e.EventCategory)
                    .ThenInclude(c => c.Category)
                .AsQueryable();
            if (filters.Name is not null)
            {
                events = events.Where(e => e.EventName.Contains(filters.Name.Trim()));
            }
            events = events
                .OrderByDescending(e => e.Interests.Count());
                //.ThenByDescending(e => e.TicketTypes.Sum(t => t.Tickets.Count()));

            if (filters.CategoriesIds is not null)
            {
                events = events.Where(e =>
                    e.EventCategory.Select(ec => ec.CategoryId).Count() == filters.CategoriesIds.Distinct().Count() &&
                    filters.CategoriesIds.All(c =>
                        e.EventCategory.Select(ec => ec.CategoryId).Contains(c))
                );
            }

            if (filters.FromDate is not null && filters.ToDate is not null)
            {
                events = events.Where(e =>
                    e.StartDate >= filters.FromDate &&
                    e.EndDate <= filters.ToDate
                );
            }

            if (filters.IsPaid is not null)
            {
                if (filters.IsPaid.Value)
                    events = events.Where(e => e.TicketTypes.All(t => t.Price != 0.00));
                else
                    events = events.Where(e => e.TicketTypes.All(t => t.Price == 0.00));
            }

            if (filters.SortBy is not null)
            {
                var normalizedSortBy = filters.SortBy.Trim().ToLower();

                if (filters.MinPrice < 0 || filters.MaxPrice < 0)
                    return new ServiceResponse<PaginatedList<EventSummaryResponse>>
                    {
                        Data = null,
                        IsSuccess = false,
                        StatusCode = ServiceStatus.BadRequest
                    };
                // Sort from lowset to highest as default
                if (filters.IsAscending)
                {
                    if (normalizedSortBy == "price")
                    {
                        events = events
                            .Where(e => e.TicketTypes.Any(tt =>
                                tt.Price >= filters.MinPrice &&
                                tt.Price <= filters.MaxPrice
                            ))
                            .OrderBy(e =>
                                e.TicketTypes
                                 .Where(tt =>
                                     tt.Price >= filters.MinPrice &&
                                     tt.Price <= filters.MaxPrice)
                                 .Min(tt => tt.Price)
                            );
                    }
                    else
                    {
                        events = events.OrderBy(e => e.StartDate);
                    }
                }
                else
                {
                    if (normalizedSortBy == "price")
                    {
                        events = events
                            .Where(e => e.TicketTypes.Any(tt =>
                                tt.Price >= filters.MinPrice &&
                                tt.Price <= filters.MaxPrice
                            ))
                            .OrderByDescending(e =>
                                e.TicketTypes
                                 .Where(tt =>
                                     tt.Price >= filters.MinPrice &&
                                     tt.Price <= filters.MaxPrice)
                                 .Min(tt => tt.Price)
                            );
                    }
                    else
                    {
                        events = events.OrderByDescending(e => e.StartDate);
                    }
                }
            }

            if (filters.Latitude is not null && filters.Longitude is not null)
            {
                var userLocation = new Point(
                    filters.Longitude.Value,
                    filters.Latitude.Value
                )
                { SRID = 4326 };

                events = events
                    .Where(e => e.Location.Distance(userLocation) <= filters.Radius * 1000)
                    .OrderBy(e => e.Location.Distance(userLocation));
            }


            PaginatedList<EventSummaryResponse> paginatedList = new();

            if (filters.PageNumber is not null && filters.PageSize is not null)
            {
                if (filters.PageNumber.Value == 0)
                {


                    var items = events
                        .Select(e => e.Adapt<EventSummaryResponse>(_config));

                    paginatedList = await PaginatedList<EventSummaryResponse>.CreateAsync(items, 1, 5);
                }
                else
                {
                    if (filters.PageNumber.Value < 0)
                        return new ServiceResponse<PaginatedList<EventSummaryResponse>>
                        {
                            Data = null!,
                            IsSuccess = false,
                            StatusCode = ServiceStatus.BadRequest
                        };

                    paginatedList = await PaginatedList<EventSummaryResponse>.CreateAsync(
                        events
                            .Select(e => e.Adapt<EventSummaryResponse>(_config)),
                        filters.PageNumber.Value,
                        filters.PageSize.Value
                    );
                }
            }
            // —————————————
            // 3) mark IsInterested exactly as before
            // —————————————
            if (userId is not null)
            {
                var interestedIds = await _context.Interests
                    .Where(i => i.UserId == userId)
                    .Select(i => i.EventId)
                    .ToListAsync();

                paginatedList.Items.ForEach(e =>
                    e.IsInterested = interestedIds.Contains(e.EventId)
                );
            }


            return new ServiceResponse<PaginatedList<EventSummaryResponse>>
            {
                Data = paginatedList,
                IsSuccess = true,
                StatusCode = ServiceStatus.Success
            };
        }
        public async Task<ServiceResponse<EventDetailsReponse>> GetEvent(int eventId, string? userId)
        {
            var @event = await _context.Events.AsNoTracking()
                .Where(e => e.EventId == eventId)
                .Select(e => new EventDetailsReponse
                {
                    EventId = eventId,
                    MaxPrice = e.TicketTypes.Max(t => t.Price),
                    MinPrice = e.TicketTypes.Min(t => t.Price),
                    EventName = e.EventName,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    EventPicture = e.EventPicture,
                    IsInterested = userId != null ? e.Interests.Any(i => i.UserId == userId) : null,
                    OrganizationId = e.OrganizationID,
                    OrganizationName = e.Organization.Name,
                    InterestedPeople = e.Interests.Count(),
                    IsPaid = e.TicketTypes.All(t => t.Price != 0.00),
                    About = new About
                    {
                        aboutCategories = e.EventCategory.Select(c => new AboutCategory
                        {
                            CategoryId = c.Category.CategoryId,
                            CategoryName = c.Category.CategoryName
                        }),
                        Description = e.Description
                    },
                    Latitude = e.Location.Y,
                    Longitude = e.Location.X
                }).FirstOrDefaultAsync();
            if (@event is null)
            {
                return new ServiceResponse<EventDetailsReponse>
                {
                    IsSuccess = false,
                    StatusCode = ServiceStatus.NotFound,
                    Data = null
                };
            }
            return new ServiceResponse<EventDetailsReponse>
            {
                IsSuccess = true,
                StatusCode = ServiceStatus.Success,
                Data = @event
            };
        }

        public async Task<ServiceResponse<IEnumerable< TicketTypeResponse>>> GetTicketTypes(int eventId)
        {
            if( await _context.Events.AnyAsync(e=>e.EventId == eventId) == false)
            {
                return new ServiceResponse<IEnumerable<TicketTypeResponse>>
                {
                    IsSuccess = false,
                    StatusCode = ServiceStatus.NotFound,
                    Data = null
                };
            }

            var ticketTypes = await _context.TicketType.AsNoTracking()
                .Where(t => t.EventId == eventId)
                .Select(t => t.Adapt<TicketTypeResponse>(_config))
                .ToListAsync();

            if(ticketTypes.Count() > 0)
            {
                return new ServiceResponse<IEnumerable<TicketTypeResponse>>
                {
                    IsSuccess = true,
                    StatusCode = ServiceStatus.Success,
                    Data = ticketTypes
                };
            }

            var exist = await _context.Events
                .AsNoTracking()
                .AnyAsync(e => e.EventId == eventId);

            if (!exist)
            {
                return new ServiceResponse<IEnumerable<TicketTypeResponse>>
                {
                    IsSuccess = false,
                    StatusCode = ServiceStatus.NotFound,
                    Data = null
                };
            }

            return new ServiceResponse<IEnumerable<TicketTypeResponse>>
            {
                IsSuccess = true,
                StatusCode = ServiceStatus.Success,
                Data = []
            };

        }
    }
}
