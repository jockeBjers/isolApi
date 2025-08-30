using System.Security.Claims;
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
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetOrganizations()
    {
        try
        {
            var organizations = await _organizationService.GetAllOrganizations();
            var orgDtos = organizations.Select(OrganizationDto.FromOrganization).ToList();
            Log.Information("Retrieved all organizations successfully");
            return Ok(orgDtos);
        }
        catch (Exception)
        {
            Log.Error("An error occurred while retrieving organizations");
            return StatusCode(500, "An error occurred while retrieving organizations");
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetOrganizationById(string id)
    {
        try
        {
            var organization = await _organizationService.GetOrganizationById(id);
            if (organization == null)
            {
                Log.Warning("Organization with ID {OrganizationId} not found", id);
                return NotFound($"Organization with ID {id} not found");
            }

            var orgDto = OrganizationDto.FromOrganization(organization);
            Log.Information("Organization with ID {OrganizationId} retrieved successfully", id);
            return Ok(orgDto);
        }
        catch (Exception)
        {
            Log.Error("An error occurred while retrieving the organization");
            return StatusCode(500, "An error occurred while retrieving the organization");
        }
    }
    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetOrganizationProfile()
    {

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            Log.Warning("User ID claim is missing in the token");
            return BadRequest("User ID is not available");
        }

        var user = await _organizationService.GetUserWithOrganization(userId);
        if (user == null || user.Organization == null)
        {
            Log.Warning("Organization profile not found for the user");
            return NotFound("Organization profile not found");
        }

        var orgDto = OrganizationDto.FromOrganization(user.Organization);
        Log.Information("Organization profile retrieved for current user");
        return Ok(orgDto);
    }
}
