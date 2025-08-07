using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using CardShowcaseAPI.Models.Enums;

namespace CardShowcaseAPI.Models.Domain;

/// <summary>
/// Глобальная модель для хранения конфигурации
/// </summary>
public class GlobalModel
{
    /// <summary>
    /// Уникальный идентификатор глобальной модели
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid IDGlobalModel { get; set; }

    /// <summary>
    /// Название глобальной модели
    /// </summary>
    [BsonElement("Name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Конфигурация полей модели
    /// </summary>
    [BsonElement("Fields")]
    public List<GlobalModelField> Fields { get; set; } = new();

    /// <summary>
    /// ID пользователя, создавшего модель
    /// </summary>
    [BsonElement("IDUserCreator")]
    [BsonRepresentation(BsonType.String)]
    public Guid IDUserCreator { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    [BsonElement("CreatedAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    [BsonElement("UpdatedAt")]
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Описание поля глобальной модели
/// </summary>
public class GlobalModelField
{
    /// <summary>
    /// Имя поля модели
    /// </summary>
    [BsonElement("FieldName")]
    public string FieldName { get; set; } = null!;

    /// <summary>
    /// Тип источника данных поля
    /// </summary>
    [BsonElement("SourceType")]
    [BsonRepresentation(BsonType.String)]
    public GlobalModelSourceType SourceType { get; set; }

    /// <summary>
    /// Источник данных Web API
    /// </summary>
    [BsonElement("FieldSource")]
    public WebApiSource? FieldSource { get; set; }

    /// <summary>
    /// Источник данных датаблока
    /// </summary>
    [BsonElement("DatablockSource")]
    public string? DatablockSource { get; set; }
}

/// <summary>
/// Информация о методе Web API
/// </summary>
public class WebApiSource
{
    /// <summary>
    /// Идентификатор конфигурации источника
    /// </summary>
    [BsonElement("Id")]
    public string Id { get; set; } = null!;

    /// <summary>
    /// Идентификатор объекта Web API
    /// </summary>
    [BsonElement("WebApiId")]
    public object? WebApiId { get; set; }

    /// <summary>
    /// Название метода
    /// </summary>
    [BsonElement("MethodId")]
    public string MethodId { get; set; } = null!;

    /// <summary>
    /// Флаг интеграции
    /// </summary>
    [BsonElement("Integration")]
    public bool Integration { get; set; }

    /// <summary>
    /// Параметры запроса
    /// </summary>
    [BsonElement("Params")]
    public List<MethodParam> Params { get; set; } = new();

    /// <summary>
    /// Параметры запроса в виде JSON
    /// </summary>
    [BsonElement("JsonParams")]
    public string JsonParams { get; set; } = string.Empty;

    /// <summary>
    /// Использовать ли JsonParams
    /// </summary>
    [BsonElement("UseJsonParams")]
    public bool UseJsonParams { get; set; }
}

/// <summary>
/// Параметр запроса
/// </summary>
public class MethodParam
{
    /// <summary>
    /// Имя параметра
    /// </summary>
    [BsonElement("Name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Значение параметра
    /// </summary>
    [BsonElement("Value")]
    public object? Value { get; set; }
}
