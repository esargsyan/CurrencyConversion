using ConvertCurrency.Domain.Models;
using ConvertCurrency.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace ConvertCurrency.Api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly ILogger<LoginController> _logger;
        public LoginController(ILoginService loginService, ILogger<LoginController> logger)
        {
            _loginService = loginService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginData loginData)
        {
            var stopwatch = Stopwatch.StartNew(); // Track response time

            string clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
            string clientId = User.Claims.FirstOrDefault(c => c.Type == "ClientId")?.Value ?? "Unknown Client";
            string httpMethod = HttpContext.Request.Method;
           
            _logger.LogInformation("Received {HttpMethod} request from ClientId: {ClientId}, IP: {ClientIp}, Endpoint: {Endpoint}", httpMethod, clientId, clientIp, HttpContext.Request.Path);

            try
            {
                if (loginData != null && !string.IsNullOrEmpty(loginData.UserName) && !string.IsNullOrEmpty(loginData.Password)) // validation
                {
                    var token = _loginService.Login(loginData);
                    stopwatch.Stop();
                    _logger.LogInformation("Successfully processed login request for ClientId: {ClientId}, ResponseTime: {ResponseTime}ms, Status: {StatusCode}", clientId, stopwatch.ElapsedMilliseconds, StatusCodes.Status200OK);
                    return Ok(new { token });
                }

                _logger.LogWarning("Unauthorized attempt from ClientId: {ClientId}, IP: {ClientIp}, Endpoint: {Endpoint}", clientId, clientIp, HttpContext.Request.Method);

                stopwatch.Stop();
                _logger.LogInformation("Failed login attempt for ClientId: {ClientId}, ResponseTime: {ResponseTime}ms, Status: {StatusCode}", clientId, stopwatch.ElapsedMilliseconds, StatusCodes.Status401Unauthorized);

                return Unauthorized();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error processing login request for ClientId: {ClientId}, Status: {StatusCode}, ResponseTime: {ResponseTime}ms", clientId, StatusCodes.Status500InternalServerError, stopwatch.ElapsedMilliseconds);

                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}
