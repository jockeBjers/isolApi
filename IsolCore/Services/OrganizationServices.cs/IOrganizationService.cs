namespace IsolCore.Services.OrganizationServices;

public interface IOrganizationService
{
    Task<List<Organization>> GetAllOrganizations();
    Task<Organization?> GetOrganizationById(string organizationId);
    Task<bool> DoesOrganizationExist(string organizationId);
    Task AddOrganization(Organization organization);
    Task<bool> RemoveOrganizationById(string organizationId);
    Task<Organization?> UpdateOrganization(string organizationId, Organization updatedOrganization);
}
