using DiplomServer.Application.DTOs.Common;

namespace DiplomServer.Application.Interfaces
{
    public interface ILookupService
    {
        Task<List<SelectListItemDto>> GetTeacherDisciplinesAsync();
        Task<List<TypeDto>> GetGroupsAsync();
        Task<List<TypeDto>> GetGroupStudentsAsync(uint groupId);
        Task<List<TypeDto>> GetAttestTypesAsync();
        Task<List<TypeDto>> GetGroupsByDisciplineIdAsync(uint disciplineId);
        Task<List<TypeDto>> GetTeachersAsync(); 
        Task<List<TypeDto>> GetSemestrByGroupAsync(uint groupId);
        Task<TypeDto> GetGroupByIdAsync(uint groupId);
        Task<Dictionary<uint, TypeDto>> GetStudentsDictionaryByIdAsync(IEnumerable<uint> studentIds);
        Task<TypeDto> GetDisciplineByIdAsync(uint disciplineId);
        Task<TypeDto> GetAttestationByIdAsync(uint typeAttestId);
    }
}