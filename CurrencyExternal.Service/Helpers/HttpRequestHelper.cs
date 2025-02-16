using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExternal.Service.Helpers
{
    public class HttpRequestHelper
    {
        public static async Task<string> SendHttpRequest(HttpRequestInput input)
        {
            string result = string.Empty;

            using (var handler = new HttpClientHandler())
            {
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                };

                using (var client = new HttpClient(handler))
                {
                    var response = await client.GetAsync(new Uri(input.Url));
                    result = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        //throw new IetaValidationException(result, response.StatusCode);
                    }
                }
            }
            return result;
        }
    }
}
