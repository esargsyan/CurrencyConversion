using ConvertCurrency.Domain.Models;
using ConvertCurrency.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Data;

namespace ConvertCurrency.DataLayer.Repositories
{
    public class LoginRepository : ILoginRepository
    {
        private const string _secretKey = "69c7fcbd-69b2-430c-a200-543c15420c6a"; // Must match the key in Program.cs
       
        public string Login(LoginData loginData)
        {
            string res = string.Empty;

            if (loginData.UserName == "Admin" && loginData.Password == "Qwerty123") // Hash password and check with userName in the Database
            { 
                res = GenerateToken(loginData.UserName); // Generate token with Admin role
            }
            else if (loginData.UserName == "User" && loginData.Password == "Qwerty123")
            {
                res = GenerateToken(loginData.UserName); // Generate token with User role
            }
            return res;
        }

        private string GenerateToken(string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "User"),
                new Claim(ClaimTypes.Role, role) // Add Role to JWT Token
            };

            var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2), // Token expires in 2 hours
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
