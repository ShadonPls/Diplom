using DiplomServer.Application.DTOs.Orders;
using System.Collections.Generic;

namespace DiplomServer.Application.Interfaces
{
    public interface IOrdersService
    {
        Task<List<OrdersResponseDto>> GetAllAsync();
        Task<OrdersResponseDto> GetByIdAsync(uint id);
        Task<OrdersResponseDto> CreateAsync(CreateOrdersRequestDto dto);
        Task<OrdersResponseDto> UpdateAsync(uint id, UpdateOrdersRequestDto dto);
        Task DeleteAsync(uint id);
    }
}