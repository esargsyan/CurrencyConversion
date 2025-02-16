using ConvertCurrency.Domain;
using ConvertCurrency.Domain.Models;
using ConvertCurrency.Service.Implementation;
using ConvertCurrency.Service.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertCurrency.UnitTests.CurrencyTest
{
    [TestClass]
    public class CurrencyServiceTests
    {
        private readonly Mock<ICurrencyExternalSereviceClient> _mockExternalClient;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly CurrencyService _currencyService;

        public CurrencyServiceTests()
        {
            _mockExternalClient = new Mock<ICurrencyExternalSereviceClient>();
            _mockCacheService = new Mock<ICacheService>();
            _currencyService = new CurrencyService(_mockExternalClient.Object, _mockCacheService.Object);
        }

        // Test: GetAllCurrencyRates() when cache has data
        [TestMethod]
        public async Task GetAllCurrencyRates_ShouldReturnFromCache_WhenCacheIsAvailable()
        {
            // Arrange
            var cachedData = new CurrencyRateData { Rates = new Dictionary<string, double> { { "USD", 1.2 } } };
            _mockCacheService.Setup(c => c.GetFromCache<CurrencyRateData>(Constants.CurrencyDefault)).Returns(cachedData);

            // Act
            var result = await _currencyService.GetAllCurrencyRates();

            // Assert
            Assert.AreEqual(cachedData, result);
            _mockExternalClient.Verify(x => x.GetAllCurrencyRates(It.IsAny<string>()), Times.Never);
        }

        // Test: GetAllCurrencyRates() when cache is empty
        [TestMethod]
        public async Task GetAllCurrencyRates_ShouldFetchFromApi_WhenCacheIsEmpty()
        {
            // Arrange
            _mockCacheService.Setup(c => c.GetFromCache<CurrencyRateData>(Constants.CurrencyDefault)).Returns((CurrencyRateData)null);
            var apiResponse = new CurrencyRateData { Rates = new Dictionary<string, double> { { "EUR", 1.1 } } };
            _mockExternalClient.Setup(c => c.GetAllCurrencyRates(It.IsAny<string>())).ReturnsAsync(apiResponse);

            // Act
            var result = await _currencyService.GetAllCurrencyRates();

            // Assert
            Assert.AreEqual(apiResponse, result);
            _mockCacheService.Verify(c => c.AddCache(Constants.CurrencyDefault, apiResponse, TimeSpan.FromMinutes(5)), Times.Once);
        }

        // Test: GetAllCurrencyByCode()
        [TestMethod]
        public async Task GetAllCurrencyByCode_ShouldFetchFromApi_WhenCacheIsEmpty()
        {
            // Arrange
            string currencyCode = "USD";
            _mockCacheService.Setup(c => c.GetFromCache<CurrencyRateData>(Constants.CurrencyCode)).Returns((CurrencyRateData)null);
            var apiResponse = new CurrencyRateData { Rates = new Dictionary<string, double> { { "USD", 1.2 } } };
            _mockExternalClient.Setup(c => c.GetAllCurrencyRates(currencyCode)).ReturnsAsync(apiResponse);

            // Act
            var result = await _currencyService.GetAllCurrencyByCode(currencyCode);

            // Assert
            Assert.AreEqual(apiResponse, result);
            _mockCacheService.Verify(c => c.AddCache(Constants.CurrencyCode, apiResponse, TimeSpan.FromMinutes(5)), Times.Once);
        }

        // Test: ConvertCurrency()
        [TestMethod]
        public async Task ConvertCurrency_ShouldCallExternalClient()
        {
            // Arrange
            var convertData = new ConvertCurrencyData { Amount = 100, CurrencyFrom = "USD", CurrencyTo = "EUR" };
            var expectedResponse = new CurrencyResponseData { ConvertedAmount = 85 };
            _mockExternalClient.Setup(c => c.ConvertCurrency(convertData)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _currencyService.ConvertCurrency(convertData);

            // Assert
            Assert.AreEqual(expectedResponse, result);
            _mockExternalClient.Verify(x => x.ConvertCurrency(convertData), Times.Once);
        }

        // Test: GetCurrencyRateHistory() with cache
        [TestMethod]
        public async Task GetCurrencyRateHistory_ShouldReturnFromCache_WhenCacheExists()
        {
            // Arrange
            var requestData = new CurrencyRateDurationData { StartDate = DateTime.UtcNow.AddDays(-7), EndDate = DateTime.UtcNow, PageNumber = 1 };
            var cachedData = new CurrencyRateHistoryData 
            {
                Rates = new Dictionary<string, Dictionary<string, string>>
                {
                    { "USD", new Dictionary<string, string> { { "EUR", "0.91" }, { "GBP", "0.78" } } },
                    { "EUR", new Dictionary<string, string> { { "USD", "1.10" }, { "GBP", "0.85" } } }
                }
            };
            _mockCacheService.Setup(c => c.GetFromCache<CurrencyRateHistoryData>(It.IsAny<string>())).Returns(cachedData);

            // Act
            var result = await _currencyService.GetCurrencyRateHistory(requestData);

            // Assert
            Assert.IsNotNull(result, "Result should not be null");
            Assert.IsNotNull(result.Rates, "Rates should not be null");

            // Ensure data is fetched from cache
            Assert.AreEqual(cachedData.Rates.Count, result.Rates.Count, "Rates count should match cached data");

            foreach (var key in cachedData.Rates.Keys)
            {
                Assert.IsTrue(result.Rates.ContainsKey(key), $"Rates should contain key: {key}");
                CollectionAssert.AreEqual(cachedData.Rates[key], result.Rates[key], $"Rate values for {key} should match cached data");
            }

            _mockExternalClient.Verify(x => x.GetCurrencyRateHistory(It.IsAny<CurrencyRateDurationData>()), Times.Never);
        }

        // Test: GetCurrencyRateHistory() fetches from API if no cache
        [TestMethod]
        public async Task GetCurrencyRateHistory_ShouldFetchFromApi_WhenCacheIsEmpty()
        {
            // Arrange
            var requestData = new CurrencyRateDurationData { StartDate = DateTime.UtcNow.AddDays(-7), EndDate = DateTime.UtcNow, PageNumber = 1 };
            _mockCacheService.Setup(c => c.GetFromCache<CurrencyRateHistoryData>(It.IsAny<string>())).Returns((CurrencyRateHistoryData)null);

            var apiResponse = new CurrencyRateHistoryData 
            {
                Rates = new Dictionary<string, Dictionary<string, string>>
                {
                    { "USD", new Dictionary<string, string> { { "EUR", "0.91" }, { "GBP", "0.78" } } },
                    { "EUR", new Dictionary<string, string> { { "USD", "1.10" }, { "GBP", "0.85" } } }
                }
            };
            _mockExternalClient.Setup(c => c.GetCurrencyRateHistory(requestData)).ReturnsAsync(apiResponse);

            // Act
            var result = await _currencyService.GetCurrencyRateHistory(requestData);

            // Assert
            Assert.IsNotNull(result, "Result should not be null");
            Assert.IsNotNull(result.Rates, "Rates should not be null");
            Assert.AreEqual(apiResponse.Rates.Count, result.Rates.Count, "Rates count should match API response");

            foreach (var key in apiResponse.Rates.Keys)
            {
                Assert.IsTrue(result.Rates.ContainsKey(key), $"Rates should contain key: {key}");
                CollectionAssert.AreEqual(apiResponse.Rates[key], result.Rates[key], $"Rate values for {key} should match API response");
            }

            // Ensure data is stored in cache
            _mockCacheService.Verify(c => c.AddCache(It.IsAny<string>(), apiResponse, TimeSpan.FromMinutes(5)), Times.Once);
        }
    }
}
