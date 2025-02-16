using ConvertCurrency.Service.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertCurrency.Service.Implementation
{
    public class CacheService : ICacheService
    {
        private static readonly object lockObject = new object();
        private static readonly List<string> cacheEntries = new List<string>();

        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T GetFromCache<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        public void AddCache<T>(string key, T value, TimeSpan? expiration)
        {
            cacheEntries.Add(key);
            lock (lockObject)
            {
                if (expiration != null)
                {
                    _cache.Set(key, value, DateTime.UtcNow.Add(expiration.Value));
                }
                else
                {
                    _cache.Set(key, value);
                }
            }
        }

        public void RemoveCache(string key)
        {
            lock (lockObject)
            {
                _cache.Remove(key);
                cacheEntries.Remove(key);
            }
        }

        public void Clean()
        {
            lock (lockObject)
            {
                foreach (var key in cacheEntries)
                {
                    _cache.Remove(key);
                }
                cacheEntries.Clear();
            }
        }
    }
}
