using FluentValidation;
using MyTodosBackend.Application.Utility.CustomExceptions;

namespace MyTodosBackend.Api.Middleware
{
    public class ExceptionHandlingMiddleware(
        RequestDelegate _next,
        ILogger<ExceptionHandlingMiddleware> _logger)
    {

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                context.Response.ContentType = "application/json";

                context.Response.StatusCode = ex switch
                {
                    ItemNotFoundException => StatusCodes.Status404NotFound,
                    ValidationException => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError
                };

                await context.Response.WriteAsJsonAsync(new
                {
                    message = context.Response.StatusCode !=
                            StatusCodes.Status500InternalServerError ? ex?.Message : "An unexpected error occurred",
                    traceId = context.TraceIdentifier
                });
            }
        }
    }
}
