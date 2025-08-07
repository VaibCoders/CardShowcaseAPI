using CardShowcaseAPI.Configuration;
using CardShowcaseAPI.Models.Domain;
using CardShowcaseAPI.Models.DTOs;
using CardShowcaseAPI.Models.Enums;
using CardShowcaseAPI.Services.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CardShowcaseAPI.Services.Implementations;

/// <summary>
/// Сервис для работы с HTML карточками в MongoDB
/// </summary>
public class ShowcaseCardService : IShowcaseCardService
{
    private readonly IMongoCollection<ShowcaseCard> _cardsCollection;
    private readonly ILogger<ShowcaseCardService> _logger;

    public ShowcaseCardService(
        IOptions<MongoDbSettings> mongoDbSettings,
        ILogger<ShowcaseCardService> logger)
    {
        _logger = logger;
        
        try
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _cardsCollection = mongoDatabase.GetCollection<ShowcaseCard>(mongoDbSettings.Value.CollectionName);
            
            // Создаем индексы при инициализации
            CreateIndexes();
            
            _logger.LogInformation("Успешное подключение к MongoDB: {Database}", mongoDbSettings.Value.DatabaseName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка подключения к MongoDB");
            throw;
        }
    }

    /// <summary>
    /// Создание индексов для оптимизации запросов
    /// </summary>
    private void CreateIndexes()
    {
        try
        {
            var indexKeys = Builders<ShowcaseCard>.IndexKeys;
            var indexes = new List<CreateIndexModel<ShowcaseCard>>
            {
                new(indexKeys.Ascending(x => x.IDUserCreator)),
                new(indexKeys.Ascending(x => x.IsPublic)),
                new(indexKeys.Text(x => x.CardDescription).Text(x => x.CardKeywords)),
                new(indexKeys.Descending(x => x.CreatedAt))
            };
            
            _cardsCollection.Indexes.CreateMany(indexes);
            _logger.LogInformation("Индексы MongoDB успешно созданы");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Не удалось создать индексы MongoDB");
        }
    }

    /// <summary>
    /// Получить все карточки
    /// </summary>
    public async Task<List<ShowcaseCard>> GetAllAsync()
    {
        try
        {
            return await _cardsCollection.Find(_ => true)
                .SortByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении всех карточек");
            throw;
        }
    }

