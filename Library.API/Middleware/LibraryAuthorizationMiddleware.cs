
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Library.Services;
using Newtonsoft.Json;

namespace Library.API.Middleware
{
    // custom middleware
    public class LibraryAuthorizationMiddleware
    {
        private ILogger<LibraryAuthorizationMiddleware> _logger;
        private RequestDelegate _requestDelegate;

        public LibraryAuthorizationMiddleware(RequestDelegate next, ILogger<LibraryAuthorizationMiddleware> logger)
        {
            _logger = logger;
            _requestDelegate = next;
        }

        public async Task Invoke(HttpContext context, IJwtService jwtService)
        {
            _logger.LogInformation("inside middleware");
            if (context.Request.Path.StartsWithSegments("/v2/Login"))
            {
                await _requestDelegate(context);
                return;
            }
            if (String.IsNullOrEmpty(context.Request.Headers["Authorization"]))
            {
                _logger.LogError("Authorization token not available!");
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Authorization token not available! Login and attach bearer token");
                return;
            }
            string authHeader = context.Request.Headers["Authorization"].FirstOrDefault().Split(" ")[1];
            Console.WriteLine($"header token {authHeader}", authHeader);
            if (String.IsNullOrEmpty(authHeader))
            {
                _logger.LogError("Invalid token!");
                return;
            }
            if (jwtService.VerifyToken(authHeader))
            {
                await _requestDelegate(context);
                return;
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Unauthorizeddddd...");
                return;
            }
        }
    }

    public static class AuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseLibraryAuthenticationMiddleware(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UseMiddleware<LibraryAuthorizationMiddleware>();
        }
    }
}