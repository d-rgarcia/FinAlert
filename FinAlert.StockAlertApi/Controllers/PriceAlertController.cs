using FinAlert.AlertStore.Core.Domain;
using Microsoft.AspNetCore.Mvc;

namespace FinAlert.StockAlertApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockPriceController : ControllerBase
    {
        private readonly ILogger<StockPriceController> _logger;

        public StockPriceController(ILogger<StockPriceController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetAllAlerts()
        {
            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult GetAlertById(int id)
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult CreateAlert([FromBody] PriceAlert alert)
        {
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateAlert(int id, [FromBody] PriceAlert alert)
        {
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAlert(int id)
        {
            return Ok();
        }        
    }
}
