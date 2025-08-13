using Microsoft.AspNetCore.Mvc;

namespace IsolkalkylAPI
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("API is working!");
        }
    }
}