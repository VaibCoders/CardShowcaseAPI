namespace CustomHTMLCardAPI.Models.DTOs;

/// <summary>
/// Модель ответа об ошибке
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string Message { get; set; } = null!;
    
    /// <summary>
    /// Детали ошибки (только в режиме разработки)
    /// </summary>
    public string? Details { get; set; }
    
    /// <summary>
    /// Код ошибки
    /// </summary>
    public string? ErrorCode { get; set; }
    
    /// <summary>
    /// Время возникновения ошибки
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}