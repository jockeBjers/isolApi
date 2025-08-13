using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IsolCore.Data;
using IsolCore.Services.UserServices;

namespace IsolkalkylAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly Database _db;
        
        public TestController(IUserService userService, Database db)
        {
            _userService = userService;
            _db = db;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("users/{id}")] 
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        } 
    }
}