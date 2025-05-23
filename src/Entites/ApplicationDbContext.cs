using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Entites;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Event>()
            .Property(e => e.Location)
            .HasColumnType("geography"); // Use geography for spherical calculations

        modelBuilder.Entity<Follow>()
          .HasOne(f => f.User)
          .WithMany(u => u.Following)
          .HasForeignKey(f => f.UserId)
          .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Order>()
            .HasOne(e => e.User)
            .WithMany(o => o.Orders);
        modelBuilder.Entity<OrderItem>()
            .HasOne(o => o.Order)
            .WithMany(o => o.Items);
        modelBuilder.Entity<Follow>()
            .HasOne(f => f.Organization)
            .WithMany(o => o.Followers)
            .HasForeignKey(f => f.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Organization>()
              .HasOne(u => u.User)
              .WithOne(o => o.Organization)
              .HasForeignKey<Organization>(o => o.UserId)
              .IsRequired(false) // Explicitly mark as nullable
              .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<EventCategory>()
          .HasOne(ec => ec.Event)
          .WithMany(e => e.EventCategory)
          .HasForeignKey(ec => ec.EventId);
        modelBuilder.Entity<EventCategory>()
            .HasOne(ec => ec.Category)
            .WithMany(c => c.EventCategory)
            .HasForeignKey(ec => ec.CategoryId);
        modelBuilder.Entity<EventCategory>()
          .ToTable("EventCategories"); // Match the SQL table name
        modelBuilder.Entity<EventCategory>()
        .HasKey(ec => new { ec.EventId, ec.CategoryId });

        modelBuilder.Entity<Interests>()
        .HasKey(i => new { i.EventId, i.UserId });

        modelBuilder.Entity<Follow>()
        .HasKey(f => new { f.UserId, f.OrganizationId });
    }
    public DbSet<Event> Events { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<EventCategory> EventCategories { get; set; }
    public DbSet<Organization> Organization { get; set; }
    public DbSet<OrganizationLinks> OrganizationLinks { get; set; }
    public DbSet<Interests> Interests { get; set; }
    public DbSet<Follow> Follow { get; set; }
    public DbSet<TicketType> TicketType { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }


    private static void SeedRoles(ModelBuilder builder)
    {
        builder.Entity<IdentityRole>().HasData
            (
            new IdentityRole() { Name = "Organization", ConcurrencyStamp = "1", NormalizedName = "ORGANIZATION" },
            new IdentityRole() { Name = "User", ConcurrencyStamp = "2", NormalizedName = "USER" },
            new IdentityRole() { Name = "Admin", ConcurrencyStamp = "3", NormalizedName = "ADMIN" }
            );
        builder.Entity<Category>().HasData(

            );
    }
    private static void SeedData(ModelBuilder builder)
    {
        builder.Entity<Category>().HasData(
            new Category { CategoryId = 1, CategoryName = "Party" },
            new Category { CategoryId = 2, CategoryName = "Sports" },
            new Category { CategoryId = 3, CategoryName = "Food" }
            );
    }
}