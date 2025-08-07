namespace CardShowcaseAPI.Models.Enums;

/// <summary>
/// Тип источника данных для поля глобальной модели
/// </summary>
public enum GlobalModelSourceType
{
    /// <summary>
    /// Данные получаются из Web API
    /// </summary>
    WebApi = 0,

    /// <summary>
    /// Данные получаются из датаблока
    /// </summary>
    Datablock = 1
}
