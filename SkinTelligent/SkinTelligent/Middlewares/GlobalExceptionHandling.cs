using SkinTelIigentContracts.CustomResponses;
using System.Net;
using System.Text.Json;

namespace SkinTelligent.Api.Middlewares
{
    public class GlobalExceptionHandling
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandling> _logger;
        private readonly IHostEnvironment _environment;


        public GlobalExceptionHandling(IHostEnvironment environment, RequestDelegate next, ILogger<GlobalExceptionHandling> logger)
        {
            _next = next;
            _logger = logger;
            _environment = environment;

        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                string errorMessage;
                if (_environment.IsDevelopment())
                {
                    errorMessage = ex.Message; 
                }
                else
                {
                    errorMessage = !string.IsNullOrEmpty(ex.Message) ? ex.Message : "An error occurred.";
                }
                var response = new ExceptionServerResponse(errorMessage);
                var jsonResponse = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(jsonResponse);
            }
        }

    }
}
