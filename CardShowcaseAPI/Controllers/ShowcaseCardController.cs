using CardShowcaseAPI.Models.Domain;
using CardShowcaseAPI.Models.DTOs;
using CardShowcaseAPI.Models.Enums;
using CardShowcaseAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardShowcaseAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShowcaseCardController : ControllerBase
{
    private readonly IShowcaseCardService _service;
    private readonly ILogger<ShowcaseCardController> _logger;

    public ShowcaseCardController(IShowcaseCardService service, ILogger<ShowcaseCardController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<ShowcaseCard>>> GetAll() =>
        Ok(await _service.GetAllAsync());


    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ShowcaseCard?>> GetById(Guid id) =>
        await _service.GetByIdAsync(id) is ShowcaseCard card ? Ok(card) : NotFound();


    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<List<ShowcaseCard>>> GetByUser(Guid userId) =>
        Ok(await _service.GetByUserAsync(userId));

    [HttpGet("public")]

    public async Task<ActionResult<List<ShowcaseCard>>> GetPublic() =>
        Ok(await _service.GetPublicAsync());

    [HttpGet("search")]
    public async Task<ActionResult<List<ShowcaseCard>>> Search([FromQuery]string q) =>
        Ok(await _service.SearchAsync(q));

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
