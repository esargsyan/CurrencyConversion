using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExternal.Service.Helpers
{
    public class HttpRequestInput
    {
        public string Url { get; set; }
        public string PostData { get; set; }
        public string RequestMethod { get; set; }
        public string ContentType { get; set; }
    }
}
