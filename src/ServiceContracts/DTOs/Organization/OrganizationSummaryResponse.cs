using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ServiceContracts.DTOs.Organization
{
    public class OrganizationSummaryResponse
    {
        public int OrganizationId { get; set; }
        public string? Name { get; set; }
        public Uri? Logo { get; set; }
        public int FollowersCount { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IsFollowed { get; set; } = null;
    }

    public class OrganizationResponse
    {
        public IEnumerable<OrganizationSummaryResponse> Following { get; set; } = new List<OrganizationSummaryResponse>();
        public IEnumerable<OrganizationSummaryResponse> ToFollow { get; set; } = new List<OrganizationSummaryResponse>();
    }
}
