namespace CardShowcaseAPI.Models.Enums;

/// <summary>
/// Тип HTML карточки
/// </summary>
public enum ShowcaseCardType
{
    /// <summary>
    /// Стандартная карточка
    /// </summary>
    Standard = 0,
    
    /// <summary>
    /// Карточка для дашборда
    /// </summary>
    Dashboard = 1,
    
    /// <summary>
    /// Карточка отчета
    /// </summary>
    Report = 2,
    
    /// <summary>
    /// Виджет
    /// </summary>
    Widget = 3,
    
    /// <summary>
    /// Пользовательский тип
    /// </summary>
    Custom = 4
}