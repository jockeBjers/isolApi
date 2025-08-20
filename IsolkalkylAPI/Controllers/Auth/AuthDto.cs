namespace IsolkalkylAPI.Controllers.Auth;

public record RegisterRequest(string Name, string Password, string OrganizationId, string Email, string? PhoneNumber);
public record Response(string Name);
public record LoginRequest(string Email, string Password);
public record LoginResponse(string Name, string AccessToken);