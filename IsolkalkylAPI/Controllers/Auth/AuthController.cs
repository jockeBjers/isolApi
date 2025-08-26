namespace IsolkalkylAPI.Controllers.Auth;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
[Route("api/auth")]
[ApiController]
public class AuthController(IAuthService authService, Validator validator, IConfiguration configuration) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly Validator _validator = validator;
    private readonly IConfiguration _configuration = configuration;

    [HttpPost("login")]
    public async Task<ActionResult> LogIn([FromBody] LoginRequest request)
    {

        var validationResult = _validator.Validate(new LoginValidator(), request);

        if (validationResult != null)
        {
            Log.Warning("Validation failed for login request");
            return validationResult;
        }
        try
        {
            // First check if user exists and is locked out
            var user = await _authService.GetUserByEmail(request.Email);
            if (user != null && user.LockoutUntil.HasValue && user.LockoutUntil.Value > DateTime.UtcNow)
            {
                var remainingLockoutTime = user.LockoutUntil.Value - DateTime.UtcNow;
                Log.Warning("Login attempt for locked out user {Email}. Locked out for {Minutes} more minutes.", request.Email, Math.Ceiling(remainingLockoutTime.TotalMinutes));
                return BadRequest($"Account is locked due to too many failed attempts. Try again in {Math.Ceiling(remainingLockoutTime.TotalMinutes)} minutes");
            }

            var result = await _authService.Login(request.Email, request.Password);
            if (result == null)
            {
                Log.Information("Invalid login attempt for {Email}", request.Email);
                return BadRequest("Invalid email or password");
            }

            // Generate JWT token
            var accessToken = _authService.CreateToken(result,
                _configuration["Jwt:Key"]!,
                _configuration["Jwt:Issuer"]!,
                _configuration["Jwt:Audience"]!
            );

            // generate a fresh refresh token on login
            var (newRefreshToken, plainToken) = _authService.GenerateRefreshToken();
            await _authService.SetUserRefreshTokenAsync(result.Id, newRefreshToken);

            var response = new LoginResponse(result.Name, accessToken, plainToken); 
            Log.Information("User {Name} logged in successfully", result.Name);
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
        var validationResult = _validator.Validate(new RegisterValidator(), request);

        if (validationResult != null)
        {
            Log.Warning("Validation failed for registration request");
            return validationResult;
        }

        var result = await _authService.Register(request.Name, request.Password, request.Email, request.PhoneNumber, request.OrganizationId);
        if (result == null)
        {
            Log.Warning("Registration failed: email {email} already exists.", request.Email);
            return BadRequest("Registration failed. Please check your details and try again.");
        }

        var response = new Response(result.Name);
        Log.Information("User {Name} registered successfully.", request.Name);
        return Created("User created", response);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult> Refresh([FromBody] RefreshRequest request)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
        {
            Log.Warning("Refresh token is missing");
            return BadRequest("Refresh token is required");
        }

        var result = await _authService.RefreshTokensAsync(
            request.RefreshToken,
            _configuration["Jwt:Key"]!,
            _configuration["Jwt:Issuer"]!,
            _configuration["Jwt:Audience"]!
        );

        if (result == null)
        {
            Log.Warning("Invalid refresh token: {RefreshToken}", request.RefreshToken);
            return BadRequest("Invalid refresh token");
        }

        Log.Information("Refreshed tokens successfully");
        return Ok(new RefreshResponse(result.Value.AccessToken, result.Value.NewRefreshToken));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout()
    {
        try
        {
            // Get user ID from JWT claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                Log.Warning("Logout attempt with invalid user ID claim");
                return BadRequest("Invalid user session");
            }

            // Revoke refresh token
            var success = await _authService.RevokeRefreshTokenAsync(userId);
            if (!success)
            {
                Log.Warning("Failed to revoke refresh token for user {UserId}", userId);
                return StatusCode(500, "Logout failed");
            }

            Log.Information("User {UserId} logged out successfully", userId);
            return Ok(new LogoutResponse("Logged out successfully"));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error during logout");
            return StatusCode(500, "An error occurred during logout");
        }
    }



}