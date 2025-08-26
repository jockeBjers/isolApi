

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Serilog;
namespace IsolkalkylAPI.Controllers.Users;

[Route("api/user")]
[ApiController]
[Authorize]
public class UserController(IUserService userService, Validator validator) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly Validator _validator = validator;

    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsers()
    {
        try
        {

            var users = await _userService.GetAllUsers();
            var response = users.Select(user => new UserListResponse(
                    user.Id,
                    user.Name,
                    user.Email,
                    user.Role,
                    user.Organization?.Name
                )).ToList();
            Log.Information("Retrieved {Count} users for admin", response.Count);
            return Ok(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting all users");
            return StatusCode(500, "An error occurred while retrieving users");
        }
    }


    [HttpGet("user")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
    {
        var emailRequest = new GetUserByEmailRequest(email);
        var validation = _validator.Validate(new GetUserByEmailValidator(), emailRequest);
        if (validation != null)
            return validation;

        var user = await _userService.GetUserByEmail(email);
        if (user == null)
            return NotFound($"User with email {email} not found");

        var userDto = new UserResponse(
            user.Id,
            user.Name,
            user.Email,
            user.OrganizationId,
            user.Phone,
            user.Role,
            user.Organization?.Name
        );
        return Ok(userDto);
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetCurrentUserProfile()
    {
        try
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
                return BadRequest("Invalid user session");

            var user = await _userService.GetUserByEmail(userEmail);
            if (user == null)
                return NotFound("User not found");

            var response = new UserResponse(
                user.Id,
                user.Name,
                user.Email,
                user.OrganizationId,
                user.Phone,
                user.Role,
                user.Organization?.Name
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting current user profile");
            return StatusCode(500, "An error occurred while retrieving user profile");
        }
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {

        var validation = _validator.Validate(new UserValidator(), request);
        if (validation != null)
            return validation;
        try
        {
            var existingUser = await _userService.GetUserByEmail(request.Email);
            if (existingUser != null)
                return Conflict("User with this email already exists");

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, 12),
                OrganizationId = request.OrganizationId,
                Phone = request.Phone,
                Role = request.Role
            };

            await _userService.AddUser(user);
            
            Log.Information("User created successfully: {Email}", request.Email);

            var response = new UserResponse(
                user.Id,
                user.Name,
                user.Email,
                user.OrganizationId,
                user.Phone,
                user.Role,
                null
            );

            return CreatedAtAction(nameof(GetUserByEmail), new { email = user.Email }, response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating user with email {Email}", request.Email);
            return StatusCode(500, "An error occurred while creating user");
        }
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateCurrentUserProfile([FromBody] UpdateUserRequest request)
    {
        var validation = _validator.Validate(new UserUpdateValidator(), request);
        if (validation != null)
            return validation;

        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return BadRequest("Invalid user session");

            var existingUser = await _userService.GetUserById(userId);
            if (existingUser == null)
                return NotFound("User not found");

            // Check email uniqueness if email is being updated
            if (!string.IsNullOrEmpty(request.Email) && request.Email != existingUser.Email)
            {
                var emailExists = await _userService.GetUserByEmail(request.Email);
                if (emailExists != null)
                    return Conflict("Email already exists");
            }

            if (!string.IsNullOrEmpty(request.Name))
                existingUser.Name = request.Name;
            if (!string.IsNullOrEmpty(request.Email))
                existingUser.Email = request.Email;
            if (!string.IsNullOrEmpty(request.Phone))
                existingUser.Phone = request.Phone;

            var updatedUser = await _userService.UpdateUser(userId, existingUser);
            if (updatedUser == null)
                return BadRequest("Failed to update user");

            var response = new UserResponse(
                updatedUser.Id,
                updatedUser.Name,
                updatedUser.Email,
                updatedUser.OrganizationId,
                updatedUser.Phone,
                updatedUser.Role,
                updatedUser.Organization?.Name
            );

            Log.Information("User profile updated successfully: {UserId}", userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating user profile for user {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return StatusCode(500, "An error occurred while updating profile");
        }
    }

    [HttpPut("admin/user/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUserByAdmin(int userId, [FromBody] AdminUpdateUserRequest request)
    {
        var validation = _validator.Validate(new AdminUserUpdateValidator(), request);
        if (validation != null)
            return validation;

        try
        {
            var existingUser = await _userService.GetUserById(userId);
            if (existingUser == null)
                return NotFound("User not found");

            // Check email uniqueness if email is being updated
            if (!string.IsNullOrEmpty(request.Email) && request.Email != existingUser.Email)
            {
                var emailExists = await _userService.GetUserByEmail(request.Email);
                if (emailExists != null)
                    return Conflict("Email already exists");
            }

            var updatedUser = new User
            {
                Id = existingUser.Id,
                Name = request.Name ?? existingUser.Name,
                Email = request.Email ?? existingUser.Email,
                Phone = request.Phone ?? existingUser.Phone,
                OrganizationId = request.OrganizationId ?? existingUser.OrganizationId,
                Role = request.Role ?? existingUser.Role,
                PasswordHash = existingUser.PasswordHash,
                FailedLoginAttempts = existingUser.FailedLoginAttempts,
                LockoutUntil = existingUser.LockoutUntil
            };

            var result = await _userService.UpdateUserAdmin(userId, updatedUser);
            if (result == null)
                return BadRequest("Failed to update user");

            var response = new UserResponse(
                result.Id,
                result.Name,
                result.Email,
                result.OrganizationId,
                result.Phone,
                result.Role,
                result.Organization?.Name
            );

            Log.Information("Admin updated user {UserId} successfully", userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating user {UserId} by admin", userId);
            return StatusCode(500, "An error occurred while updating user");
        }
    }

    [HttpDelete("admin/user/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        try
        {
            var user = await _userService.GetUserById(userId);
            if (user == null)
                return NotFound("User not found");

            var success = await _userService.RemoveUserById(userId);
            if (!success)
                return BadRequest("Failed to delete user");

            Log.Information("Admin deleted user {UserId} with email {Email}", userId, user.Email);
            return NoContent();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting user {UserId}", userId);
            return StatusCode(500, "An error occurred while deleting user");
        }
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequest request)
    {
        var validation = _validator.Validate(new PasswordResetValidator(), request);
        if (validation != null)
            return validation;

        try
        {
            var user = await _userService.GetUserByEmail(request.Email);
            if (user == null)
            {
                Log.Warning("Password reset requested for non-existent email: {Email}", request.Email);
                return Ok("If the email exists, a password reset link has been sent");
            }

            // Generate temporary password
            var tempPassword = Guid.NewGuid().ToString().Substring(0, 12);
            var success = await _userService.UpdateUserPassword(user.Id, tempPassword);

            if (success)
            {
                // Todo: send an actual mail in prod
                Log.Information("Password reset for user {Email}. Temporary password: {TempPassword}", request.Email, tempPassword);
                return Ok($"Password reset successful. Temporary password: {tempPassword}");
            }

            return StatusCode(500, "Failed to reset password");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error resetting password for {Email}", request.Email);
            return StatusCode(500, "An error occurred while resetting password");
        }
    }
}
