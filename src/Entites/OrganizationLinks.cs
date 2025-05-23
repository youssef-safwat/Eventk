
namespace Entites;

public class OrganizationLinks
{
    public int OrganizationLinksId { get; set; }
    public Uri? LinkUrl { get; set; }
    public string? LinkName { get; set; }
    public int OrganizationId { get; set; }
    public Organization Organization { get; set; }

}
