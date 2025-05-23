namespace Entites;
public class Interests
{
    public int EventId { get; set; }
    public string UserId { get; set; }
    public Event Event { get; set; }
    public ApplicationUser User { get; set; }
}
