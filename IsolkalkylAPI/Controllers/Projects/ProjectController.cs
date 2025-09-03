namespace IsolkalkylAPI.Controllers.Projects;

using System.Security.Claims;

[Route("api/projects")]
[ApiController]
[Authorize]
public class ProjectController(IProjectService projectService, Validator validator) : ControllerBase
{
    private readonly IProjectService _projectService = projectService;
    private readonly Validator _validator = validator;

    [HttpGet("{projectNumber}")]
    public async Task<IActionResult> GetProjectByProjectNumber(string projectNumber)
    {
        try
        {
            var userOrganizationId = User.FindFirstValue("organizationId");
            if (string.IsNullOrEmpty(userOrganizationId))
            {
                Log.Warning("User has no organization ID in token");
                return StatusCode(403, "Access denied");
            }

            var project = await _projectService.GetProjectByProjectNumber(projectNumber);
            if (project == null)
            {
                Log.Error("Project not found");
                return NotFound("Could not find project");
            }

            // Ensure user can only access projects from their organization
            if (project.OrganizationId != userOrganizationId && !User.IsInRole("Admin"))
            {
                Log.Warning("User attempted to access project from different organization. User org: {UserOrg}, Project org: {ProjectOrg}", 
                    userOrganizationId, project.OrganizationId);
                return StatusCode(403, "Access denied - you can only access projects within your organization");
            }

            var projectResponse = ProjectResponse.FromProject(project);

            Log.Information("Project retrieved successfully");
            return Ok(projectResponse);
        }
        catch (Exception)
        {
            Log.Error("Error retrieving project");
            return StatusCode(500, "Error retrieving project");
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
    {

        var validation = _validator.Validate(new ProjectValidator(), request);
        if (validation != null) return validation;

        try
        {
            var userOrganizationId = User.FindFirstValue("organizationId");
            if (string.IsNullOrEmpty(userOrganizationId))
            {
                Log.Warning("User has no organization ID in token");
                return StatusCode(403, "Access denied");
            }

            // Ensure user can only create projects for their own organization (unless Admin)
            if (request.OrganizationId != userOrganizationId && !User.IsInRole("Admin"))
            {
                Log.Warning("User attempted to create project for different organization. User org: {UserOrg}, Request org: {RequestOrg}", 
                    userOrganizationId, request.OrganizationId);
                return StatusCode(403, "Access denied - you can only create projects within your organization");
            }

            var existingProject = await _projectService.DoesProjectExist(request.ProjectNumber);
            if (existingProject)
            {
                Log.Warning("Project with this project number already exists");
                return Conflict("A project with this project number already exists!");
            }

            var project = request.ToProject();

            await _projectService.AddProject(project);
            Log.Information("Project created succesfully");

            var response = ProjectResponse.FromProject(project);

            return CreatedAtAction(nameof(GetProjectByProjectNumber), new { projectNumber = project.ProjectNumber }, response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating project");
            return StatusCode(500, "An error occured while creating project");
        }
    }

    [HttpPut("{projectNumber}")]
    public async Task<IActionResult> UpdateProject(string projectNumber, [FromBody] UpdateProjectRequest request)
    {
        var validation = _validator.Validate(new ProjectUpdateValidator(), request);
        if (validation != null) return validation;
        try
        {
            var userOrganizationId = User.FindFirstValue("organizationId");
            if (string.IsNullOrEmpty(userOrganizationId))
            {
                Log.Warning("User has no organization ID in token");
                return StatusCode(403, "Access denied");
            }

            var existingProject = await _projectService.GetProjectByProjectNumber(projectNumber);
            if (existingProject == null)
            {
                Log.Warning("Project not found: {ProjectNumber}", projectNumber);
                return NotFound("Project not found");
            }

            // Ensure user can only update projects from their organization
            if (existingProject.OrganizationId != userOrganizationId && !User.IsInRole("Admin"))
            {
                Log.Warning("User attempted to update project from different organization. User org: {UserOrg}, Project org: {ProjectOrg}", 
                    userOrganizationId, existingProject.OrganizationId);
                return StatusCode(403, "Access denied - you can only update projects within your organization");
            }

            // Prevent users from changing organization ID (unless Admin)
            if (!string.IsNullOrEmpty(request.OrganizationId) && 
                request.OrganizationId != existingProject.OrganizationId && 
                !User.IsInRole("Admin"))
            {
                Log.Warning("User attempted to change project organization. User org: {UserOrg}, Current org: {CurrentOrg}, Requested org: {RequestedOrg}", 
                    userOrganizationId, existingProject.OrganizationId, request.OrganizationId);
                return StatusCode(403, "Access denied - you cannot change project organization");
            }

            request.ApplyTo(existingProject);

            var updatedProject = await _projectService.UpdateProject(existingProject.ProjectNumber, existingProject);
            if (updatedProject == null)
            {
                Log.Error("Failed to update project: {ProjectNumber}", projectNumber);
                return BadRequest("Failed to update project");
            }

            Log.Information("Project updated successfully: {ProjectNumber}", projectNumber);

            var response = ProjectResponse.FromProject(updatedProject);

            return Ok(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating project");
            return StatusCode(500, "An error occurred while updating project");
        }
    }

    [HttpDelete("{projectNumber}")]
    public async Task<IActionResult> DeleteProject(string projectNumber)
    {
        try
        {
            var userOrganizationId = User.FindFirstValue("organizationId");
            if (string.IsNullOrEmpty(userOrganizationId))
            {
                Log.Warning("User has no organization ID in token");
                return StatusCode(403, "Access denied");
            }

            var project = await _projectService.GetProjectByProjectNumber(projectNumber);
            if (project == null)
            {
                Log.Warning("Project not found for deletion");
                return NotFound("Project not found");
            }

            // Ensure user can only delete projects from their organization
            if (project.OrganizationId != userOrganizationId && !User.IsInRole("Admin"))
            {
                Log.Warning("User attempted to delete project from different organization. User org: {UserOrg}, Project org: {ProjectOrg}", 
                    userOrganizationId, project.OrganizationId);
                return StatusCode(403, "Access denied - you can only delete projects within your organization");
            }

            var success = await _projectService.RemoveProjectById(projectNumber);
            if (!success)
            {
                Log.Error("Failed to delete the project");
                return BadRequest("Failed to delete project");
            }

            Log.Information("Project deleted successfully");
            return NoContent();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting the project");
            return StatusCode(500, "An error occurred while deleting project");
        }
    }

}
