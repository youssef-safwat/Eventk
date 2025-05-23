using Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceContracts.DTOs;
using ServiceContracts.DTOs.Payment;
using ServiceContracts.Helpers;
using ServiceContracts.ServicesContracts;
using System.Data;

namespace Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly PaymobService _paymobApi;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;

        public PaymentService(
            ApplicationDbContext context,
            PaymobService paymob,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            IServiceScopeFactory scopeFactory)
        {
            _context = context;
            _paymobApi = paymob;
            _configuration = configuration;
            _userManager = userManager;
            _scopeFactory = scopeFactory;
        }

        public async Task<ServiceResponse<string>> PaymentIntent(PaymentRequest paymentRequest,string userId)
        {
            // Start a transaction so we can roll back both the inventory updates and the order insert
            await using var tx = await _context.Database
                .BeginTransactionAsync(IsolationLevel.ReadCommitted);

            var paymobItems = new List<Item>();

            // 1) Atomically decrement remaining tickets
            foreach (var line in paymentRequest.Items)
            {
                var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync($@"
                    UPDATE TicketType
                       SET TotalTickets = TotalTickets - {line.Quantity}
                     WHERE TicketTypeId = {line.TicketTypeId}
                       AND TotalTickets >= {line.Quantity};
                ");

                if (rowsAffected == 0)
                {
                    await tx.RollbackAsync();
                    return new ServiceResponse<string>
                    {
                        Data = $"Not enough tickets for type {line.TicketTypeId}",
                        IsSuccess = false,
                        StatusCode = ServiceStatus.NotFound
                    };
                }
            }

            // 2) Build Order + Paymob DTOs
            var order = new Order
            {
                UserId = userId,
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddMinutes(15),
                Status = OrderStatus.Reserved,
                TotalAmount = 0,
                Items = new List<OrderItem>()
            };

            foreach (var line in paymentRequest.Items)
            {
                var info = await _context.TicketType
                    .Where(tt => tt.TicketTypeId == line.TicketTypeId)
                    .Select(tt => new
                    {
                        tt.Price,
                        EventName = tt.Event.EventName,
                        Description = tt.Event.Description
                    })
                    .FirstAsync();

                order.Items.Add(new OrderItem
                {
                    TicketTypeId = line.TicketTypeId,
                    Quantity = line.Quantity,
                    UnitPrice = info.Price,
                    TotalPrice = info.Price * line.Quantity
                });
                order.TotalAmount += info.Price * line.Quantity;

                paymobItems.Add(new Item
                {
                    amount = (int)(info.Price * 100),
                    name = info.EventName,
                    description = info.Description,
                    quantity = line.Quantity
                });
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // 3) Create Paymob payment intention
            var user = await _userManager.FindByIdAsync(userId);
            var paymobRequest = new PaymentIntentionRequest
            {
                amount = (int)(order.TotalAmount * 100),
                items = paymobItems,
                billing_data = new BillingData
                {
                    email = user.Email,
                    first_name = user.FirstName,
                    last_name = user.LastName,
                    phone_number = paymentRequest.PhoneNumber
                }
            };

            var paymobResponse = await _paymobApi
                .CreatePaymentIntentionAsync(paymobRequest);

            order.PaymentOrderId = paymobResponse.intention_order_id;
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            var checkoutUrl =
                $"https://accept.paymob.com/unifiedcheckout/?" +
                $"publicKey={_configuration["Paymob:PublicKey"]}" +
                $"&clientSecret={paymobResponse.client_secret}";

            return new ServiceResponse<string>
            {
                Data = checkoutUrl,
                IsSuccess = true,
                StatusCode = ServiceStatus.Success
            };
        }
        // NOTE: Changed async void → async Task, and using a fresh scoped DbContext
        public async Task HandleCallBack(CallBackRequest request)
        {
            // Create a new scope for callback processing
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Load the reserved order
            var order = await db.Orders
                .FirstOrDefaultAsync(o => o.PaymentOrderId == request.OrderId);

            if (order == null || order.Status != OrderStatus.Reserved)
                return;

            if (request.IsSuccessful)
            {
                order.Status = OrderStatus.Paid;
                order.PaidAt = request.PaidAt;
                order.PaymentTransactionId = request.TransactionId;

                // Generate tickets for each OrderItem
                var items = await db.OrderItems
                    .Where(i => i.OrderId == order.OrderId)
                    .ToListAsync();

                foreach (var item in items)
                {
                    for (int i = 0; i < item.Quantity; i++)
                    {
                        db.Tickets.Add(new Ticket
                        {
                            OrderItemId = item.OrderItemId,
                            Code = Guid.NewGuid(),
                            PurchasePrice = item.UnitPrice,
                            PurchaseDate = DateTime.Now,
                            Status = TicketStatus.Active
                        });
                    }
                }

                await db.SaveChangesAsync();
            }
        }
        public async Task<ServiceResponse<Response>> PaymentRefund(RefundRequest request, string userId)
        {
            // 1. Fetch order + ensure it's paid and belongs to the user
            var order = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.TicketType)
                        .ThenInclude(tt => tt.Event)
                .FirstOrDefaultAsync(o => o.OrderId == request.OrderId && o.UserId == userId);

            if (order == null || order.Status != OrderStatus.Paid)
                return new ServiceResponse<Response>
                {
                    IsSuccess = false,
                    StatusCode = ServiceStatus.NotFound,
                    Data = new Response 
                    {
                        Status = "Failed",
                        Message = "Order not found or not paid."
                    }
                };

            // 2. Check refund time windows
            var now = DateTime.Now;
            if (now > order.PaidAt.AddDays(2))
                return new ServiceResponse<Response>
                {
                    IsSuccess = false,
                    StatusCode = ServiceStatus.Conflict,
                    Data = new Response{ Message =  "Refund period (2 days) has expired.", Status = "Failed" }
                };

            var eventStart = order.Items.First().TicketType.Event.StartDate;
            if (now > eventStart.AddDays(-5))
                return new ServiceResponse<Response>
                {
                    IsSuccess = false,
                    StatusCode = ServiceStatus.Conflict,
                    Data = new Response { Status = "Failed", Message = "Refund cutoff (5 days before event) has passed." }
                };

            // 3. Begin transaction
            await using var tx = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            double totalRefundAmount = 0;
            foreach (var line in request.Items)
            {
                var item = order.Items.FirstOrDefault(i => i.OrderItemId == line.OrderItemId);
                if (item == null)
                    return BadLine($"OrderItem {line.OrderItemId} not found.");

                // how many remain refundable?
                var canRefund = item.Quantity - item.RefundedQuantity;
                if (line.Quantity <= 0 || line.Quantity > canRefund)
                    return BadLine($"Invalid refund qty for item {line.OrderItemId}.");

                // 4. Mark that many Tickets as refunded
                var ticketsToRefund = await _context.Tickets
                    .Where(t => t.OrderItemId == item.OrderItemId && t.Status == TicketStatus.Active)
                    .Take(line.Quantity)
                    .ToListAsync();

                if (ticketsToRefund.Count < line.Quantity)
                    return BadLine($"Only {ticketsToRefund.Count} active tickets remain for item {line.OrderItemId}.");

                foreach (var t in ticketsToRefund)
                    t.Status = TicketStatus.Refunded;

                // 5. Return seats to pool
                await _context.Database.ExecuteSqlInterpolatedAsync($@"
                UPDATE TicketType
                   SET TotalTickets = TotalTickets + {line.Quantity}
                 WHERE TicketTypeId = {item.TicketTypeId};
            ");

                // 6. Update refund-tracking on the OrderItem
                var refundAmount = item.UnitPrice * line.Quantity;
                item.RefundedQuantity += line.Quantity;
                item.RefundedAmount += refundAmount;
                item.OrderItemRefundStatus = item.RefundedQuantity == item.Quantity
                    ? OrderItemRefundStatus.Refunded
                    : OrderItemRefundStatus.PartiallyRefunded;

                totalRefundAmount += refundAmount;
            }

            // 7. Call Paymob refund API once for the full total
            try
            {
                var transactionId = order.PaymentTransactionId.ToString();
                var amountCents = ((int)(totalRefundAmount * 100)).ToString();
                await _paymobApi.RefundTransactionAsync(transactionId, amountCents);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return new ServiceResponse<Response>
                {
                    IsSuccess = false,
                    StatusCode = ServiceStatus.InternalServerError,
                    Data = new Response { Message = "Payment provider refund failed: " + ex.Message, Status = "Failed" }
                };
            }

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return new ServiceResponse<Response>
            {
                IsSuccess = true,
                StatusCode = ServiceStatus.Success,
                Data = new Response { Status = "Success", Message = $"Refunded {totalRefundAmount:C} successfully." }
            };

            ServiceResponse<Response> BadLine(string msg) => new()
            {
                IsSuccess = false,
                StatusCode = ServiceStatus.Conflict,
                Data = new Response { Message = msg, Status = "Failed" }
            };
        }
    }
}