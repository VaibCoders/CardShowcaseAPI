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
public class GlobalModelController : ControllerBase
{
    private readonly IGlobalModelService _service;
    private readonly ILogger<GlobalModelController> _logger;

    public GlobalModelController(IGlobalModelService service, ILogger<GlobalModelController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<GlobalModel>>> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GlobalModel?>> GetById(Guid id) =>
        await _service.GetByIdAsync(id) is GlobalModel model ? Ok(model) : NotFound();

    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<List<GlobalModel>>> GetByUser(Guid userId) =>
        Ok(await _service.GetByUserAsync(userId));

    [HttpPost]
    public async Task<ActionResult<GlobalModel?>> Process([FromBody] GlobalModelModifyModel model)
    {
        var result = await _service.ProcessAsync(model);
        return result switch
        {
            null when model.Action == GlobalModelAction.Delete => NoContent(),
            null => BadRequest(),
            _ => Ok(result)
        };
    }
}
