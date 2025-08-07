using CardShowcaseAPI.Models.Domain;
using CardShowcaseAPI.Models.DTOs;

namespace CardShowcaseAPI.Services.Interfaces;

/// <summary>
/// Интерфейс сервиса для работы с глобальными моделями
/// </summary>
public interface IGlobalModelService
{
    /// <summary>
    /// Получить все модели
    /// </summary>
    Task<List<GlobalModel>> GetAllAsync();

    /// <summary>
    /// Получить модель по ID
    /// </summary>
    Task<GlobalModel?> GetByIdAsync(Guid id);

    /// <summary>
    /// Получить модели пользователя
    /// </summary>
    Task<List<GlobalModel>> GetByUserAsync(Guid userId);

    /// <summary>
    /// Создать новую модель
    /// </summary>
    Task<GlobalModel> CreateAsync(GlobalModelModifyModel model);

    /// <summary>
    /// Обновить модель
    /// </summary>
    Task<GlobalModel?> UpdateAsync(GlobalModelModifyModel model);

    /// <summary>
    /// Удалить модель
    /// </summary>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Обработать модель в зависимости от действия
    /// </summary>
    Task<GlobalModel?> ProcessAsync(GlobalModelModifyModel model);
}
