using CustomHTMLCardAPI.Configuration;
using CustomHTMLCardAPI.Models.Domain;
using CustomHTMLCardAPI.Models.DTOs;
using CustomHTMLCardAPI.Models.Enums;
using CustomHTMLCardAPI.Services.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CustomHTMLCardAPI.Services.Implementations;

/// <summary>
/// Сервис для работы с HTML карточками в MongoDB
/// </summary>
public class CustomHTMLCardService : ICustomHTMLCardService
{
    private readonly IMongoCollection<CustomHTMLCard> _cardsCollection;
    private readonly ILogger<CustomHTMLCardService> _logger;

    public CustomHTMLCardService(
        IOptions<MongoDbSettings> mongoDbSettings,
        ILogger<CustomHTMLCardService> logger)
    {
        _logger = logger;
        
        try
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _cardsCollection = mongoDatabase.GetCollection<CustomHTMLCard>(mongoDbSettings.Value.CollectionName);
            
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
            var indexKeys = Builders<CustomHTMLCard>.IndexKeys;
            var indexes = new List<CreateIndexModel<CustomHTMLCard>>
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

    public async Task<List<CustomHTMLCard>> GetAllAsync()
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

    public async Task<CustomHTMLCard?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _cardsCollection.Find(x => x.IDCustomHTMLCard == id)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении карточки с ID: {CardId}", id);
            throw;
        }
    }

    public async Task<List<CustomHTMLCard>> GetByUserAsync(Guid userId)
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

    public async Task<List<CustomHTMLCard>> GetPublicAsync()
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

    public async Task<CustomHTMLCard> CreateAsync(CustomHTMLCardModifyModel model)
    {
        try
        {
            var card = new CustomHTMLCard
            {
                IDCustomHTMLCard = model.IDCustomHTMLCard ?? Guid.NewGuid(),
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
                card.IDCustomHTMLCard, card.IDUserCreator);
            
            return card;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании карточки");
            throw;
        }
    }

    public async Task<CustomHTMLCard?> UpdateAsync(CustomHTMLCardModifyModel model)
    {
        try
        {
            if (model.IDCustomHTMLCard == null)
            {
                _logger.LogWarning("Попытка обновления карточки без указания ID");
                return null;
            }

            var filter = Builders<CustomHTMLCard>.Filter.Eq(x => x.IDCustomHTMLCard, model.IDCustomHTMLCard.Value);
            var update = Builders<CustomHTMLCard>.Update
                .Set(x => x.CardDescription, model.CardDescription)
                .Set(x => x.CardBody, model.CardBody)
                .Set(x => x.CardImage, model.CardImage)
                .Set(x => x.CardKeywords, model.CardKeywords)
                .Set(x => x.CardType, model.CardType)
                .Set(x => x.IsPublic, model.IsPublic)
                .Set(x => x.UpdatedAt, DateTime.UtcNow);

            var options = new FindOneAndUpdateOptions<CustomHTMLCard>
            {
                ReturnDocument = ReturnDocument.After
            };

            var result = await _cardsCollection.FindOneAndUpdateAsync(filter, update, options);
            
            if (result != null)
            {
                _logger.LogInformation("Обновлена карточка {CardId}", model.IDCustomHTMLCard);
            }
            else
            {
                _logger.LogWarning("Карточка {CardId} не найдена для обновления", model.IDCustomHTMLCard);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении карточки: {CardId}", model.IDCustomHTMLCard);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var result = await _cardsCollection.DeleteOneAsync(x => x.IDCustomHTMLCard == id);
            
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

    public async Task<List<CustomHTMLCard>> SearchAsync(string keywords)
    {
        try
        {
            var filter = Builders<CustomHTMLCard>.Filter.Or(
                Builders<CustomHTMLCard>.Filter.Regex(x => x.CardDescription, 
                    new MongoDB.Bson.BsonRegularExpression(keywords, "i")),
                Builders<CustomHTMLCard>.Filter.Regex(x => x.CardKeywords, 
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
public async Task<CustomHTMLCard?> ProcessAsync(CustomHTMLCardModifyModel model)
{
    try
    {
        _logger.LogInformation("Обработка карточки с действием: {Action}", model.Action);

        switch (model.Action)
        {
            case CustomHTMLCardAction.Create:
                return await CreateAsync(model);

            case CustomHTMLCardAction.Update:
            {
                var updated = await UpdateAsync(model);
                if (updated != null)
                {
                    return updated;
                }

                _logger.LogInformation(
                    "Обновление карточки {CardId} не нашло записи — выполняем создание новой", 
                    model.IDCustomHTMLCard);

                // Сбрасываем ID, чтобы CreateAsync сгенерировал новый
                model.IDCustomHTMLCard = null;
                return await CreateAsync(model);
            }

            case CustomHTMLCardAction.Delete:
                if (model.IDCustomHTMLCard.HasValue)
                {
                    var success = await DeleteAsync(model.IDCustomHTMLCard.Value);
                    return success 
                        ? new CustomHTMLCard { IDCustomHTMLCard = model.IDCustomHTMLCard.Value } 
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