
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Entites;

public class TicketType
{
    [Key]
    public int TicketTypeId { get; set; }
    public string Description { get; set; }
    public string TicketName { get; set; }
    public double Price { get; set; }
    public int TotalTickets { get; set; }
    public int EventId { get; set; }
    public Event Event { get; set; }
    public OrderItem OrderItem { get; set; }
}
