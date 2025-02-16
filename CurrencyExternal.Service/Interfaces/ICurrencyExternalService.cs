using ConvertCurrency.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExternal.Service.Interfaces
{
    public interface ICurrencyExternalService
    {
        Task<CurrencyRateData> GetAllCurrencyRates(string apiBaseUrl, string currency);

        Task<CurrencyResponseData> ConvertCurrency(ConvertCurrencyData currencyData, string apiBaseUrl);

        Task<CurrencyRateHistoryData> GetCurrencyRateHistory(CurrencyRateDurationData currencyRateDurationData, string apiBaseUrl);
    }
}
