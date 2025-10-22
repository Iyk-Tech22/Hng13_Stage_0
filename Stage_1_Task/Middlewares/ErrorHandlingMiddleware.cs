using Stage_1_Task.Exceptions;

namespace Stage_1_Task.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            httpContext.Response.ContentType = "application/json";

            var (statusCode, message) = ex switch
            {
                ConflictException => (409, "String already exists in the system"),
                BadRequestException => (400, "Invalid request body or missing \"value\" field"),
                UnprocessableEntityException => (422, " Invalid data type for \"value\" (must be string)"),
                NotFoundException => (404, "String does not exist in the system"),
                _ => (500, "Something went wrong")
            };

            var response = new
            {
                errors = new
                {
                    statusCode = statusCode,
                    message = message,
                    type = ex.GetType().Name,
                    traceId = httpContext.TraceIdentifier,
                    stack = _env.IsDevelopment() ? ex.StackTrace : null
                }
            };
            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.WriteAsJsonAsync(response);
        }
    }
}
