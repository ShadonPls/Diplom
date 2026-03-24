using DiplomServer.Interfaces.Service;
using DiplomServer.Models.DTO.RetakeDirection;
using DiplomServer.Models.DTO.Select;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RetakeDirectionsController : ControllerBase
{
    private readonly IRetakeDirectionService _service;

    public RetakeDirectionsController(IRetakeDirectionService service)
    {
        _service = service;
    }

    /// <summary>
    /// Мои черновики направлений
    /// </summary>
    [HttpGet("my")]
    public async Task<ActionResult<List<RetakeDirectionResponseDto>>> GetMyDrafts()
    {
        uint teacherId = 1;
        var drafts = await _service.GetMyDraftsAsync(teacherId);
        return Ok(drafts);  
    }

    /// <summary>
    /// Быстрое создание
    /// </summary>
    [HttpPost("quick")]
    public async Task<ActionResult<RetakeDirectionResponseDto>> CreateQuick(
        [FromBody] CreateRetakeDirectionRequestDto dto)
    {
        uint teacherId = 1;
        var direction = await _service.CreateQuickAsync(dto, teacherId);
        return CreatedAtAction(nameof(GetMyDrafts), direction);
    }

    /// <summary>
    /// Полная форма
    /// </summary>
    [HttpPost("form")]
    public async Task<ActionResult<RetakeDirectionResponseDto>> CreateForm(
        [FromBody] CreateRetakeDirectionFormDto dto)
    {
        uint teacherId = 1;
        var direction = await _service.CreateFormAsync(dto, teacherId);
        return CreatedAtAction(nameof(GetMyDrafts), direction);
    }

    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> DownloadPdf(uint id)
    {
        //Заглушка на нерабочий контроллер
        return Ok();
    }

    [HttpGet("groups")]
    public async Task<ActionResult<List<SelectListItemDto>>> GetGroups()
    {
        var groups = await _service.GetGroupsAsync();
        return Ok(groups);
    }

    /// <summary>
    /// Dropdown студенты группы
    /// </summary>
    [HttpGet("groups/{groupId}/students")]
    public async Task<ActionResult<List<SelectListItemDto>>> GetGroupStudents(uint groupId)
    {
        var students = await _service.GetGroupStudentsAsync(groupId);
        return Ok(students);
    }
}
