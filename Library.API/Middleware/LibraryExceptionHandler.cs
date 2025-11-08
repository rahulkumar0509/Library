using Microsoft.AspNetCore.Diagnostics;

namespace Library.API.Middleware
{
    public class LibraryExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            // await 
            httpContext.Response.StatusCode = (int)StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(new {source = "Global Exception", error=exception.Message});
            return true;
        }
    }
}