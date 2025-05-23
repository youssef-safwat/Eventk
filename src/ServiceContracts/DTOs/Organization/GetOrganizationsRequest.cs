using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTOs.Organization
{
    public class GetOrganizationsRequest
    {
        public bool? IsFollowing { get; set; } = null;
    }
}
