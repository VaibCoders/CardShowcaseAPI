using CustomHTMLCardAPI.Models.Domain;
using CustomHTMLCardAPI.Models.DTOs;
using CustomHTMLCardAPI.Models.Enums;
using CustomHTMLCardAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomHTMLCardAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomHTMLCardController : ControllerBase
{
    private readonly ICustomHTMLCardService _service;
    private readonly ILogger<CustomHTMLCardController> _logger;

    public CustomHTMLCardController(ICustomHTMLCardService service, ILogger<CustomHTMLCardController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<CustomHTMLCard>>> GetAll() =>
        Ok(await _service.GetAllAsync());


    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomHTMLCard?>> GetById(Guid id) =>
        await _service.GetByIdAsync(id) is CustomHTMLCard card ? Ok(card) : NotFound();


    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<List<CustomHTMLCard>>> GetByUser(Guid userId) =>
        Ok(await _service.GetByUserAsync(userId));

    [HttpGet("public")]

    public async Task<ActionResult<List<CustomHTMLCard>>> GetPublic() =>
        Ok(await _service.GetPublicAsync());

    [HttpGet("search")]
    public async Task<ActionResult<List<CustomHTMLCard>>> Search([FromQuery]string q) =>
        Ok(await _service.SearchAsync(q));

    [HttpPost]
    public async Task<ActionResult<CustomHTMLCard?>> Process([FromBody] CustomHTMLCardModifyModel model)
    {
        var result = await _service.ProcessAsync(model);
        return result switch
        {
            null when model.Action == CustomHTMLCardAction.Delete => NoContent(),
            null => BadRequest(),
            _ => Ok(result)
        };
    }
}
