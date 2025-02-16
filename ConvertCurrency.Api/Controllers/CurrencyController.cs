using ConvertCurrency.Domain.Models;
using ConvertCurrency.Service.Implementation;
using ConvertCurrency.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace ConvertCurrency.Api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;
        private readonly ILogger<CurrencyController> _logger;
        public CurrencyController(ICurrencyService currencyService, ILogger<CurrencyController> logger)
        {
            _currencyService = currencyService;
            _logger = logger;
        }

        // GET: api/<CurrencyController>
        [HttpGet("GetAllCurrencyRates")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<CurrencyRateData>> GetAllCurrencyRates([FromQuery] string? currency)
        {
            var stopwatch = Stopwatch.StartNew(); // Track response time

            string clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
            string clientId = User.Claims.FirstOrDefault(c => c.Type == "ClientId")?.Value ?? "Unknown Client";
            string httpMethod = HttpContext.Request.Method;

            _logger.LogInformation("Received {HttpMethod} request from ClientId: {ClientId}, IP: {ClientIp}, Endpoint: {Endpoint}", httpMethod, clientId, clientIp, HttpContext.Request.Path);

            try
            {
                CurrencyRateData currencyRates;
                if (!string.IsNullOrEmpty(currency))
                {
                    currencyRates = await _currencyService.GetAllCurrencyByCode(currency);
                }
                else
                {
                    currencyRates = await _currencyService.GetAllCurrencyRates();
                }

                stopwatch.Stop();
                _logger.LogInformation("Successfully processed request for ClientId: {ClientId}, ResponseTime: {ResponseTime}ms, Status: {StatusCode}", clientId, stopwatch.ElapsedMilliseconds, StatusCodes.Status200OK);

                return Ok(currencyRates);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error processing request for ClientId: {ClientId}, Status: {StatusCode}, ResponseTime: {ResponseTime}ms", clientId, StatusCodes.Status500InternalServerError, stopwatch.ElapsedMilliseconds);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost("ConvertCurrency")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<CurrencyResponseData>> ConvertCurrency([FromBody] ConvertCurrencyData convertCurrencyData)
        {
            var stopwatch = Stopwatch.StartNew(); // Track response time

            string clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
            string clientId = User.Claims.FirstOrDefault(c => c.Type == "ClientId")?.Value ?? "Unknown Client";
            string httpMethod = HttpContext.Request.Method;

            _logger.LogInformation("Received {HttpMethod} request from ClientId: {ClientId}, IP: {ClientIp}, Endpoint: {Endpoint}", httpMethod, clientId, clientIp, HttpContext.Request.Path);

            try
            {
                var convertCurrencyResData = await _currencyService.ConvertCurrency(convertCurrencyData);

                stopwatch.Stop();
                _logger.LogInformation("Successfully processed request for ClientId: {ClientId}, ResponseTime: {ResponseTime}ms, Status: {StatusCode}", clientId, stopwatch.ElapsedMilliseconds, StatusCodes.Status200OK);

                return Ok(convertCurrencyResData);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error processing request for ClientId: {ClientId}, Status: {StatusCode}, ResponseTime: {ResponseTime}ms", clientId, StatusCodes.Status500InternalServerError, stopwatch.ElapsedMilliseconds);

                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost("GetCurrencyRateHistory")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<CurrencyRateHistoryData>> GetCurrencyRateHistory([FromBody] CurrencyRateDurationData currencyRateDurationData)
        {
            var stopwatch = Stopwatch.StartNew(); // Track response time

            string clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
            string clientId = User.Claims.FirstOrDefault(c => c.Type == "ClientId")?.Value ?? "Unknown Client";
            string httpMethod = HttpContext.Request.Method;

            _logger.LogInformation("Received {HttpMethod} request from ClientId: {ClientId}, IP: {ClientIp}, Endpoint: {Endpoint}", httpMethod, clientId, clientIp, HttpContext.Request.Path);

            try
            {
                var currencyRateHistoryData = await _currencyService.GetCurrencyRateHistory(currencyRateDurationData);

                stopwatch.Stop();
                _logger.LogInformation("Successfully processed request for ClientId: {ClientId}, ResponseTime: {ResponseTime}ms, Status: {StatusCode}", clientId, stopwatch.ElapsedMilliseconds, StatusCodes.Status200OK);

                return Ok(currencyRateHistoryData);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error processing request for ClientId: {ClientId}, Status: {StatusCode}, ResponseTime: {ResponseTime}ms", clientId, StatusCodes.Status500InternalServerError, stopwatch.ElapsedMilliseconds);

                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}
