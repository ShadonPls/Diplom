using DiplomServer.Application.DTOs.RetakeDirections;
using DiplomServer.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiplomServer.Controllers
{
    [ApiController]
    [Route("api/retake-directions")]
    [Authorize]
    public class RetakeDirectionsController : ControllerBase
    {
        private readonly IRetakeDirectionService _service;

        public RetakeDirectionsController(IRetakeDirectionService service)
        {
            _service = service;
        }

        [HttpGet("drafts")]
        public async Task<ActionResult<List<RetakeDirectionResponseDto>>> GetMyDrafts()
        {
            return Ok(await _service.GetMyDraftsAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RetakeDirectionResponseDto>> GetById(uint id)
        {
            return Ok(await _service.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<ActionResult<RetakeDirectionResponseDto>> Create([FromBody] CreateRetakeDirectionRequestDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RetakeDirectionResponseDto>> Update(uint id, [FromBody] UpdateRetakeDirectionRequestDto dto)
        {
            var result = await _service.UpdateDraftAsync(id, dto);
            return Ok(result);
        }

        [HttpPost("{id}/publish")]
        public async Task<IActionResult> Publish(uint id)
        {
            await _service.PublishAsync(id);
            return NoContent();
        }
    }
}