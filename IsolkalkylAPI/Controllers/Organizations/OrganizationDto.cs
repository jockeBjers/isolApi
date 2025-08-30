namespace IsolkalkylAPI.Controllers.Organizations;

using IsolCore.Models;

public class OrganizationDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required string Phone { get; set; }
    public required string Email { get; set; }
    public string? Website { get; set; }

    public int UserCount { get; set; }
    public int ProjectCount { get; set; }
    public int InsulationTypeCount { get; set; }

    public static OrganizationDto FromOrganization(Organization org)
    {
        return new OrganizationDto
        {
            Id = org.Id,
            Name = org.Name,
            Address = org.Address,
            Phone = org.Phone,
            Email = org.Email,
            Website = org.Website,
            UserCount = org.Users?.Count ?? 0,
            ProjectCount = org.Projects?.Count ?? 0,
            InsulationTypeCount = org.InsulationTypes?.Count ?? 0
        };
    }
}

public class CreateOrganizationRequest
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required string Phone { get; set; }
    public required string Email { get; set; }
    public string? Website { get; set; }
}

public class UpdateOrganizationRequest
{
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
}