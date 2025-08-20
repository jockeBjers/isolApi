namespace IsolCore.Services.AuthServices;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
}
