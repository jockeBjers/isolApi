namespace IsolkalkylAPI.Controllers.Organizations;

using IsolCore.Models;

public record OrganizationDto(
    string Id,
    string Name,
    string Address,
    string Phone,
    string Email,
    string? Website,
    int UserCount,
    int ProjectCount,
    int InsulationTypeCount
)
{
    public static OrganizationDto FromOrganization(Organization org)
    {
        return new OrganizationDto(
            org.Id,
            org.Name,
            org.Address,
            org.Phone,
            org.Email,
            org.Website,
            org.Users?.Count ?? 0,
            org.Projects?.Count ?? 0,
            org.InsulationTypes?.Count ?? 0
        );
    }
}

public record CreateOrganizationRequest(
    string Id,
    string Name,
    string Address,
    string Phone,
    string Email,
    string? Website
)
{
    public Organization ToOrganization()
    {
        return new Organization
        {
            Id = Id,
            Name = Name,
            Address = Address,
            Phone = Phone,
            Email = Email,
            Website = Website
        };
    }
}

public record UpdateOrganizationRequest
{
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
}

public static class ProjectRequestExtensions
{
    public static void ApplyTo(this UpdateOrganizationRequest request, Organization existingOrganization)
    {
        if (request.Name != null)
            existingOrganization.Name = request.Name;
        if (request.Address != null)
            existingOrganization.Address = request.Address;
        if (request.Phone != null)
            existingOrganization.Phone = request.Phone;
        if (request.Email != null)
            existingOrganization.Email = request.Email;
        if (request.Website != null)
            existingOrganization.Website = request.Website;
    }
}