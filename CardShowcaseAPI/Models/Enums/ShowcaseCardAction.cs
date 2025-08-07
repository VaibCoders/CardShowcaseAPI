namespace CardShowcaseAPI.Models.Enums;

/// <summary>
/// Действие над карточкой
/// </summary>
public enum ShowcaseCardAction
{
    /// <summary>
    /// Создание карточки
    /// </summary>
    Create = 0,
    
    /// <summary>
    /// Обновление карточки
    /// </summary>
    Update = 1,
    
    /// <summary>
    /// Удаление карточки
    /// </summary>
    Delete = 2
}