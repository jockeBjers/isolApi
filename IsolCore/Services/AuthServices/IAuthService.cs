namespace IsolCore.Services.AuthServices;

public interface IAuthService
{
    Task<User?> Login(string email, string password);
    Task<User?> Register(string name, string password, string email, string? phoneNumber, string organizationId);
    Task<User?> GetUserByEmail(string email);
    string CreateToken(User user, string secretKey, string issuer, string audience);
}
