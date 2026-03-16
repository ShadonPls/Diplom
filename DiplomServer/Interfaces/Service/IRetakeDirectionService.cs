using DiplomServer.Models.DTO.RetakeDirection;
using DiplomServer.Models.DTO.Select;

namespace DiplomServer.Interfaces.Service
{
    public interface IRetakeDirectionService
    {
        Task<List<RetakeDirectionResponseDto>> GetMyDraftsAsync(uint teacherId);
        Task<RetakeDirectionResponseDto?> GetByIdAsync(uint id);
        Task<RetakeDirectionResponseDto> CreateQuickAsync(CreateRetakeDirectionRequestDto dto, uint teacherId);
        Task<RetakeDirectionResponseDto> CreateFormAsync(CreateRetakeDirectionFormDto dto, uint teacherId);
        Task<byte[]> GeneratePdfAsync(uint id);
        Task<List<SelectListItemDto>> GetGroupsAsync();
        Task<List<SelectListItemDto>> GetGroupStudentsAsync(uint groupId);
    }
}
