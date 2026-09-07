using OrdersService.Core.Exceptions;

namespace OrdersMicroservice.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                int statusCode;
                string title;
                string message;

                switch (ex)
                {
                    case NotFoundException notFoundEx:
                        statusCode = StatusCodes.Status404NotFound;
                        title = "Resource Not Found";
                        message = notFoundEx.Message;
                        break;

                    case ConflictException conflictEx:
                        statusCode = StatusCodes.Status409Conflict;
                        title = "Conflict";
                        message = conflictEx.Message;
                        break;

                    case UnauthorizedOperationException unauthEx:
                        statusCode = StatusCodes.Status401Unauthorized;
                        title = "Unauthorized";
                        message = unauthEx.Message;
                        break;

                    case BusinessValidationException validationEx:
                        statusCode = StatusCodes.Status400BadRequest;
                        title = "Business Validation Failed";
                        message = validationEx.Message;
                        break;

                    case ServiceUnavailableException:
                        statusCode = StatusCodes.Status503ServiceUnavailable;
                        title = "Service Unavailable";
                        message = "The service is temporarily unavailable. Please try again in a few moments.";
                        break;

                    case DownstreamTimeoutException:
                        statusCode = StatusCodes.Status504GatewayTimeout;
                        title = "Request Timeout";
                        message = "The request took too long to complete. Please try again.";
                        break;

                    default:
                        statusCode = StatusCodes.Status500InternalServerError;
                        title = "Internal Server Error";
                        message = "Something went wrong on our end. Please try again later.";
                        break;
                }

                if (statusCode >= StatusCodes.Status500InternalServerError || ex.InnerException != null)
                {
                    _logger.LogError(ex, "Server Error ({StatusCode}): {Message}. InnerException: {InnerMessage}",
                        statusCode, ex.Message, ex.InnerException?.Message);
                }

                httpContext.Response.StatusCode = statusCode;
                await httpContext.Response.WriteAsJsonAsync(new
                {
                    status = statusCode,
                    title,
                    message
                });
            }
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}

