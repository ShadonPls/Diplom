using DiplomServer.Application.DTOs.RetakeDirections;

namespace DiplomServer.Application.Interfaces
{
    public interface IRetakeDirectionService
    {
        Task<List<RetakeDirectionResponseDto>> GetMyDraftsAsync();
        Task<RetakeDirectionResponseDto> GetByIdAsync(uint id);
        Task<RetakeDirectionResponseDto> CreateAsync(CreateRetakeDirectionRequestDto dto);
        Task<RetakeDirectionResponseDto> UpdateDraftAsync(uint id, UpdateRetakeDirectionRequestDto dto);
        Task PublishAsync(uint id);
    }
}