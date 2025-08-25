using System.Text.Json;
using Microsoft.AspNetCore.Http.Extensions;

namespace Library.API.Middleware
{
    public class LibraryExceptionHandlerMiddleware
    {
        RequestDelegate _next;
        ILogger<LibraryExceptionHandlerMiddleware> _logger;
        // add logger for the request/response 
        public LibraryExceptionHandlerMiddleware(RequestDelegate requestDelegate, ILogger<LibraryExceptionHandlerMiddleware> logger)
        {
            _logger = logger;
            _next = requestDelegate;
        }
        public async Task Invoke(HttpContext context)
        {
            string url = context.Request.GetEncodedUrl();
            var body = context.Request.Body.ToString(); // read payload from buffer and log it
            
            var header = context.Request.Headers["Authorization"]; // read bearer token
            _logger.LogInformation($"Url:{url},\n Body: {body},\n Headers: {header}");
            try
            {
                await _next(context);
                return;
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { Error = ex.Message });
                return;
            }
        }
    }
    public static class ExceptionHandlerMiddlewareExtension
    {
        public static IApplicationBuilder UseLibraryExceptionHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LibraryExceptionHandlerMiddleware>();
        }
    }
}