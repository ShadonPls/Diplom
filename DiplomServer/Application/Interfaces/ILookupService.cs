using DiplomServer.Application.DTOs.Common;

namespace DiplomServer.Application.Interfaces
{
    public interface ILookupService
    {
        Task<List<SelectListItemDto>> GetTeacherDisciplinesAsync();
        Task<List<TypeDto>> GetGroupsAsync();
        Task<List<SelectListItemDto>> GetGroupStudentsAsync(uint groupId);
        Task<List<TypeDto>> GetAttestTypesAsync();
        Task<List<SelectListItemDto>> GetGroupsByDisciplineIdAsync(uint disciplineId);
        Task<List<TypeDto>> GetTeachersByIdAsync(uint teacherId);
        Task<List<SelectListItemDto>> GetTeachersAsync();
        Task<List<SelectListItemDto>> GetStudentsDebts();
        Task<List<SelectListItemDto>> GetDisciplinesStudentsDebts(uint studentId);
        Task<List<SelectListItemDto>> GetTeacherByDisciplinesAsync(uint disciplineId);
        Task<Dictionary<uint, TypeDto>> GetTeachersDictionaryByIdAsync(IEnumerable<uint> teachersId);
        Task<List<SelectListItemDto>> GetSemestrByGroupAsync(uint groupId);
        Task<TypeDto> GetGroupByIdAsync(uint groupId);
        Task<StudentTypeDto> GetStudentByIdAsync(uint studentIds);
        Task<Dictionary<uint, TypeDto>> GetStudentsDictionaryByIdAsync(IEnumerable<uint> studentIds);
        Task<TypeDto> GetDisciplineByIdAsync(uint disciplineId);
        Task<TypeDto> GetAttestationByIdAsync(uint typeAttestId);
    }
}