using ServiceContracts.DTOs;
using ServiceContracts.DTOs.Organization;

namespace ServiceContracts.ServicesContracts
{
    public interface IOrganizationService
    {
        Task<ServiceResponse<OrganizationDetailsResponse>> GetOrganization(int id, string? userId, int pageNumber, int pageSize);
        Task<ServiceResponse<IEnumerable<OrganizationSummaryResponse>>> GetOrganizations(GetOrganizationsRequest organizationsRequest, string? userId);
        Task<ServiceResponse<Response>> FollowOrganization(int organizationId, string userId);
        Task<ServiceResponse<Response>> UnFollowOrganization(int organizationId, string userId);
        
        // New methods for organization management
        Task<ServiceResponse<Response>> UpdateOrganization(UpdateOrganizationRequest request);
        Task<ServiceResponse<OrganizationDetailsResponse>> GetOrganizationByUserId(string userId);
    }
}
