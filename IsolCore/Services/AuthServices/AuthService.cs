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
        var lockoutTimer = 5;
        var maxFailedAttempts = 5;
        var user = await _userService.GetUserByEmail(email);
        if (user == null)
        {
            Log.Warning("Login failed: User with email {Email} not found", email);
            return null;
        }

        // Check if user is currently locked out
        if (user.LockoutUntil.HasValue && user.LockoutUntil.Value > DateTime.UtcNow)
        {
            Log.Warning("Login failed: User {Email} is locked out until {LockoutEnd}", email, user.LockoutUntil);
            return null;
        }

        // If lockout period has expired, reset attempts and lockout
        if (user.LockoutUntil.HasValue && user.LockoutUntil.Value <= DateTime.UtcNow)
        {
            user.FailedLoginAttempts = 0;
            user.LockoutUntil = null;
            Log.Information("Lockout period expired for {Email}, resetting failed login attempts", email);
        }

        // verify password
        bool passwordVerified = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        Log.Debug("Password verification for {Email}: {PasswordVerified}", email, passwordVerified);

        // if password is incorrect, increment failed attempts
        if (!passwordVerified)
        {
            user.FailedLoginAttempts++;
            Log.Information("Failed login attempts for {Email}: {FailedLoginAttempts}", email, user.FailedLoginAttempts);

            // If failed attempts reach threshold, set lockout
            if (user.FailedLoginAttempts >= maxFailedAttempts)
            {
                user.LockoutUntil = DateTime.UtcNow.AddMinutes(lockoutTimer);
                Log.Warning("User {Email} locked out until {LockoutEnd}", email, user.LockoutUntil);
            }

            await _userService.UpdateUser(user.Id, user);
            return null;
        }

        // Reset on success
        Log.Information("Login successful for {Email}", email);
        user.FailedLoginAttempts = 0;
        user.LockoutUntil = null;
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

    public (RefreshToken HashedToken, string PlainToken) GenerateRefreshToken()
    {
        // Generate random token
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        var plainToken = Convert.ToBase64String(randomBytes);
        var hashedToken = BCrypt.Net.BCrypt.HashPassword(plainToken, workFactor: 12);

        var refreshToken = new RefreshToken
        {
            Token = hashedToken,
            Expires = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow,
            IsRevoked = false
        };

        return (refreshToken, plainToken);
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
        // Get all users with valid refresh tokens
        var users = await _userService.GetUsersWithValidRefreshTokens();

        foreach (var user in users)
        {
            if (user.RefreshToken != null &&
                !user.RefreshToken.IsRevoked &&
                user.RefreshToken.Expires > DateTime.UtcNow &&
                BCrypt.Net.BCrypt.Verify(refreshToken, user.RefreshToken.Token)) // verify hash
            {
                Log.Debug("Refresh token validated successfully for user {UserId}", user.Id);
                return user;
            }
        }

        Log.Warning("Refresh token validation failed: token not found or invalid");
        return null;
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

    public async Task<(string AccessToken, string NewRefreshToken)?> RefreshTokensAsync(string refreshToken, string secretKey, string issuer, string audience)
    {
        try
        {
            var user = await ValidateRefreshTokenAsync(refreshToken);
            if (user == null)
            {
                Log.Warning("Refresh failed: token validation failed");
                return null;
            }

            // Generate new tokens
            var accessToken = CreateToken(user, secretKey, issuer, audience);
            var (newRefreshToken, plainToken) = GenerateRefreshToken();

            // Revoke old token and set new one
            if (user.RefreshToken == null)
            {
                Log.Warning("No existing refresh token found for user {UserId}", user.Id);
                return null;
            }

            user.RefreshToken.IsRevoked = true;
            user.RefreshToken = newRefreshToken;

            await _userService.UpdateUser(user.Id, user);
            Log.Information("Tokens refreshed successfully for user {UserId}", user.Id);

            return (accessToken, plainToken);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error refreshing tokens");
            return null;
        }
    }
}
