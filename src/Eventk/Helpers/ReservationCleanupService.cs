using Entites;
using Microsoft.EntityFrameworkCore;

namespace Eventk.Helpers
{
    public class ReservationCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ReservationCleanupService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Run every minute until the host shuts down
            var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await ExpireReservationsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    // TODO: log the exception
                }
            }
        }

        private async Task ExpireReservationsAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var strategy = db.Database.CreateExecutionStrategy();

            // Wrap the whole operation so that transient errors (like closed connections)
            // cause an automatic retry.
            await strategy.ExecuteAsync(async () =>
            {
                // Ensure the connection is open throughout this block
                await db.Database.OpenConnectionAsync(ct);

                var now = DateTime.Now;
                var expiredOrders = await db.Orders
                    .Include(o => o.Items)
                    .Where(o => o.Status == OrderStatus.Reserved && o.ExpiresAt <= now)
                    .ToListAsync(ct);

                if (expiredOrders.Count == 0)
                    return;

                foreach (var order in expiredOrders)
                {
                    foreach (var item in order.Items)
                    {
                        // Return seats to pool
                        await db.Database.ExecuteSqlInterpolatedAsync($@"
                            UPDATE TicketType
                               SET TotalTickets = TotalTickets + {item.Quantity}
                             WHERE TicketTypeId = {item.TicketTypeId};
                        ", ct);
                    }

                    order.Status = OrderStatus.Expired;
                }

                await db.SaveChangesAsync(ct);
                await db.Database.CloseConnectionAsync();
            });
        }
    }
}
