using System;

namespace Stage_0_Task.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        public RequestDelegate _next;
        public ILogger<ErrorHandlingMiddleware> _logger;
        public IWebHostEnvironment _env;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
               await _next(httpContext);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unhandled exception occurred");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            httpContext.Response.ContentType = "application/json";
            var (statusCode, message) = ex switch
            {
                // 4xx Errors
                InvalidOperationException => (400, "Invalid operation"),

                // 5xx Errors
                HttpRequestException => (503, "External service unavailable"),
                TimeoutException => (504, "Request timeout"),
                _ => (500, "An unexpected error occurred")
            };

            var response = new
            {
                error = new
                {
                    status = "error",
                    statusCode = statusCode,
                    message = message,
                    type = ex.GetType().Name,
                    traceId = httpContext.TraceIdentifier,
                    stackTrace = _env.IsDevelopment() ? ex.StackTrace : null
                }
            };

            return httpContext.Response.WriteAsJsonAsync(response);
        }
    }
}
