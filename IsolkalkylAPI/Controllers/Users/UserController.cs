

using Microsoft.AspNetCore.Authorization;

namespace IsolkalkylAPI.Controllers.Users;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userService.GetAllUsers();
        var userDtos = users.Select(user => new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            OrganizationId = user.OrganizationId,
            Phone = user.Phone,
            Role = user.Role,
            OrganizationName = user.Organization?.Name
        }).ToList();
        return Ok(userDtos);
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
    {
        var user = await _userService.GetUserByEmail(email);
        if (user == null)
            return NotFound();

        var userDto = new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            OrganizationId = user.OrganizationId,
            Phone = user.Phone,
            Role = user.Role,
            OrganizationName = user.Organization?.Name
        };
        return Ok(userDto);
    }

    [HttpPost("create")]
    public async Task<IActionResult> AddUser([FromBody] UserDto userDto)
    {
        if (userDto == null)
            return BadRequest();

        var user = new User
        {
            Id = userDto.Id,
            Name = userDto.Name,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("GenerateSecurePassword()", 12),
            Email = userDto.Email,
            OrganizationId = userDto.OrganizationId,
            Phone = userDto.Phone,
            Role = userDto.Role,
        };
        await _userService.AddUser(user);
        return CreatedAtAction(nameof(GetUserByEmail), new { email = user.Email }, userDto);
    }
}
