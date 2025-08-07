using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using CustomHTMLCardAPI.Models.Enums;

namespace CustomHTMLCardAPI.Models.Domain;

/// <summary>
/// Модель HTML карточки для хранения в MongoDB
/// </summary>
public class CustomHTMLCard
{
    /// <summary>
    /// Уникальный идентификатор карточки
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid IDCustomHTMLCard { get; set; }

    /// <summary>
    /// Описание карточки
    /// </summary>
    [BsonElement("CardDescription")]
    public string CardDescription { get; set; } = null!;

    /// <summary>
    /// HTML содержимое карточки
    /// </summary>
    [BsonElement("CardBody")]
    public string CardBody { get; set; } = null!;

    /// <summary>
    /// Изображение карточки (Base64 или URL)
    /// </summary>
    [BsonElement("CardImage")]
    public string? CardImage { get; set; }

    /// <summary>
    /// Ключевые слова для поиска
    /// </summary>
    [BsonElement("CardKeywords")]
    public string? CardKeywords { get; set; }

    /// <summary>
    /// Тип карточки
    /// </summary>
    [BsonElement("CardType")]
    public CustomHTMLCardType CardType { get; set; }

    /// <summary>
    /// Признак публичной карточки
    /// </summary>
    [BsonElement("IsPublic")]
    public bool IsPublic { get; set; }

    /// <summary>
    /// ID создателя карточки
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