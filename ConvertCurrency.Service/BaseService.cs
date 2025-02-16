using ConvertCurrency.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertCurrency.Service
{
    public class BaseService
    {
        protected readonly ICacheService _cacheService;
        public BaseService(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }
    }
}
