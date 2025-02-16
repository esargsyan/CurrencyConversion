using ConvertCurrency.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertCurrency.Service.Interfaces
{
    public interface ICurrencyService
    {
        Task<CurrencyRateData> GetAllCurrencyRates();

        Task<CurrencyRateData> GetAllCurrencyByCode(string currency);

        Task<CurrencyResponseData> ConvertCurrency(ConvertCurrencyData currencyData);

        Task<CurrencyRateHistoryData> GetCurrencyRateHistory(CurrencyRateDurationData currencyRateDurationData);
        
    }
}
