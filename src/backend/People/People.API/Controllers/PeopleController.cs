using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace People.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PeopleController : ControllerBase
    {
        [Authorize(Roles = "Manager")]
        [HttpGet("test")]
        public IActionResult PeopleSecure()
        {
            var username = User.Identity?.Name;
            return Ok($"People data secured for {username}");
        }
    }
}
