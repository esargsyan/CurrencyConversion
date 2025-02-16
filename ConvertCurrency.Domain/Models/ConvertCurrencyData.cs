using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertCurrency.Domain.Models
{
    public class ConvertCurrencyData
    {
        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("currencyFrom")]
        public string CurrencyFrom { get; set; }

        [JsonProperty("currencyTo")]
        public string CurrencyTo { get; set; }
    }
}
