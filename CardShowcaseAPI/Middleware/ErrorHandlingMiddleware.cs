using System.Net;
using System.Text.Json;
using CardShowcaseAPI.Models.DTOs;

namespace CardShowcaseAPI.Middleware;

/// <summary>
/// Middleware для глобальной обработки ошибок
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ErrorHandlingMiddleware(
        RequestDelegate next, 
        ILogger<ErrorHandlingMiddleware> logger,
        IWebHostEnvironment environment)
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
            _logger.LogError(ex, "Произошла необработанная ошибка");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        
        var response = new ErrorResponse
        {
            Message = "Произошла ошибка при обработке запроса",
            ErrorCode = "INTERNAL_ERROR"
        };

        // В режиме разработки показываем детали ошибки
        if (_environment.IsDevelopment())
        {
            response.Details = exception.Message;
            response.ErrorCode = exception.GetType().Name;
        }

        context.Response.StatusCode = exception switch
        {
            ArgumentNullException => (int)HttpStatusCode.BadRequest,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.InternalServerError
        };

        // Устанавливаем правильное сообщение в зависимости от типа ошибки
        response.Message = context.Response.StatusCode switch
        {
            400 => "Некорректный запрос",
            401 => "Требуется авторизация",
            404 => "Ресурс не найден",
            _ => response.Message
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var jsonResponse = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(jsonResponse);
    }
}