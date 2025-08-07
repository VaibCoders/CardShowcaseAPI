using CardShowcaseAPI.Models.Enums;

namespace CardShowcaseAPI.Models.DTOs;

/// <summary>
/// Модель для создания/изменения карточки
/// </summary>
public class ShowcaseCardModifyModel
{
    /// <summary>
    /// Тип операции над карточкой
    /// </summary>
    public ShowcaseCardAction Action { get; set; }
    
    /// <summary>
    /// ID карточки (для обновления/удаления)
    /// </summary>
    public Guid? IDShowcaseCard { get; set; }
    
    /// <summary>
    /// Описание карточки
    /// </summary>
    public string CardDescription { get; set; } = null!;
    
    /// <summary>
    /// HTML содержимое карточки
    /// </summary>
    public string CardBody { get; set; } = null!;
    
    /// <summary>
    /// Изображение карточки
    /// </summary>
    public string? CardImage { get; set; }
    
    /// <summary>
    /// Ключевые слова для поиска
    /// </summary>
    public string? CardKeywords { get; set; }
    
    /// <summary>
    /// Тип карточки
    /// </summary>
    public ShowcaseCardType CardType { get; set; }
    
    /// <summary>
    /// Признак публичной карточки
    /// </summary>
    public bool IsPublic { get; set; }
    
    /// <summary>
    /// ID создателя карточки
    /// </summary>
    public Guid IDUserCreator { get; set; }
}