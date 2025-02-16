using ConvertCurrency.Domain.Models;
using ConvertCurrency.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ConvertCurrency.Service.Implementation
{
    public class LoginService : ILoginService
    {
        private readonly ILoginRepository _loginRepository;

        public LoginService(ILoginRepository loginRepository)
        {
            _loginRepository = loginRepository;
        }

        public string Login(LoginData loginData)
        {
            return _loginRepository.Login(loginData);
        }
    }
}
