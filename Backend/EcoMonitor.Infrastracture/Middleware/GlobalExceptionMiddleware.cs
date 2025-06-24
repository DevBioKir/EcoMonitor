using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace EcoMonitor.Infrastracture.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                // вызываем следующий компонент в конвейере(контроллер например)
                await _next(context);
            }
            catch (Exception ex)
            {
                //уникальный идентификатор для поиска ошибки в логах
                var correlationId = context.TraceIdentifier;

                //логируем ошибки с их идентификатором, проще отслеживать
                _logger.LogError(ex, "Unhandled exception. CorrelationId: {CorrelationId}", correlationId);

                await HandleExceptionAsync(context, ex, correlationId);
            }
        }

        private static Task HandleExceptionAsync(
            HttpContext context,
            Exception exception,
            string correlationId)
        {
            //тело ответа - JSON
            context.Response.ContentType = "application/json";

            int statusCode;
            string message;

            switch (exception)
            {
                case NotFoundException notFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    message = notFoundException.Message;
                    break;
                case ValidationException validationException:
                    statusCode = StatusCodes.Status400BadRequest;
                    message = validationException.Message;
                    break;
                case ArgumentException argumentException:
                    statusCode = StatusCodes.Status400BadRequest;
                    message = argumentException.Message;
                    break;
                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    message = "An unexpected error occurred.";
                    break;
            }

            //устанавливает HTTP статус ответа
            context.Response.StatusCode = statusCode;

            //сериализируем JSON
            var result = JsonSerializer.Serialize(new
            {
                error = message,
                correlationId = correlationId
            });

            //отправка JSON клиенту
            return context.Response.WriteAsync(result);
        }
    }
}
