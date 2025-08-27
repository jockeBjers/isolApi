using IsolkalkylAPI.Controllers.Organizations;
using Microsoft.AspNetCore.Authorization;

[Route("api/organization")]
[ApiController]
public class OrganizationController : ControllerBase
{
    private readonly IOrganizationService _organizationService;

    public OrganizationController(IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    [HttpGet("organizations")]
    [Authorize]
    public async Task<IActionResult> GetOrganizations()
    {
        try
        {
            var organizations = await _organizationService.GetAllOrganizations();
            var orgDtos = organizations.Select(OrganizationDto.FromOrganization).ToList();
            return Ok(orgDtos);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while retrieving organizations");
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetOrganizationById(string id)
    {
        try
        {
            var organization = await _organizationService.GetOrganizationById(id);
            if (organization == null)
            {
                return NotFound($"Organization with ID {id} not found");
            }

            var orgDto = OrganizationDto.FromOrganization(organization);
            return Ok(orgDto);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while retrieving the organization");
        }
    }
}
