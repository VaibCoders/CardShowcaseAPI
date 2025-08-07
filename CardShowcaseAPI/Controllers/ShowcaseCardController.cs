using CardShowcaseAPI.Models.Domain;
using CardShowcaseAPI.Models.DTOs;
using CardShowcaseAPI.Models.Enums;
using CardShowcaseAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardShowcaseAPI.Controllers;

/// <summary>
/// Контроллер для работы с HTML карточками
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShowcaseCardController : ControllerBase
{
    private readonly IShowcaseCardService _service;

    public ShowcaseCardController(IShowcaseCardService service)
    {
        _service = service;
    }

    /// <summary>
    /// Получить все карточки
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ShowcaseCard>>> GetAll() =>
        Ok(await _service.GetAllAsync());


    /// <summary>
    /// Получить карточку по ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ShowcaseCard?>> GetById(Guid id) =>
        await _service.GetByIdAsync(id) is ShowcaseCard card ? Ok(card) : NotFound();


    /// <summary>
    /// Получить карточки пользователя
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<List<ShowcaseCard>>> GetByUser(Guid userId) =>
        Ok(await _service.GetByUserAsync(userId));

    /// <summary>
    /// Получить публичные карточки
    /// </summary>
    [HttpGet("public")]
    public async Task<ActionResult<List<ShowcaseCard>>> GetPublic() =>
        Ok(await _service.GetPublicAsync());

    /// <summary>
    /// Поиск карточек по строке
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<List<ShowcaseCard>>> Search([FromQuery]string q) =>
        Ok(await _service.SearchAsync(q));

    /// <summary>
    /// Создание, обновление или удаление карточки
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ShowcaseCard?>> Process([FromBody] ShowcaseCardModifyModel model)
    {
        var result = await _service.ProcessAsync(model);
        return result switch
        {
            null when model.Action == ShowcaseCardAction.Delete => NoContent(),
            null => BadRequest(),
            _ => Ok(result)
        };
    }
}
