namespace IsolkalkylAPI.Controllers.Users;

// Response DTOs
public record UserResponse(
    int Id,
    string Name,
    string Email,
    string OrganizationId,
    string? Phone,
    string Role,
    string? OrganizationName
)
{
    public static UserResponse FromUser(User user)
    {
        return new UserResponse(
            user.Id,
            user.Name,
            user.Email,
            user.OrganizationId,
            user.Phone,
            user.Role,
            user.Organization?.Name
        );
    }
}

public record UserListResponse(
    int Id,
    string Name,
    string Email,
    string Role,
    string? OrganizationName
)
{
    public static UserListResponse FromUser(User user)
    {
        return new UserListResponse(
            user.Id,
            user.Name,
            user.Email,
            user.Role,
            user.Organization?.Name
        );
    }
}

// Request DTOs
public record CreateUserRequest(
    string Name,
    string Email,
    string OrganizationId,
    string? Phone,
    string Role,
    string? Password = null
)
{
    public User ToUser()
    {
        return new User
        {
            Name = Name,
            Email = Email,
            OrganizationId = OrganizationId,
            Phone = Phone,
            Role = Role,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password, 12)
        };
    }
}

public record UpdateUserRequest(
    string? Name,
    string? Email,
    string? Phone,
    string? Role,
    string? OrganizationId
);

public static class UserRequestExtensions
{
    public static void ApplyTo(this UpdateUserRequest request, User existingUser, bool isAdmin = false)
    {
        if (!string.IsNullOrEmpty(request.Name))
            existingUser.Name = request.Name;
        if (!string.IsNullOrEmpty(request.Email))
            existingUser.Email = request.Email;
        if (!string.IsNullOrEmpty(request.Phone))
            existingUser.Phone = request.Phone;

        // Admin-only fields
        if (isAdmin)
        {
            if (!string.IsNullOrEmpty(request.Role))
                existingUser.Role = request.Role;
            if (!string.IsNullOrEmpty(request.OrganizationId))
                existingUser.OrganizationId = request.OrganizationId;
        }
    }
}

public record PasswordResetRequest(string Email);

public record GetUserByEmailRequest(string Email);