    /// <summary>
    /// Найти карточку по идентификатору
    /// </summary>
    public async Task<ShowcaseCard?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _cardsCollection.Find(x => x.IDShowcaseCard == id)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении карточки с ID: {CardId}", id);
            throw;
        }
    }

    /// <summary>
    /// Получить карточки пользователя
    /// </summary>
    public async Task<List<ShowcaseCard>> GetByUserAsync(Guid userId)
    {
        try
        {
            return await _cardsCollection.Find(x => x.IDUserCreator == userId)
                .SortByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении карточек пользователя: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Получить публичные карточки
    /// </summary>
    public async Task<List<ShowcaseCard>> GetPublicAsync()
    {
        try
        {
            return await _cardsCollection.Find(x => x.IsPublic)
                .SortByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении публичных карточек");
            throw;
        }
    }

    /// <summary>
    /// Создать новую карточку
    /// </summary>
    public async Task<ShowcaseCard> CreateAsync(ShowcaseCardModifyModel model)
    {
        try
        {
            var card = new ShowcaseCard
            {
                IDShowcaseCard = model.IDShowcaseCard ?? Guid.NewGuid(),
                CardDescription = model.CardDescription,
                CardBody = model.CardBody,
                CardImage = model.CardImage,
                CardKeywords = model.CardKeywords,
                CardType = model.CardType,
                IsPublic = model.IsPublic,
                IDUserCreator = model.IDUserCreator,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _cardsCollection.InsertOneAsync(card);
            _logger.LogInformation("Создана карточка {CardId} пользователем {UserId}", 
                card.IDShowcaseCard, card.IDUserCreator);
            
            return card;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании карточки");
            throw;
        }
    }

    /// <summary>
    /// Обновить существующую карточку
    /// </summary>
    public async Task<ShowcaseCard?> UpdateAsync(ShowcaseCardModifyModel model)
    {
        try
        {
            if (model.IDShowcaseCard == null)
            {
                _logger.LogWarning("Попытка обновления карточки без указания ID");
                return null;
            }

            var filter = Builders<ShowcaseCard>.Filter.Eq(x => x.IDShowcaseCard, model.IDShowcaseCard.Value);
            var update = Builders<ShowcaseCard>.Update
                .Set(x => x.CardDescription, model.CardDescription)
                .Set(x => x.CardBody, model.CardBody)
                .Set(x => x.CardImage, model.CardImage)
                .Set(x => x.CardKeywords, model.CardKeywords)
                .Set(x => x.CardType, model.CardType)
                .Set(x => x.IsPublic, model.IsPublic)
                .Set(x => x.UpdatedAt, DateTime.UtcNow);

            var options = new FindOneAndUpdateOptions<ShowcaseCard>
            {
                ReturnDocument = ReturnDocument.After
            };

            var result = await _cardsCollection.FindOneAndUpdateAsync(filter, update, options);
            
            if (result != null)
            {
                _logger.LogInformation("Обновлена карточка {CardId}", model.IDShowcaseCard);
            }
            else
            {
                _logger.LogWarning("Карточка {CardId} не найдена для обновления", model.IDShowcaseCard);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении карточки: {CardId}", model.IDShowcaseCard);
            throw;
        }
    }

    /// <summary>
    /// Удалить карточку
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var result = await _cardsCollection.DeleteOneAsync(x => x.IDShowcaseCard == id);
            
            if (result.DeletedCount > 0)
            {
                _logger.LogInformation("Удалена карточка {CardId}", id);
                return true;
            }
            
            _logger.LogWarning("Карточка {CardId} не найдена для удаления", id);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении карточки: {CardId}", id);
            throw;
        }
    }

    /// <summary>
    /// Поиск карточек по ключевым словам
    /// </summary>
    public async Task<List<ShowcaseCard>> SearchAsync(string keywords)
    {
        try
        {
            var filter = Builders<ShowcaseCard>.Filter.Or(
                Builders<ShowcaseCard>.Filter.Regex(x => x.CardDescription, 
                    new MongoDB.Bson.BsonRegularExpression(keywords, "i")),
                Builders<ShowcaseCard>.Filter.Regex(x => x.CardKeywords, 
                    new MongoDB.Bson.BsonRegularExpression(keywords, "i"))
            );

            var result = await _cardsCollection.Find(filter)
                .SortByDescending(x => x.CreatedAt)
                .ToListAsync();
            
            _logger.LogInformation("Поиск по ключевым словам '{Keywords}' вернул {Count} результатов", 
                keywords, result.Count);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при поиске по ключевым словам: {Keywords}", keywords);
            throw;
        }
    }

    /// <summary>
    /// Обработать карточку в зависимости от действия
    /// </summary>
    public async Task<ShowcaseCard?> ProcessAsync(ShowcaseCardModifyModel model)
    {
        try
        {
            _logger.LogInformation("Обработка карточки с действием: {Action}", model.Action);

            switch (model.Action)
            {
                case ShowcaseCardAction.Create:
                    return await CreateAsync(model);

                case ShowcaseCardAction.Update:
                {
                    var updated = await UpdateAsync(model);
                    if (updated != null)
                    {
                        return updated;
                    }

                    _logger.LogInformation(
                        "Обновление карточки {CardId} не нашло записи — выполняем создание новой",
                        model.IDShowcaseCard);

                    // Сбрасываем ID, чтобы CreateAsync сгенерировал новый
                    model.IDShowcaseCard = null;
                    return await CreateAsync(model);
                }

                case ShowcaseCardAction.Delete:
                    if (model.IDShowcaseCard.HasValue)
                    {
                        var success = await DeleteAsync(model.IDShowcaseCard.Value);
                        return success
                            ? new ShowcaseCard { IDShowcaseCard = model.IDShowcaseCard.Value }
                            : null;
                    }
                    _logger.LogWarning("Попытка удаления карточки без указания ID");
                    return null;

                default:
                    _logger.LogWarning("Неизвестное действие: {Action}", model.Action);
                    return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке карточки с действием: {Action}", model.Action);
            throw;
        }
    }

}
