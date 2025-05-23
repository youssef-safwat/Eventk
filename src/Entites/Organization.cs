namespace Entites;
public class Organization 
{
    public int OrganizationId { get; set; } 
    public string? Name { get; set; } 
    public string? Description { get; set; } 
    public Uri? Logo { get; set; }
    public ICollection<Event> Events { get; set; } = new List<Event>();
    public ICollection<Follow> Followers { get; set; } = new List<Follow>();
    public ICollection<OrganizationLinks> Links {  get; set; } = new List<OrganizationLinks>();
    public int FollowersCount { get; set; }
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
}
