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
);

public record UserListResponse(
    int Id,
    string Name,
    string Email,
    string Role,
    string? OrganizationName
);

// Request DTOs
public record CreateUserRequest(
    string Name,
    string Email,
    string OrganizationId,
    string? Phone,
    string Role,
    string? InitialPassword = null
);

public record UpdateUserRequest(
    string? Name,
    string? Email,
    string? Phone,
    string? Role
);

public record AdminUpdateUserRequest(
    string? Name,
    string? Email,
    string? Phone,
    string? Role,
    string? OrganizationId
);

public record PasswordResetRequest(string Email);

public record GetUserByEmailRequest(string Email);