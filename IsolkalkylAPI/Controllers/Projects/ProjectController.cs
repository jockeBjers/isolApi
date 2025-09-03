namespace IsolkalkylAPI.Controllers.Projects;

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
            var project = await _projectService.GetProjectByProjectNumber(projectNumber);
            if (project == null)
            {
                Log.Error("Project not found");
                return NotFound("Could not find project");
            }
            var projectResponse = new ProjectResponse(
                project.Id,
                project.ProjectNumber,
                project.Name,
                project.FromDate,
                project.ToDate,
                project.OrganizationId,
                project.Address,
                project.Customer,
                project.ContactPerson,
                project.ContactNumber,
                project.Comment
            );

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
            var existingProject = await _projectService.DoesProjectExist(request.ProjectNumber);
            if (existingProject)
            {
                Log.Warning("Project with this project number already exists");
                return Conflict("A project with this project number already exists!");
            }

            var project = new Project
            {
                ProjectNumber = request.ProjectNumber,
                Name = request.Name,
                OrganizationId = request.OrganizationId,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Address = request.Address,
                Customer = request.Customer,
                ContactPerson = request.ContactPerson,
                ContactNumber = request.ContactNumber,
                Comment = request.Comment
            };

            await _projectService.AddProject(project);
            Log.Information("Project created succesfully");

            var response = new ProjectResponse(
                project.Id,
                project.ProjectNumber,
                project.Name,
                project.FromDate,
                project.ToDate,
                project.OrganizationId,
                project.Address,
                project.Customer,
                project.ContactPerson,
                project.ContactNumber,
                project.Comment
            );

            return CreatedAtAction(nameof(GetProjectByProjectNumber), new { projectNumber = project.ProjectNumber }, response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating project");
            return StatusCode(500, "An error occured while creating project");
        }
    }

}
