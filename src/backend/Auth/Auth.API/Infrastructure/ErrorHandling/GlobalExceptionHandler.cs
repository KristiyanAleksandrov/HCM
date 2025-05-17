using Auth.Application.Errors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Auth.API.Infrastructure.ErrorHandling
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> logger;
        private readonly ProblemDetailsFactory pdf;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, ProblemDetailsFactory pdf)
        {
            this.logger = logger;
            this.pdf = pdf;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, $"Exception occured: {exception.Message}");

            var (status, detail) = exception switch
            {
                ApiException apiEx => (apiEx.StatusCode, apiEx.Message),
                _ => (500, "Unexpected server error")
            };

            var problem = pdf.CreateProblemDetails(httpContext, status, detail: detail);
            httpContext.Response.StatusCode = status;
            await httpContext.Response.WriteAsJsonAsync(problem, httpContext.RequestAborted);
            return true; 
        }
    }
}
