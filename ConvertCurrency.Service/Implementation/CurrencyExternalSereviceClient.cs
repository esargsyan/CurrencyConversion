using ConvertCurrency.Domain.Models;
using ConvertCurrency.Service.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ConvertCurrency.Service.Implementation
{
    public class CurrencyExternalSereviceClient : ICurrencyExternalSereviceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CurrencyExternalSereviceClient> _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy;

        public CurrencyExternalSereviceClient(HttpClient httpClient, ILogger<CurrencyExternalSereviceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            // Define an exponential backoff retry policy (max 3 attempts)
            _retryPolicy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode) // Retry only on non-success responses
                .WaitAndRetryAsync(retryCount: 3, sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), 
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        _logger.LogInformation($"Retry {retryAttempt} after {timespan.TotalSeconds} seconds due to {outcome.Result?.StatusCode}");
                    });

            // Create a circuit breaker policy that trips after 2 consecutive failures within 30 seconds
            _circuitBreakerPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode) // Break on non-success responses
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 2, // Allow 2 failures before opening
                    durationOfBreak: TimeSpan.FromSeconds(30), // Open state duration
                    onBreak: (outcome, breakDuration) =>
                    {
                        _logger.LogInformation($"[Circuit Breaker] Open: HTTP {(int)outcome.Result.StatusCode}. Circuit will remain open for {breakDuration.TotalSeconds} seconds.");
                    },
                    onReset: () =>
                    {
                        _logger.LogInformation("[Circuit Breaker] Reset: Circuit is now closed.");
                    },
                    onHalfOpen: () =>
                    {
                        _logger.LogInformation("[Circuit Breaker] Half-Open: Testing API availability.");
                    });
        }

        public async Task<CurrencyRateData> GetAllCurrencyRates(string currency)
        {
            try
            {
                string requestUrl = $"/api/ExternalService/GetAllCurrencyRates?currency={currency}";
                _logger.LogInformation("Sending GET request to {Url}", requestUrl);

                var response = await _circuitBreakerPolicy.ExecuteAsync(() => _retryPolicy.ExecuteAsync(() => _httpClient.GetAsync(requestUrl)));

                response.EnsureSuccessStatusCode(); // Throws 

                var currencyRates = await response.Content.ReadFromJsonAsync<CurrencyRateData>();

                if (currencyRates == null)
                {
                    throw new Exception("Invalid currency rate data received.");
                }

                _logger.LogInformation("Successfully retrieved currency rates for {Currency}", currency);
                return currencyRates;
            }
            catch (BrokenCircuitException)
            {
                _logger.LogError("[Circuit Breaker] Request blocked because the circuit is open.");
                throw new Exception("Service temporarily unavailable due to repeated failures.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("HTTP error: {Message}", ex.Message);
                throw new Exception("Failed to retrieve currency rates.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<CurrencyResponseData> ConvertCurrency(ConvertCurrencyData convertCurrencyData)
        {
            try
            {
                _logger.LogInformation("Sending POST request to convert currency: {From} to {To}, Amount: {Amount}", convertCurrencyData.CurrencyFrom, convertCurrencyData.CurrencyTo, convertCurrencyData.Amount);

                var response = await _circuitBreakerPolicy.ExecuteAsync(() =>
                    _retryPolicy.ExecuteAsync(() => _httpClient.PostAsJsonAsync("/api/ExternalService/ConvertCurrency", convertCurrencyData)));

                response.EnsureSuccessStatusCode(); // Throws for non-2xx responses

                var result = await response.Content.ReadFromJsonAsync<CurrencyResponseData>();

                if (result == null)
                {
                    throw new Exception("Invalid currency conversion data received.");
                }

                _logger.LogInformation("Successfully converted {From} {Amount} to {To}, Result: {ConvertedAmount}", convertCurrencyData.CurrencyFrom, convertCurrencyData.Amount, convertCurrencyData.CurrencyTo, result.ConvertedAmount);

                return result;
            }
            catch (BrokenCircuitException)
            {
                _logger.LogError("[Circuit Breaker] Request blocked because the circuit is open.");
                throw new Exception("Service temporarily unavailable due to repeated failures.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("HTTP error: {Message}", ex.Message);
                throw new Exception("Failed to convert currency.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<CurrencyRateHistoryData> GetCurrencyRateHistory(CurrencyRateDurationData currencyRateDurationData)
        {
            try
            {
                _logger.LogInformation("Fetching currency rate history for {PageNumber}, from {StartDate} to {EndDate}", currencyRateDurationData.PageNumber, currencyRateDurationData.StartDate, currencyRateDurationData.EndDate);

                var response = await _circuitBreakerPolicy.ExecuteAsync(() =>
                    _retryPolicy.ExecuteAsync(() => _httpClient.PostAsJsonAsync("/api/ExternalService/GetCurrencyRateHistory", currencyRateDurationData)));

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<CurrencyRateHistoryData>();

                if (result == null)
                {
                    throw new Exception("Invalid currency rate history data received.");
                }

                _logger.LogInformation("Successfully retrieved currency rate history for EUR currency");
                return result;
            }
            catch (BrokenCircuitException)
            {
                _logger.LogError("[Circuit Breaker] Request blocked because the circuit is open.");
                throw new Exception("Service temporarily unavailable due to repeated failures.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("HTTP error: {Message}", ex.Message);
                throw new Exception("Failed to retrieve currency rate history.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error: {Message}", ex.Message);
                throw;
            }
        }
    }
}
