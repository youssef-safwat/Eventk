namespace Entites;

public class EventCategory
{
    public int EventId { get; set; }
    public int CategoryId { get; set; }

    // Navigation properties
    public Event Event { get; set; }
    public Category Category { get; set; }
}
