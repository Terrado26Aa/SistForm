namespace AuthLogin.Middleware
{
    /// <summary>
    /// Middleware global que captura cualquier excepción no manejada,
    /// la registra con Serilog y devuelve al cliente un mensaje genérico
    /// sin exponer detalles internos del servidor.
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción no controlada en la petición {Method} {Path}",
                    context.Request.Method, context.Request.Path);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var errorResponse = new
                {
                    StatusCode = 500,
                    Message = "Ocurrió un error interno en el servidor. Por favor intente más tarde."
                };

                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        }
    }
}
