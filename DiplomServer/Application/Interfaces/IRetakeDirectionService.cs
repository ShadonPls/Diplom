using DiplomServer.Application.DTOs.RetakeDirections;

namespace DiplomServer.Application.Interfaces
{
    public interface IRetakeDirectionService
    {
        Task<List<RetakeDirectionDetailsDto>> GetMyDraftsAsync();
        Task<RetakeDirectionDetailsDto> GetByIdAsync(uint id);
        Task<RetakeDirectionDetailsDto> CreateAsync(CreateRetakeDirectionRequestDto dto);
        Task<RetakeDirectionDetailsDto> UpdateDraftAsync(uint id, UpdateRetakeDirectionRequestDto dto);
        Task PublishAsync(uint id);
    }
}