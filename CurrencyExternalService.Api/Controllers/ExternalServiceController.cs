using ConvertCurrency.Domain.Models;
using CurrencyExternal.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CurrencyExternalService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalServiceController : ControllerBase
    {
        private readonly ICurrencyExternalService _currencyExternalService;
        private readonly AppSettings _appSettings;
      
        public ExternalServiceController(ICurrencyExternalService currencyExternalService, IOptions<AppSettings> settings)
        {
            _currencyExternalService = currencyExternalService;
            _appSettings = settings.Value;
        }

        [HttpGet("GetAllCurrencyRates")]
        public async Task<ActionResult<CurrencyRateData>> GetAllCurrencyRates(string? currency)
        {
            try
            {
                var currencyRates = await _currencyExternalService.GetAllCurrencyRates(_appSettings.BaseApiUrl, currency);
                return Ok(currencyRates);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"{ex.Message} An internal server error occurred. Please try again later." });
            }
        }

        [HttpPost("ConvertCurrency")]
        public async Task<ActionResult<CurrencyResponseData>> ConvertCurrency(ConvertCurrencyData convertCurrencyData)
        {
            try
            {
                var convertCurrencyResData = await _currencyExternalService.ConvertCurrency(convertCurrencyData, _appSettings.BaseApiUrl);
                return Ok(convertCurrencyResData);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"{ex.Message} An internal server error occurred. Please try again later." });
            }
        }

        [HttpPost("GetCurrencyRateHistory")]
        public async Task<ActionResult<CurrencyRateHistoryData>> GetCurrencyRateHistory(CurrencyRateDurationData currencyRateDurationData)
        {
            try
            {
                var currencyRateHistoryData = await _currencyExternalService.GetCurrencyRateHistory(currencyRateDurationData, _appSettings.BaseApiUrl);
                return Ok(currencyRateHistoryData);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"{ex.Message} An internal server error occurred. Please try again later." });
            }
        }
    }
}
