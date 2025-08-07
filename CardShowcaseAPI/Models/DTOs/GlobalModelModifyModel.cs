using CardShowcaseAPI.Models.Domain;
using CardShowcaseAPI.Models.Enums;

namespace CardShowcaseAPI.Models.DTOs;

/// <summary>
/// Модель для создания/изменения глобальной модели
/// </summary>
public class GlobalModelModifyModel
{
    /// <summary>
    /// Тип операции над моделью
    /// </summary>
    public GlobalModelAction Action { get; set; }

    /// <summary>
    /// ID модели (для обновления/удаления)
    /// </summary>
    public Guid? IDGlobalModel { get; set; }

    /// <summary>
    /// Название глобальной модели
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Конфигурация полей модели
    /// </summary>
    public List<GlobalModelField> Fields { get; set; } = new();

    /// <summary>
    /// ID создателя модели
    /// </summary>
    public Guid IDUserCreator { get; set; }
}
