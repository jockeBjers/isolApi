namespace IsolkalkylAPI.Controllers.Organizations;

[Route("api/organization")]
[ApiController]
public class OrganizationController(IOrganizationService organizationService, Validator validator) : ControllerBase
{
    private readonly IOrganizationService _organizationService = organizationService;
    private readonly Validator _validator = validator;

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

    [HttpGet("{id}/users")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetUsersByOrganization(string id)
    {
        try
        {
            var organization = await _organizationService.GetOrganizationById(id);
            if (organization == null)
            {
                Log.Warning("Organization with that id not found");
                return NotFound($"Organization with ID {id} not found");
            }

            var users = await _organizationService.GetUsersByOrganizationId(id);
            var response = users.Select(user => new UserListResponse(
                user.Id,
                user.Name,
                user.Email,
                user.Role,
                user.Organization?.Name
            )).ToList();

            Log.Information("Retrieved {Count} users for the organization", response.Count);
            return Ok(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting users for the organization");
            return StatusCode(500, "An error occurred while retrieving users");
        }
    }

    [HttpGet("{organizationId}/projects")]
    public async Task<ActionResult<List<ProjectListResponse>>> GetProjectsByOrganization(string organizationId)
    {
        try
        {
            var projects = await _organizationService.GetAllProjectsInOrganization(organizationId);
            if (projects == null)
            {
                Log.Warning("No projects found for the organization with that ID");
                return NotFound("No projects found for the organization.");
            }

            var response = projects.Select(p => new ProjectListResponse(
                p.Id,
                p.ProjectNumber,
                p.Name,
                p.FromDate,
                p.ToDate
            )).ToList();
            Log.Information("Retrieved {Count} projects for the organization", response.Count);
            return Ok(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting projects for the organization");
            return StatusCode(500, "An error occurred while retrieving projects.");
        }

    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateOrganization([FromBody] CreateOrganizationRequest request)
    {
        var validation = _validator.Validate(new OrganizationValidator(), request);
        if (validation != null)
            return validation;
        try
        {
            if (await _organizationService.DoesOrganizationExist(request.Id))
            {
                Log.Warning("Attempt to create duplicate organization with ID {OrganizationId}", request.Id);
                return Conflict($"Organization with ID {request.Id} already exists");
            }

            var organization = new Organization
            {
                Id = request.Id,
                Name = request.Name,
                Address = request.Address,
                Phone = request.Phone,
                Email = request.Email
            };

            await _organizationService.AddOrganization(organization);
            Log.Information("Organization with ID {OrganizationId} created successfully", request.Id);
            var response = OrganizationDto.FromOrganization(organization);
            return CreatedAtAction(nameof(GetOrganizationById), new { id = organization.Id }, response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating organization");
            return StatusCode(500, $"An error occurred while creating the organization: {ex.Message}");
        }

    }

    [HttpPut("Profile")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateOrganizationProfile([FromBody] UpdateOrganizationRequest request)
    {
        var validation = _validator.Validate(new OrganizationUpdateValidator(), request);
        if (validation != null)
            return validation;

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Invalid instance");

            var user = await _organizationService.GetUserWithOrganization(userId);
            if (user?.Organization == null)
                return NotFound("Organization not found");

            var existingOrg = user.Organization;

            if (!string.IsNullOrEmpty(request.Email) && request.Email != existingOrg.Email)
            {
                var emailExists = await _organizationService.GetOrganizationByEmail(request.Email);
            }

            if (!string.IsNullOrEmpty(request.Name))
                existingOrg.Name = request.Name;
            if (!string.IsNullOrEmpty(request.Email))
                existingOrg.Email = request.Email;
            if (!string.IsNullOrEmpty(request.Phone))
                existingOrg.Phone = request.Phone;
            if (!string.IsNullOrEmpty(request.Address))
                existingOrg.Address = request.Address;

            var updatedOrg = await _organizationService.UpdateOrganization(existingOrg.Id, existingOrg);
            if (updatedOrg == null)
                return BadRequest("failed to update");

            var response = OrganizationDto.FromOrganization(updatedOrg);

            Log.Information("Organization profile updated successfully!");
            return Ok(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating organization profile for the user");
            return StatusCode(500, "An error occurred while updating organization profile");
        }
    }

    [HttpPut("admin/organization/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateOrganization(string id, [FromBody] UpdateOrganizationRequest request)
    {
        var validation = _validator.Validate(new OrganizationUpdateValidator(), request);
        if (validation != null)
            return validation;

        try
        {
            var existingOrg = await _organizationService.GetOrganizationById(id);
            if (existingOrg == null)
                return NotFound($"Error retrieving organization");

            if (!string.IsNullOrEmpty(request.Name))
                existingOrg.Name = request.Name;
            if (!string.IsNullOrEmpty(request.Email))
                existingOrg.Email = request.Email;
            if (!string.IsNullOrEmpty(request.Phone))
                existingOrg.Phone = request.Phone;
            if (!string.IsNullOrEmpty(request.Address))
                existingOrg.Address = request.Address;

            var updatedOrg = await _organizationService.UpdateOrganization(existingOrg.Id, existingOrg);
            if (updatedOrg == null)
                return BadRequest("failed to update");

            var response = OrganizationDto.FromOrganization(updatedOrg);

            Log.Information("Organization profile updated successfully!");
            return Ok(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating organization profile for the user");
            return StatusCode(500, "An error occurred while updating organization profile");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteOrganization(string id)
    {
        try
        {
            var organization = await _organizationService.GetOrganizationById(id);
            if (organization == null)
            {
                Log.Warning("Attempt to delete non-existing organization with ID {OrganizationId}", id);
                return NotFound($"Organization with ID {id} not found");
            }

            await _organizationService.RemoveOrganizationById(id);
            Log.Information("Organization with ID {OrganizationId} deleted successfully", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting organization with ID {OrganizationId}", id);
            return StatusCode(500, $"An error occurred while deleting the organization: {ex.Message}");
        }
    }
}
