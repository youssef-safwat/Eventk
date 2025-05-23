
namespace Entites;

public class Follow
{
    public ApplicationUser User { get; set; }
    public string UserId { get; set; }
    public int OrganizationId { get; set; }
    public Organization Organization { get; set; }
}
