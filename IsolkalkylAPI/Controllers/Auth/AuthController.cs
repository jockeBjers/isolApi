using IsolCore.Services.AuthServices;
namespace IsolkalkylAPI.Controllers.Auth;

[Route("api/auth")]
[ApiController]
public class AuthController(IAuthService authService) : Controller
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    public async Task<ActionResult> LogIn([FromBody] LoginRequest request)
    {
        var result = await _authService.Login(request.Email, request.Password);
        if (result == null)
            return NotFound("Invalid email or password");

        var response = new Response(result.Name);
        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<ActionResult> Handle([FromBody] RegisterRequest request)
    {
        var result = await _authService.Register(request.Name, request.Password, request.Email, request.PhoneNumber, request.OrganizationId);
        if (result == null)
            return BadRequest("Username already exists");

        var response = new Response(result.Name);
        return Created("User created", response);
    }
}