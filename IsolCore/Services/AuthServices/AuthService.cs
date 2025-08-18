namespace IsolCore.Services.AuthServices;

public class AuthService(IUserService userService) : IAuthService
{
    private readonly IUserService _userService = userService;

    public async Task<User?> Login(string email, string password)
    {
        var user = await _userService.GetUserByName(email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

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
} 
