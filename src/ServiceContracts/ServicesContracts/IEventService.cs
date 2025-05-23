using ServiceContracts.DTOs;
using ServiceContracts.DTOs.Event;
using ServiceContracts.Helpers;

namespace ServiceContracts.ServicesContracts
{
    public interface IEventService
    {
        public Task<ServiceResponse<PaginatedList<EventSummaryResponse>>> GetEvents(EventFilters filters , string? userId);
        public Task<ServiceResponse<EventDetailsReponse>> GetEvent(int eventId , string? userId );
        public Task<ServiceResponse<IEnumerable<TicketTypeResponse>>> GetTicketTypes(int eventId);
    }
}
