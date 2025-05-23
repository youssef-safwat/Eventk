using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace Entites;

public class Ticket
{
    public int TicketId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public Guid Code { get; set; }
    public double PurchasePrice { get; set; }
    public int OrderItemId { get; set; }
    public TicketStatus Status { get; set; }  // Active, Used, Refunded, Cancelled
    public OrderItem OrderItem { get; set; }
}
public enum TicketStatus
{
    Active,
    Used,
    Refunded,
    Cancelled
}