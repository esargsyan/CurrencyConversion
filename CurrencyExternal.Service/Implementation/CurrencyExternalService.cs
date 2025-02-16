using ConvertCurrency.Domain;
using ConvertCurrency.Domain.Models;
using CurrencyExternal.Service.Helpers;
using CurrencyExternal.Service.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExternal.Service.Implementation
{
    public class CurrencyExternalService : ICurrencyExternalService
    {
        public async Task<CurrencyRateData> GetAllCurrencyRates(string apiBaseUrl, string currency)
        {
            string actionUrl = !string.IsNullOrEmpty(currency) ? $"{apiBaseUrl}latest?base={currency}" : $"{apiBaseUrl}latest";

            var httpRequestInput = new HttpRequestInput
            {
                ContentType = Constants.HttpContentTypes.ApplicationJson,
                RequestMethod = Constants.HttpRequestMethods.Get,
                Url = actionUrl
            };

            var currencyRateData = new CurrencyRateData();

            // Send http request
            string strResponse = await HttpRequestHelper.SendHttpRequest(httpRequestInput);

            if (!string.IsNullOrEmpty(strResponse))
            {
                currencyRateData = JsonConvert.DeserializeObject<CurrencyRateData>(strResponse);
            }
            return currencyRateData;
        }

        public async Task<CurrencyResponseData> ConvertCurrency(ConvertCurrencyData convertCurrencyData, string apiBaseUrl)
        {
            string actionUrl = $"{apiBaseUrl}latest?base={convertCurrencyData.CurrencyFrom}&symbols={convertCurrencyData.CurrencyTo}";

            var httpRequestInput = new HttpRequestInput
            {
                ContentType = Constants.HttpContentTypes.ApplicationJson,
                RequestMethod = Constants.HttpRequestMethods.Get,
                Url = actionUrl
            };
            var convertCurrencyResData = new CurrencyResponseData();

            // Send http request
            string strResponse = await HttpRequestHelper.SendHttpRequest(httpRequestInput);

            if (!string.IsNullOrEmpty(strResponse))
            {
                convertCurrencyResData = JsonConvert.DeserializeObject<CurrencyResponseData>(strResponse);
                convertCurrencyResData.Amount = convertCurrencyData.Amount;
                convertCurrencyResData.ConvertedAmount = Math.Round(convertCurrencyResData.Amount * convertCurrencyResData.Rates[convertCurrencyData.CurrencyTo], 2);
            }
            return convertCurrencyResData;
        }

        public async Task<CurrencyRateHistoryData> GetCurrencyRateHistory(CurrencyRateDurationData currencyRateDurationData, string apiBaseUrl)
        {
            string actionUrl = $"{apiBaseUrl}{currencyRateDurationData.StartDate.ToString("yyyy-MM-dd")}..{currencyRateDurationData.EndDate.ToString("yyyy-MM-dd")}";

            var httpRequestInput = new HttpRequestInput
            {
                ContentType = Constants.HttpContentTypes.ApplicationJson,
                RequestMethod = Constants.HttpRequestMethods.Get,
                Url = actionUrl
            };
            var currencyRateHistoryData = new CurrencyRateHistoryData();

            // Send http request
            string strResponse = await HttpRequestHelper.SendHttpRequest(httpRequestInput);

            if (!string.IsNullOrEmpty(strResponse))
            {
                currencyRateHistoryData = JsonConvert.DeserializeObject<CurrencyRateHistoryData>(strResponse);
            }
            return currencyRateHistoryData;
        }
    }
}
