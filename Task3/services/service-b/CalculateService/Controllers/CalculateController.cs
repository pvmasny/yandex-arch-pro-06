using Microsoft.AspNetCore.Mvc;

namespace CalculateService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalculateController : ControllerBase
    {
        private readonly ILogger<CalculateController> _logger;

        public CalculateController(ILogger<CalculateController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get(int id)
        {
            return Ok(new
            {
                OrderId = id,
                Value = id * 100,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
