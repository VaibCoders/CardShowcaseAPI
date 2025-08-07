using CustomHTMLCardAPI.Models.Domain;
using CustomHTMLCardAPI.Models.DTOs;

namespace CustomHTMLCardAPI.Services.Interfaces;

/// <summary>
/// Интерфейс сервиса для работы с HTML карточками
/// </summary>
public interface ICustomHTMLCardService
{
    /// <summary>
    /// Получить все карточки
    /// </summary>
    Task<List<CustomHTMLCard>> GetAllAsync();
    
    /// <summary>
    /// Получить карточку по ID
    /// </summary>
    Task<CustomHTMLCard?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Получить карточки пользователя
    /// </summary>
    Task<List<CustomHTMLCard>> GetByUserAsync(Guid userId);
    
    /// <summary>
    /// Получить публичные карточки
    /// </summary>
    Task<List<CustomHTMLCard>> GetPublicAsync();
    
    /// <summary>
    /// Создать новую карточку
    /// </summary>
    Task<CustomHTMLCard> CreateAsync(CustomHTMLCardModifyModel model);
    
    /// <summary>
    /// Обновить карточку
    /// </summary>
    Task<CustomHTMLCard?> UpdateAsync(CustomHTMLCardModifyModel model);
    
    /// <summary>
    /// Удалить карточку
    /// </summary>
    Task<bool> DeleteAsync(Guid id);
    
    /// <summary>
    /// Поиск карточек по ключевым словам
    /// </summary>
    Task<List<CustomHTMLCard>> SearchAsync(string keywords);
    
    /// <summary>
    /// Обработать карточку в зависимости от действия
    /// </summary>
    Task<CustomHTMLCard?> ProcessAsync(CustomHTMLCardModifyModel model);
}