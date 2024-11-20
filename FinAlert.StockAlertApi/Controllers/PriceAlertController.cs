using System.Security.Claims;
using FinAlert.AlertStore.Core.Contracts;
using FinAlert.AlertStore.Core.Domain;
using FinAlert.StockAlertApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinAlert.StockAlertApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class StockPriceController : ControllerBase
    {
        private readonly ILogger<StockPriceController> _logger;
        private readonly IPriceAlertService _priceAlertService;

        public StockPriceController(ILogger<StockPriceController> logger, IPriceAlertService priceAlertService)
        {
            _logger = logger;
            _priceAlertService = priceAlertService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAlerts()
        {
            Guid? userId = getHttpContextUserId(out string errorMessage);
            if (userId is null)
                return Unauthorized(errorMessage);

            try
            {
                var alerts = await _priceAlertService.GetAlertsAsync(userId.Value);

                return Ok(alerts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting alerts");

                return StatusCode(500, "Error getting alerts");
            }
        }

        [HttpGet("{alertId}")]
        public async Task<IActionResult> GetAlertById(Guid alertId)
        {
            Guid? userId = getHttpContextUserId(out string errorMessage);
            if (userId is null)
                return Unauthorized(errorMessage);

            try
            {
                var alert = await _priceAlertService.GetAlertAsync(userId.Value, alertId);
                if (alert is null)
                    return NotFound();

                return Ok(alert);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting alert");

                return StatusCode(500, "Error getting alert");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAlert([FromBody] CreatePriceAlertRequest alert)
        {
            Guid? userId = getHttpContextUserId(out string errorMessage);
            if (userId is null)
                return Unauthorized(errorMessage);

            try
            {
                await _priceAlertService.CreateAlertAsync(new PriceAlert
                {
                    UserId = userId.Value,
                    Symbol = alert.Symbol,
                    Threshold = alert.Threshold,
                    AlertType = alert.AlertType,
                    TriggerType = alert.TriggerType,
                    Enabled = alert.Enabled,
                    Triggered = false
                });

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating alert");

                return StatusCode(500, "Error creating alert");
            }
        }

        [HttpPut("{alertId}")]
        public async Task<IActionResult> UpdateAlert(Guid alertId, [FromBody] UpdatePriceAlertRequest alert)
        {
            Guid? userId = getHttpContextUserId(out string errorMessage);
            if (userId is null)
                return Unauthorized(errorMessage);

            try
            {
                await _priceAlertService.UpdateAlertAsync(userId.Value, alertId, alert.AlertType, alert.TriggerType, alert.Threshold);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating alert");

                return StatusCode(500, "Error updating alert");
            }
        }

        [HttpDelete("{alertId}")]
        public async Task<IActionResult> DeleteAlert(Guid alertId)
        {
            Guid? userId = getHttpContextUserId(out string errorMessage);
            if (userId is null)
                return Unauthorized(errorMessage);


            try
            {
                await _priceAlertService.DeleteAlertAsync(userId.Value, alertId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting alert");

                return StatusCode(500, "Error deleting alert");
            }
        }

        #region Private Methods

        private Guid? getHttpContextUserId(out string errorMessage)
        {
            var userIdentity = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdentity))
            {
                errorMessage = "User not authenticated";

                return null;
            }

            if (!Guid.TryParse(userIdentity, out Guid userId))
            {
                errorMessage = "Invalid authenticated user id";

                return null;
            }

            errorMessage = string.Empty;
            return userId;
        }

        #endregion
    }
}
