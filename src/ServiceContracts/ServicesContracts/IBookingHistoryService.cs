using ServiceContracts.DTOs;

namespace ServiceContracts.ServicesContracts
{
    public interface IBookingHistoryService
    {
        Task<ServiceResponse<IEnumerable<OrderResponse>>> GetAllOrders(string userId);
        Task<ServiceResponse<IEnumerable<OrderDetailsResponse>>> GetOrder(int orderId, string userId);
    }
}
