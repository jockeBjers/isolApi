namespace IsolCore.Services.AuthServices;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Serilog;

public class AuthService(IUserService userService) : IAuthService
{
    private readonly IUserService _userService = userService;

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _userService.GetUserByEmail(email);
    }

    public async Task<User?> Login(string email, string password)
    {

        var user = await _userService.GetUserByEmail(email);
        if (user == null)
        {
            Log.Warning("Login failed: User with email {Email} not found", email);
            return null;
        }

        // Check lockout
        if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
        {
            Log.Warning("Login failed: User {Email} is locked out until {LockoutEnd}", email, user.LockoutEnd);
            return null;
        }

        bool passwordVerified = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        Log.Debug("Password verification for {Email}: {PasswordVerified}", email, passwordVerified);

        if (!passwordVerified)
        {
            user.FailedLoginAttempts++;
            Log.Information("Failed login attempts for {Email}: {FailedLoginAttempts}", email, user.FailedLoginAttempts);

            if (user.FailedLoginAttempts >= 5)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                Log.Warning("User {Email} locked out until {LockoutEnd}", email, user.LockoutEnd);
            }

            await _userService.UpdateUser(user.Id, user);
            return null;
        }

        // Reset on success
        Log.Information("Login successful for {Email}", email);
        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;
        await _userService.UpdateUser(user.Id, user);

        return user;
    }

    public async Task<User?> Register(string name, string password, string email, string? phoneNumber, string organizationId)
    {
        if (await _userService.DoesUserExist(name))
            return null;

        var user = new User()
        {
            Name = name,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12),
            OrganizationId = organizationId,
            Email = email,
            Phone = phoneNumber
        };

        await _userService.AddUser(user);
        return user;
    }
    public string CreateToken(User user, string secretKey, string issuer, string audience)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role),
            new("organizationId", user.OrganizationId)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public RefreshToken GenerateRefreshToken()
    {
        // Generate rndom token
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomBytes),
            Expires = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow,
            IsRevoked = false
        };
    }

    public async Task<bool> SetUserRefreshTokenAsync(int userId, RefreshToken refreshToken)
    {
        try
        {
            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                Log.Warning("Failed to set refresh token: User {UserId} not found", userId);
                return false;
            }

            user.RefreshToken = refreshToken;
            await _userService.UpdateUser(user.Id, user);
            Log.Debug("Refresh token set for user {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error setting refresh token for user {UserId}", userId);
            return false;
        }
    }

    public async Task<User?> ValidateRefreshTokenAsync(string refreshToken)
    {
        var user = await _userService.GetUserByRefreshToken(refreshToken);
        if (user == null)
        {
            Log.Warning("Refresh token validation failed: token not found");
            return null;
        }

        // Check if token is expired or revoked
        if (user.RefreshToken?.Expires < DateTime.UtcNow ||
            user.RefreshToken?.IsRevoked == true)
        {
            Log.Warning("Refresh token validation failed: token expired or revoked for user {UserId}", user.Id);
            return null;
        }

        Log.Debug("Refresh token validated successfully for user {UserId}", user.Id);
        return user;
    }

    public async Task<bool> RevokeRefreshTokenAsync(int userId)
    {
        try
        {
            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                Log.Warning("Failed to revoke refresh token: User {UserId} not found", userId);
                return false;
            }

            if (user.RefreshToken != null)
            {
                user.RefreshToken.IsRevoked = true;
                await _userService.UpdateUser(user.Id, user);
                Log.Information("Refresh token revoked for user {UserId}", userId);
            }

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error revoking refresh token for user {UserId}", userId);
            return false;
        }
    }

    public async Task<(string AccessToken, RefreshToken NewRefreshToken)?> RefreshTokensAsync(string refreshToken, string secretKey, string issuer, string audience)
    {
        try
        {
            var user = await _userService.GetUserByRefreshToken(refreshToken);
            if (user == null)
            {
                Log.Warning("Refresh failed: token not found");
                return null;
            }

            // Validate token without revoking it yet
            if (user.RefreshToken?.Expires < DateTime.UtcNow ||
                user.RefreshToken?.IsRevoked == true)
            {
                Log.Warning("Refresh failed: token expired or revoked for user {UserId}", user.Id);
                return null;
            }

            // Generate new tokens
            var accessToken = CreateToken(user, secretKey, issuer, audience);
            var newRefreshToken = GenerateRefreshToken();

            // Revoke old token and set new one
            if (user.RefreshToken == null)
            {
                Log.Warning("No existing refresh token found for user {UserId}", user.Id);
                return null;
            }
            user.RefreshToken.IsRevoked = true;
            var tempOldToken = user.RefreshToken;
            user.RefreshToken = newRefreshToken;

            try
            {
                await _userService.UpdateUser(user.Id, user);
                Log.Information("Tokens refreshed successfully for user {UserId}", user.Id);
                return (accessToken, newRefreshToken);
            }
            catch
            {
                user.RefreshToken = tempOldToken;
                user.RefreshToken.IsRevoked = false;
                throw;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error refreshing tokens");
            return null;
        }
    }
}
