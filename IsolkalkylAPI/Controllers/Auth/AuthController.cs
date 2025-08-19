
namespace IsolkalkylAPI.Controllers.Auth;

[Route("api/auth")]
[ApiController]
public class AuthController(IAuthService authService) : Controller
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    public async Task<ActionResult> LogIn([FromBody] LoginRequest request)
    {
        try
        {
            // First check if user exists and is locked out
            var user = await _authService.GetUserByEmail(request.Email);
            if (user != null && user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
            {
                var remainingLockoutTime = user.LockoutEnd.Value - DateTime.UtcNow;
                Log.Warning("Login attempt for locked out user {Email}. Locked out for {Minutes} more minutes.", request.Email, Math.Ceiling(remainingLockoutTime.TotalMinutes));
                return BadRequest($"Account is locked due to too many failed attempts. Try again in {Math.Ceiling(remainingLockoutTime.TotalMinutes)} minutes");
            }

            var result = await _authService.Login(request.Email, request.Password);
            if (result == null)
            {
                Log.Information("Invalid login attempt for {Email}", request.Email);
                return BadRequest("Invalid email or password");
            }

            var response = new Response(result.Name);
            return Ok(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Login error for {Email}", request.Email);
            return StatusCode(500, "An error occurred during login");
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult> Handle([FromBody] RegisterRequest request)
    {
        var result = await _authService.Register(request.Name, request.Password, request.Email, request.PhoneNumber, request.OrganizationId);
        if (result == null)
        {
            Log.Warning("Registration failed: Username {Name} already exists.", request.Name);
            return BadRequest("Username already exists");
        }

        var response = new Response(result.Name);
        Log.Information("User {Name} registered successfully.", request.Name);
        return Created("User created", response);
    }
}