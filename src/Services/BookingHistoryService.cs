using Entites;
using Microsoft.EntityFrameworkCore;
using ServiceContracts.DTOs;
using ServiceContracts.ServicesContracts;

namespace Services
{
    public class BookingHistoryService : IBookingHistoryService
    {
        private readonly ApplicationDbContext _context;
        public BookingHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ServiceResponse<IEnumerable<OrderResponse>>> GetAllOrders(string userId)
        {

            var orders = await _context.Orders
                .AsNoTracking()
                .Where(o => o.UserId == userId && o.Status == OrderStatus.Paid)
                .Select(o => new OrderResponse
                {
                    OrderId = o.OrderId,
                    TotalAmount = o.TotalAmount,
                    PaidAt = o.PaidAt,
                    // EF Core 7+ will translate this into CROSS APPLY ... ORDER BY ...
                    EventId = o.Items
                            .Select(oi => oi.TicketType.EventId)
                            .FirstOrDefault(),
                    EventName = o.Items
                                .Select(oi => oi.TicketType.Event.EventName)
                                .FirstOrDefault(),
                    EventPicture = o.Items
                                    .Select(oi => oi.TicketType.Event.EventPicture)
                                    .FirstOrDefault(),
                })
                .ToListAsync();



            return new ServiceResponse<IEnumerable<OrderResponse>>
            {
                Data = orders,
                IsSuccess = true,
                StatusCode = ServiceStatus.Success
            };

        }

        public async Task<ServiceResponse <IEnumerable< OrderDetailsResponse>>> GetOrder(int orderId, string userId)
        {
            var order = await _context.Orders.AsNoTracking()
                .Where(o => o.OrderId == orderId)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Tickets)
                .Include(o=>o.Items)
                    .ThenInclude (oi => oi.TicketType)
                .FirstOrDefaultAsync();

            if (order is null)
            {
                return new ServiceResponse<IEnumerable<OrderDetailsResponse>>
                {
                    IsSuccess = false,
                    StatusCode = ServiceStatus.NotFound,
                    Data = null
                };
            }
            if (order.UserId != userId)
            {
                return new ServiceResponse<IEnumerable<OrderDetailsResponse>>
                {
                    IsSuccess = false,
                    StatusCode = ServiceStatus.Forbidden,
                    Data = null
                };
            }
            var orderDetails = order.Items
                .Select(oi => new OrderDetailsResponse
                {
                    Quantity = oi.Quantity,
                    IsRefunded =  oi.OrderItemRefundStatus == OrderItemRefundStatus.Refunded || oi.OrderItemRefundStatus == OrderItemRefundStatus.PartiallyRefunded,
                    OrderItemId = oi.OrderItemId,
                    TicketTypeName = oi.TicketType.TicketName,
                    TicketTypeDetails = oi.TicketType.Description,
                    UnitPrice = oi.UnitPrice,
                    Tickets = oi.Tickets
                        .Select(t => new TicketResponse
                        {
                            Code = t.Code,
                            Status = GetStatus(t.Status),
                        })
                        .ToList(),
                })
                .ToList();
            return new ServiceResponse<IEnumerable<OrderDetailsResponse>>
            {
                Data = orderDetails,
                IsSuccess = true,
                StatusCode = ServiceStatus.Success
            };
        }
        private string GetStatus(TicketStatus status)
        {
            return status switch
            {
                TicketStatus.Active => "Active",
                TicketStatus.Used => "Used",
                TicketStatus.Refunded => "Refunded",
                _ => "Cancelled"
            };
        }
    }
}
