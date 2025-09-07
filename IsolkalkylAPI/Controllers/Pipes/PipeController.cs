namespace IsolkalkylAPI.Controllers.Pipes;

[Route("api/pipes")]
[ApiController]
[Authorize]
public class PipeController(IPipeService pipeService, Validator validator) : ControllerBase
{

    private readonly IPipeService _pipeService = pipeService;
    private readonly Validator _validator = validator;

    [HttpGet("{pipeId}")]
    public async Task<IActionResult> GetPipeById(int pipeId)
    {
        try
        {
            var userOrganizationId = User.FindFirstValue("organizationId");
            if (string.IsNullOrEmpty(userOrganizationId))
            {
                Log.Warning("User has no organization ID in token");
                return StatusCode(403, "Access denied");
            }

            var currentPipe = await _pipeService.GetPipeById(pipeId);
            if (currentPipe == null)
            {
                Log.Error("Pipe not found");
                return NotFound("Could not find pipe");
            }

            if (currentPipe.Project == null)
            {
                Log.Error("The pipe doesnt belong to a project");
                return StatusCode(500, "Pipe is missing project reference");
            }

            if (currentPipe.Project.OrganizationId != userOrganizationId)
            {
                Log.Warning("User attempted to access pipe from different organization");
                return StatusCode(403, "Access denied");
            }
            var response = PipeResponse.FromInsulatedPipe(currentPipe);
            Log.Information("Pipe retrieved successfully");
            return Ok(response);

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving pipe");
            return StatusCode(500, "An unexpected error occurred");
        }
    }

}