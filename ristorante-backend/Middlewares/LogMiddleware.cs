using System.Security.Claims;
using ristorante_backend.Services;

namespace ristorante_backend.Middlewares
{
    public class LogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ICustomLogger _logger;

        public LogMiddleware(RequestDelegate next, ICustomLogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            DateTime startTime = DateTime.Now;
            string utente = context.User?.FindFirst(ClaimTypes.Email)?.Value ?? "Sconosciuto";

            _logger.WriteRequest(context, utente);

            await _next(context);

            DateTime endTime = DateTime.Now;
            int duration = (int)(endTime - startTime).TotalMilliseconds;

            _logger.WriteResponse(context, utente, duration);
        }
    }
}
