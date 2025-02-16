using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConvertCurrency.Domain.Models
{
    public class CurrencyRateHistoryData
    {
        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("base")]
        public string BaseCurrency { get; set; }

        [JsonProperty("start_date")]
        public string StartDate { get; set; }

        [JsonProperty("end_date")]
        public string EndDate { get; set; }

        [JsonProperty("rates")]
        public Dictionary<string, Dictionary<string, string>> Rates { get; set; }

        public object Clone()
        {
            return new CurrencyRateHistoryData 
            {
                Amount = Amount,
                BaseCurrency = BaseCurrency,
                StartDate = StartDate,
                EndDate = EndDate,
                Rates = Rates
            };
        }
    }
}
