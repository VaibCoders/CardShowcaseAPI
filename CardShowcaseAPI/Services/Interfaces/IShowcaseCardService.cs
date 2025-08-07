using CardShowcaseAPI.Models.Domain;
using CardShowcaseAPI.Models.DTOs;

namespace CardShowcaseAPI.Services.Interfaces;

/// <summary>
/// Интерфейс сервиса для работы с HTML карточками
/// </summary>
public interface IShowcaseCardService
{
    /// <summary>
    /// Получить все карточки
    /// </summary>
    Task<List<ShowcaseCard>> GetAllAsync();
    
    /// <summary>
    /// Получить карточку по ID
    /// </summary>
    Task<ShowcaseCard?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Получить карточки пользователя
    /// </summary>
    Task<List<ShowcaseCard>> GetByUserAsync(Guid userId);
    
    /// <summary>
    /// Получить публичные карточки
    /// </summary>
    Task<List<ShowcaseCard>> GetPublicAsync();
    
    /// <summary>
    /// Создать новую карточку
    /// </summary>
    Task<ShowcaseCard> CreateAsync(ShowcaseCardModifyModel model);
    
    /// <summary>
    /// Обновить карточку
    /// </summary>
    Task<ShowcaseCard?> UpdateAsync(ShowcaseCardModifyModel model);
    
    /// <summary>
    /// Удалить карточку
    /// </summary>
    Task<bool> DeleteAsync(Guid id);
    
    /// <summary>
    /// Поиск карточек по ключевым словам
    /// </summary>
    Task<List<ShowcaseCard>> SearchAsync(string keywords);
    
    /// <summary>
    /// Обработать карточку в зависимости от действия
    /// </summary>
    Task<ShowcaseCard?> ProcessAsync(ShowcaseCardModifyModel model);
}