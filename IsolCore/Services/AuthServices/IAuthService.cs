namespace IsolCore.Services.AuthServices;

public interface IAuthService
{
    Task<User?> Login(string email, string password);
    Task<User?> Register(string name, string password, string email, string? phoneNumber, string organizationId);
    Task<User?> GetUserByEmail(string email);
    string CreateToken(User user, string secretKey, string issuer, string audience);

    RefreshToken GenerateRefreshToken();
    Task<bool> SetUserRefreshTokenAsync(int userId, RefreshToken refreshToken);
    Task<User?> ValidateRefreshTokenAsync(string refreshToken);
    Task<bool> RevokeRefreshTokenAsync(int userId);
    Task<(string AccessToken, RefreshToken NewRefreshToken)?> RefreshTokensAsync(string refreshToken, string secretKey, string issuer, string audience);
}
