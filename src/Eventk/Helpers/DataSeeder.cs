//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using Bogus;
//using Entites;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;

//namespace DataSeeding
//{
//    public static class DataSeeder
//    {
//        public static async Task SeedAsync(IServiceProvider services)
//        {
//            using var scope = services.CreateScope();
//            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
//            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

//            // Apply migrations
//            await context.Database.MigrateAsync();

//            // ==== CLEAR EXISTING DATA ====
//            context.Tickets.RemoveRange(context.Tickets);
//            context.Set<TicketType>().RemoveRange(context.Set<TicketType>());
//            context.EventCategories.RemoveRange(context.EventCategories);
//            context.Interests.RemoveRange(context.Interests);
//            context.Follow.RemoveRange(context.Follow);
//            context.OrganizationLinks.RemoveRange(context.OrganizationLinks);
//            context.Events.RemoveRange(context.Events);
//            context.Organization.RemoveRange(context.Organization);
//            context.Set<IdentityUserRole<string>>().RemoveRange(context.Set<IdentityUserRole<string>>());
//            context.Users.RemoveRange(context.Users);
//            await context.SaveChangesAsync();

//            // ==== SEED ROLES ====
//            string[] roles = { "Admin", "Organization", "User" };
//            foreach (var role in roles)
//                if (!await roleManager.RoleExistsAsync(role))
//                    await roleManager.CreateAsync(new IdentityRole(role));

//            // ==== SEED CATEGORIES ====
//            context.Categories.AddRange(
//                new Category { CategoryName = "Party" },
//                new Category { CategoryName = "Sports" },
//                new Category { CategoryName = "Food" }
//            );
//            await context.SaveChangesAsync();

//            // ==== SEED USERS ====
//            var userFaker = new Faker<ApplicationUser>()
//                .RuleFor(u => u.UserName, f => f.Internet.UserName())
//                .RuleFor(u => u.Email, f => f.Internet.Email())
//                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
//                .RuleFor(u => u.LastName, f => f.Name.LastName())
//                .RuleFor(u => u.BirthDate, f => DateOnly.FromDateTime(f.Date.Past(30, DateTime.Today.AddYears(-18))))
//                .RuleFor(u => u.Bio, f => f.Lorem.Sentence())
//                .RuleFor(u => u.ProfilePicture, f => new Uri(f.Internet.Avatar()));
//            var users = userFaker.Generate(500);
//            foreach (var user in users)
//            {
//                var result = await userManager.CreateAsync(user, "Password123!");
//                if (!result.Succeeded)
//                    throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
//            }
//            var userRole = await roleManager.FindByNameAsync("User");
//            var userRoles = users.Select(u => new IdentityUserRole<string> { UserId = u.Id, RoleId = userRole.Id });
//            context.Set<IdentityUserRole<string>>().AddRange(userRoles);
//            await context.SaveChangesAsync();

//            // ==== SEED ORGANIZATIONS ====
//            var orgFaker = new Faker<Organization>()
//                .RuleFor(o => o.Name, f => f.Company.CompanyName())
//                .RuleFor(o => o.Description, f => f.Company.CatchPhrase())
//                .RuleFor(o => o.Logo, f => new Uri(f.Internet.Avatar()));
//            var orgOwners = users.OrderBy(_ => Guid.NewGuid()).Take(50).ToList();
//            foreach (var owner in orgOwners)
//            {
//                var org = orgFaker.Generate();
//                org.UserId = owner.Id;
//                context.Organization.Add(org);
//            }
//            await context.SaveChangesAsync();

//            // ==== SEED EVENTS ====
//            var allOrgs = context.Organization.ToList();
//            var eventFaker = new Faker<Event>()
//                .RuleFor(e => e.EventName, f => f.Lorem.Sentence(3))
//                .RuleFor(e => e.Description, f => f.Lorem.Paragraph())
//                .RuleFor(e => e.EventPicture, f => new Uri(f.Image.PlaceImgUrl()))
//                .RuleFor(e => e.StartDate, f => f.Date.FutureOffset().DateTime)
//                .RuleFor(e => e.EndDate, (f, e) => e.StartDate.AddDays(f.Random.Int(1, 5)))
//                .RuleFor(e => e.AvailableTickets, f => f.Random.Int(50, 500))
//                .RuleFor(e => e.Status, "Active")
//                .RuleFor(e => e.CreateAt, f => DateTime.UtcNow)
//                .RuleFor(e => e.OrganizationID, f => f.PickRandom(allOrgs).OrganizationId);
//            var events = eventFaker.Generate(200);
//            context.Events.AddRange(events);
//            await context.SaveChangesAsync();

//            // ==== SEED EVENT-CATEGORIES ====
//            var categoryIds = context.Categories.Select(c => c.CategoryId).ToArray();
//            foreach (var ev in context.Events)
//            {
//                var picks = categoryIds.OrderBy(_ => Guid.NewGuid()).Take(new Random().Next(1, 3));
//                foreach (var catId in picks)
//                    context.EventCategories.Add(new EventCategory { EventId = ev.EventId, CategoryId = catId });
//            }
//            await context.SaveChangesAsync();

//            // ==== SEED INTERESTS & FOLLOWS ====
//            var allUserIds = users.Select(u => u.Id).ToList();
//            var allEventIds = context.Events.Select(e => e.EventId).ToList();
//            var allOrgIds = allOrgs.Select(o => o.OrganizationId).ToList();
//            int interestCount = 0, followCount = 0;

//            for (int ui = 0; ui < allUserIds.Count && (interestCount < 1000 || followCount < 1000); ui++)
//            {
//                for (int ei = 0; ei < allEventIds.Count && interestCount < 1000; ei++)
//                {
//                    context.Interests.Add(new Interests { UserId = allUserIds[ui], EventId = allEventIds[ei] });
//                    interestCount++;
//                }
//                for (int oi = 0; oi < allOrgIds.Count && followCount < 1000; oi++)
//                {
//                    context.Follow.Add(new Follow { UserId = allUserIds[ui], OrganizationId = allOrgIds[oi] });
//                    followCount++;
//                }
//            }
//            await context.SaveChangesAsync();

//            // ==== SEED TICKETS ====
//            var rnd = new Random();
//            var tickets = new List<Ticket>();
//            for (int i = 0; i < 2000; i++)
//            {
//                tickets.Add(new Ticket
//                {
//                    EventId = events[rnd.Next(events.Count)].EventId,
//                    UserId = allUserIds[rnd.Next(allUserIds.Count)],
//                    PurchaseDate = DateTime.UtcNow
//                });
//            }
//            context.Tickets.AddRange(tickets);
//            await context.SaveChangesAsync();

//            // ==== SEED TICKET TYPES ====
//            string[] ticketTypeNames = { "Standard", "VIP", "Premium" };
//            var savedTickets = context.Tickets.ToList();
//            foreach (var ticket in savedTickets)
//            {
//                var typeName = ticketTypeNames[rnd.Next(ticketTypeNames.Length)];
//                decimal price = typeName switch { "Standard" => 20m, "VIP" => 50m, _ => 100m };
//                context.TicketType.Add(new TicketType
//                {
//                    EventId = ticket.EventId,
//                    TicketId = ticket.TicketId,
//                    TicketName = typeName,
//                    Price = price
//                });
//            }
//            await context.SaveChangesAsync();
//        }
//    }
//}
