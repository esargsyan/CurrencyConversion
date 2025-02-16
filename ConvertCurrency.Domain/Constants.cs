using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertCurrency.Domain
{
    public class Constants
    {
        public const string CurrencyDefault = "CurrencyDefault";
        public const string CurrencyCode = "CurrencyCode"; 
        public const string CurrencyRateDuration = "CurrencyRateDuration";

        public class HttpContentTypes
        {
            public const string ApplicationJson = "application/json";
            public const string ApplicationXml = "application/xml";
            public const string ApplicationUrlEncoded = "application/x-www-form-urlencoded";
            public const string TextXml = "text/xml";
            public const string TextJson = "text/json";
        }

        public class HttpRequestMethods
        {
            public static string Post = "POST";
            public static string Get = "GET";
            public static string Delete = "DELETE";
            public static string Put = "PUT";
            public static string Patch = "PATCH";
        }

    }
}
