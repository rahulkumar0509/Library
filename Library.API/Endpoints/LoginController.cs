using System.Reflection;
using Library.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Endpoints
{
    [ApiController]
    [EnableCors("AngularApp")]
    public class LoginController : ControllerBase
    {
        private IJwtService _jwtService;
        private ILogger<LoginController> _logger;
        public LoginController(IJwtService jwtService, ILogger<LoginController> logger)
        {
            _logger = logger;
            _jwtService = jwtService;
        }

        [AllowAnonymous] // [AllowAnonymous] doesn't work for custom middleware because it's a part of ASP.NET Core's built-in authorization framework, not the middleware pipeline itself. The framework processes this attribute after your custom middleware has already run.
        [HttpGet("v2/Login")]
        public string Login(string username, string password)
        {
            _logger.LogInformation($"{MethodBase.GetCurrentMethod().Name} : Login with creds : {username}/{password}");
            var token = _jwtService.GenerateAccessToken(username);
            if (String.IsNullOrEmpty(token))
            {
                _logger.LogInformation("Couldn't login. Wrong Credential!");
                throw new Exception("Couldn't login. Wrong Credential!");
            }
            else
            {
                _logger.LogInformation("Login Successful!");
                return token;
            }
        }
    }
}