using Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceContracts.DTOs;
using ServiceContracts.DTOs.Event;
using ServiceContracts.ServicesContracts;

namespace Services;

public class InterestsService : IInterestsService
{
    private readonly ApplicationDbContext _context;

    public InterestsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<Response>> AddInterest(InterestAddRequest request, string userId)
    {
        bool isFoundEvent = await _context.Events.AnyAsync(e => e.EventId == request.EventId);
        if (!isFoundEvent)
        {
            return new ServiceResponse<Response>
            {
                StatusCode = ServiceStatus.NotFound,
                IsSuccess = false,
                Data = new Response
                {
                    Status = "Failed",
                    Message = "Event with the given Id Doesn't Exist."
                }
            };
        }
        Interests interest = new Interests { UserId = userId, EventId = request.EventId };
        try
        {
            _context.Interests.Add(interest);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new ServiceResponse<Response>
            {
                StatusCode = ServiceStatus.Conflict,
                IsSuccess = false,
                Data = new Response { Status = "Failed", Message = "Already Intersted in." }
            };
        }
        return new ServiceResponse<Response>
        {
            StatusCode = ServiceStatus.Success,
            IsSuccess = true,
            Data = new Response
            {
                Status = "Success",
                Message = "Add Successfully"
            }
        };
    }
    public async Task<ServiceResponse<Response>> DeleteInterest(InterestAddRequest request, string userId)
    {
        Interests? interest = await _context.Interests.FindAsync(request.EventId, userId);
        if (interest is null)
        {
            return new ServiceResponse<Response>
            {
                StatusCode = ServiceStatus.NotFound,
                IsSuccess = false,
                Data = new Response
                {
                    Status = "Failed",
                    Message = "Not Interest for this user to remove."
                }
            };
        }
        _context.Interests.Remove(interest);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new ServiceResponse<Response>
            {
                StatusCode = ServiceStatus.InternalServerError,
                IsSuccess = false,
                Data = new Response { Status = "Failed", Message = "An error occurred." }
            };
        }
        return new ServiceResponse<Response>
        {
            StatusCode = ServiceStatus.Success,
            IsSuccess = true,
            Data = new Response
            {
                Status = "Success",
                Message = "Removed Successfully"
            }
        }; ;
    }
    public async Task<ServiceResponse<ICollection<EventSummaryResponse>>> GetAllInterest(string userId)
    {
        var interestedEvents = await _context.Interests
          .AsNoTracking()
          .Where(i => i.UserId == userId)
          .Select(i => new EventSummaryResponse
          {
              EventId = i.EventId,
              EventName = i.Event.EventName,
              StartDate = i.Event.StartDate,
              EventPicture = i.Event.EventPicture,
              IsInterested = true,
          })
          .ToListAsync();
        return new ServiceResponse<ICollection<EventSummaryResponse>>
        {
            IsSuccess = true,
            StatusCode = ServiceStatus.Success,
            Data = interestedEvents
        };
    }
}
