namespace IsolkalkylAPI.Controllers.Auth;

public interface IAuthService
{
    Task<User?> Login(string email, string password);
    Task<User?> Register(string name, string password, string email, string? phoneNumber, string organizationId);
    Task<User?> GetUserByEmail(string email);
    string CreateToken(User user, string secretKey, string issuer, string audience);

    (RefreshToken HashedToken, string PlainToken) GenerateRefreshToken();
    Task<bool> SetUserRefreshTokenAsync(int userId, RefreshToken refreshToken);
    Task<User?> ValidateRefreshTokenAsync(string refreshToken);
    Task<bool> RevokeRefreshTokenAsync(int userId);
    Task<(string AccessToken, string NewRefreshToken)?> RefreshTokensAsync(string refreshToken, string secretKey, string issuer, string audience);
}
