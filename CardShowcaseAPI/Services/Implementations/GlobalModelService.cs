using CardShowcaseAPI.Configuration;
using CardShowcaseAPI.Models.Domain;
using CardShowcaseAPI.Models.DTOs;
using CardShowcaseAPI.Models.Enums;
using CardShowcaseAPI.Services.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CardShowcaseAPI.Services.Implementations;

/// <summary>
/// Сервис для работы с глобальными моделями в MongoDB
/// </summary>
public class GlobalModelService : IGlobalModelService
{
    private readonly IMongoCollection<GlobalModel> _modelsCollection;
    private readonly ILogger<GlobalModelService> _logger;

    public GlobalModelService(
        IOptions<MongoDbSettings> mongoDbSettings,
        ILogger<GlobalModelService> logger)
    {
        _logger = logger;

        try
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _modelsCollection = mongoDatabase.GetCollection<GlobalModel>(mongoDbSettings.Value.GlobalModelCollectionName);
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
            var indexKeys = Builders<GlobalModel>.IndexKeys;
            var indexes = new List<CreateIndexModel<GlobalModel>>
            {
                new(indexKeys.Ascending(x => x.IDUserCreator)),
                new(indexKeys.Ascending(x => x.Name)),
                new(indexKeys.Descending(x => x.CreatedAt))
            };
            _modelsCollection.Indexes.CreateMany(indexes);
            _logger.LogInformation("Индексы MongoDB для глобальных моделей успешно созданы");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Не удалось создать индексы MongoDB для глобальных моделей");
        }
    }

    public async Task<List<GlobalModel>> GetAllAsync()
    {
        try
        {
            return await _modelsCollection.Find(_ => true)
                .SortByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении всех глобальных моделей");
            throw;
        }
    }

    public async Task<GlobalModel?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _modelsCollection.Find(x => x.IDGlobalModel == id)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении глобальной модели: {ModelId}", id);
            throw;
        }
    }

    public async Task<List<GlobalModel>> GetByUserAsync(Guid userId)
    {
        try
        {
            return await _modelsCollection.Find(x => x.IDUserCreator == userId)
                .SortByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении глобальных моделей пользователя: {UserId}", userId);
            throw;
        }
    }

    public async Task<GlobalModel> CreateAsync(GlobalModelModifyModel model)
    {
        try
        {
            var entity = new GlobalModel
            {
                IDGlobalModel = Guid.NewGuid(),
                Name = model.Name,
                Fields = model.Fields,
                IDUserCreator = model.IDUserCreator,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _modelsCollection.InsertOneAsync(entity);
            _logger.LogInformation("Создана глобальная модель {ModelId}", entity.IDGlobalModel);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании глобальной модели");
            throw;
        }
    }

    public async Task<GlobalModel?> UpdateAsync(GlobalModelModifyModel model)
    {
        try
        {
            if (!model.IDGlobalModel.HasValue)
            {
                _logger.LogWarning("Попытка обновления глобальной модели без ID");
                return null;
            }

            var filter = Builders<GlobalModel>.Filter.Eq(x => x.IDGlobalModel, model.IDGlobalModel.Value);
            var update = Builders<GlobalModel>.Update
                .Set(x => x.Name, model.Name)
                .Set(x => x.Fields, model.Fields)
                .Set(x => x.UpdatedAt, DateTime.UtcNow);

            var options = new FindOneAndUpdateOptions<GlobalModel>
            {
                ReturnDocument = ReturnDocument.After
            };

            var result = await _modelsCollection.FindOneAndUpdateAsync(filter, update, options);

            if (result != null)
            {
                _logger.LogInformation("Обновлена глобальная модель {ModelId}", model.IDGlobalModel);
            }
            else
            {
                _logger.LogWarning("Глобальная модель {ModelId} не найдена для обновления", model.IDGlobalModel);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении глобальной модели: {ModelId}", model.IDGlobalModel);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var result = await _modelsCollection.DeleteOneAsync(x => x.IDGlobalModel == id);
            if (result.DeletedCount > 0)
            {
                _logger.LogInformation("Удалена глобальная модель {ModelId}", id);
                return true;
            }
            _logger.LogWarning("Глобальная модель {ModelId} не найдена для удаления", id);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении глобальной модели: {ModelId}", id);
            throw;
        }
    }

    public async Task<GlobalModel?> ProcessAsync(GlobalModelModifyModel model)
    {
        try
        {
            _logger.LogInformation("Обработка глобальной модели с действием: {Action}", model.Action);
            switch (model.Action)
            {
                case GlobalModelAction.Create:
                    return await CreateAsync(model);

                case GlobalModelAction.Update:
                {
                    var updated = await UpdateAsync(model);
                    if (updated != null)
                    {
                        return updated;
                    }

                    _logger.LogInformation("Обновление модели {ModelId} не нашло записи — выполняем создание новой", model.IDGlobalModel);
                    model.IDGlobalModel = null;
                    return await CreateAsync(model);
                }

                case GlobalModelAction.Delete:
                    if (model.IDGlobalModel.HasValue)
                    {
                        var success = await DeleteAsync(model.IDGlobalModel.Value);
                        return success ? new GlobalModel { IDGlobalModel = model.IDGlobalModel.Value } : null;
                    }
                    _logger.LogWarning("Попытка удаления глобальной модели без указания ID");
                    return null;

                default:
                    _logger.LogWarning("Неизвестное действие: {Action}", model.Action);
                    return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке глобальной модели с действием: {Action}", model.Action);
            throw;
        }
    }
}
