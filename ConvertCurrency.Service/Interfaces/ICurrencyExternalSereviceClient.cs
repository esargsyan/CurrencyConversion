using ConvertCurrency.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertCurrency.Service.Interfaces
{
    public interface ICurrencyExternalSereviceClient
    {
        Task<CurrencyRateData> GetAllCurrencyRates(string currency);

        Task<CurrencyResponseData> ConvertCurrency(ConvertCurrencyData convertCurrencyData);

        Task<CurrencyRateHistoryData> GetCurrencyRateHistory(CurrencyRateDurationData currencyRateDurationData);
    }
}
