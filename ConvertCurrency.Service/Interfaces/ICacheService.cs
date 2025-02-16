using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertCurrency.Service.Interfaces
{
    public interface ICacheService
    {
        void AddCache<T>(string key, T value, TimeSpan? expiration);

        void RemoveCache(string key);

        void Clean();

        T GetFromCache<T>(string key);
    }
}
