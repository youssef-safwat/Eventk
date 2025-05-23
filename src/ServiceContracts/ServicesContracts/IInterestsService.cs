using ServiceContracts.DTOs;
using ServiceContracts.DTOs.Event;


namespace ServiceContracts.ServicesContracts;

public interface IInterestsService
{
    Task<ServiceResponse<Response>> AddInterest(InterestAddRequest request, string userId);
    Task<ServiceResponse<Response>> DeleteInterest(InterestAddRequest request, string userId);
    Task<ServiceResponse<ICollection<EventSummaryResponse>>> GetAllInterest(string userId);
}
