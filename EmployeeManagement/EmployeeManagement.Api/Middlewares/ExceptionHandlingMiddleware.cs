namespace EmployeeManagement.Api.Middlewares
{
    public class ExceptionHandlingMiddleware(
        RequestDelegate _next,
        ILogger<ExceptionHandlingMiddleware> _logger
        )
    {
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(
                    new { message = "Internal server error" });
            }
        }
    }
}
