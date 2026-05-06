using DiplomServer.Application.DTOs.Orders;
using DiplomServer.Application.Interfaces;
using DiplomServer.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiplomServer.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _service;

        public OrdersController(IOrdersService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrdersResponseDto>>> GetAll()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrdersResponseDto>> GetById(uint id)
        {
            var item = await _service.GetByIdAsync(id);
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<OrdersResponseDto>> Create(
            [FromBody] CreateOrdersRequestDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<OrdersResponseDto>> Update(
            uint id,
            [FromBody] UpdateOrdersRequestDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(uint id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}