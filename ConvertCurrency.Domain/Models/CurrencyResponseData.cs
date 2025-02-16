using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertCurrency.Domain.Models
{
    public class CurrencyResponseData
    {
        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("base")]
        public string CurrencyFrom { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("rates")]
        public Dictionary<string, double> Rates { get; set; }

        [JsonProperty("convertedAmount")]
        public double ConvertedAmount { get; set; }
    }
}
