using ConvertCurrency.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertCurrency.Service.Interfaces
{
    public interface ILoginRepository
    {
        string Login(LoginData loginData);
    }
}
