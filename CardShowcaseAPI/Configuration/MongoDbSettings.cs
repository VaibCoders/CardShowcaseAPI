namespace CardShowcaseAPI.Configuration;

/// <summary>
/// Настройки подключения к MongoDB
/// </summary>
public class MongoDbSettings
{
    /// <summary>
    /// Строка подключения к MongoDB
    /// </summary>
    public string ConnectionString { get; set; } = null!;
    
    /// <summary>
    /// Имя базы данных
    /// </summary>
    public string DatabaseName { get; set; } = null!;
    
    /// <summary>
    /// Имя коллекции для карточек
    /// </summary>
    public string CollectionName { get; set; } = "ShowcaseCards";
}