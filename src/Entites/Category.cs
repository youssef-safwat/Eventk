namespace Entites;

public class Category
{
    public int CategoryId { get; set; }
    public Uri? CategoryImage { get; set; }
    public string? CategoryName { get; set; }
    public ICollection<EventCategory> EventCategory { get; set; } = new List<EventCategory>();
}