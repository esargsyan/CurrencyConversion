using ConvertCurrency.Domain;
using ConvertCurrency.Domain.Models;
using ConvertCurrency.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConvertCurrency.Service.Implementation
{
    public class CurrencyService : BaseService, ICurrencyService
    {
        private readonly ICurrencyExternalSereviceClient _currencyExternalSereviceClient;

        public CurrencyService(ICurrencyExternalSereviceClient currencyExternalSereviceClient, ICacheService cacheService) : base(cacheService)
        {
            _currencyExternalSereviceClient = currencyExternalSereviceClient;
        }

        public async Task<CurrencyRateData> GetAllCurrencyRates()
        {
            var oldValue = _cacheService.GetFromCache<CurrencyRateData>(Constants.CurrencyDefault);
            if (oldValue != null)
            {
                return oldValue;
            }
            var newValue = await _currencyExternalSereviceClient.GetAllCurrencyRates(string.Empty);
            _cacheService.AddCache(Constants.CurrencyDefault, newValue, TimeSpan.FromMinutes(5));
            return newValue;
        }

        public async Task<CurrencyRateData> GetAllCurrencyByCode(string currency)
        {
            var oldValue = _cacheService.GetFromCache<CurrencyRateData>(Constants.CurrencyCode);
            if (oldValue != null)
            {
                return oldValue;
            }
            var newValue = await _currencyExternalSereviceClient.GetAllCurrencyRates(currency);
            _cacheService.AddCache(Constants.CurrencyCode, newValue, TimeSpan.FromMinutes(5));
            return newValue;
        }

        public async Task<CurrencyResponseData> ConvertCurrency(ConvertCurrencyData convertCurrencyData)
        {    
            var result = await _currencyExternalSereviceClient.ConvertCurrency(convertCurrencyData);
            return result;
        }

        public async Task<CurrencyRateHistoryData> GetCurrencyRateHistory(CurrencyRateDurationData currencyRateDurationData)
        {
            var oldValue = _cacheService.GetFromCache<CurrencyRateHistoryData>($"{Constants.CurrencyRateDuration}_{currencyRateDurationData.StartDate.ToString("yyyy-MM-dd")}_{currencyRateDurationData.EndDate.ToString("yyyy-MM-dd")}");
            if (oldValue != null)
            {
                var resValue = (CurrencyRateHistoryData)oldValue.Clone();
                resValue.Rates = resValue.Rates.Skip(currencyRateDurationData.PageNumber - 1).Take(1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                return resValue;
            }
            var newValue = await _currencyExternalSereviceClient.GetCurrencyRateHistory(currencyRateDurationData);
            var cacheValue = (CurrencyRateHistoryData)newValue.Clone();
            _cacheService.AddCache($"{Constants.CurrencyRateDuration}_{currencyRateDurationData.StartDate.ToString("yyyy-MM-dd")}_{currencyRateDurationData.EndDate.ToString("yyyy-MM-dd")}", cacheValue, TimeSpan.FromMinutes(5));
            newValue.Rates = newValue.Rates.Skip(currencyRateDurationData.PageNumber - 1).Take(1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            return newValue;
        }
    }
}
