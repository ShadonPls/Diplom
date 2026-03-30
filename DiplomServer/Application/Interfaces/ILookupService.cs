using DiplomServer.Application.DTOs.Common;

namespace DiplomServer.Application.Interfaces
{
    public interface ILookupService
    {
        Task<List<SelectListItemDto>> GetTeacherDisciplinesAsync();
        Task<List<SelectListItemDto>> GetAttestTypesAsync();
    }
}