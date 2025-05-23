using System.Text.Json.Serialization;
using ServiceContracts.DTOs.Event;
using ServiceContracts.Helpers;
namespace ServiceContracts.DTOs.Organization
{
    public class OrganizationDetailsResponse
    {
        public int OrganizationId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Uri? Logo { get; set; }
        public PaginatedList<EventSummaryResponse> Events { get; set; } = default!;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IsFollowed { get; set; } = null;
        public int FollowersCount { get; set; }
        public ICollection<OrganizationLinksReponse> Links { get; set; } = new List<OrganizationLinksReponse>();
    }

    public class OrganizationLinksReponse
    {
        public Uri? LinkUrl { get; set; }
        public string? LinkName { get; set; }
    }
}
