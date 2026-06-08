using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PRReviewAgent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("PR Review Agent Running");
        }
    }
}
