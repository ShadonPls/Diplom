using DiplomServer.Application.DTOs.Common;
using DiplomServer.Domain.Entities;
using DiplomServer.Infrastructure.Data;
using System.Text.RegularExpressions;

namespace DiplomServer.Infrastructure.Repositories.Interfaces
{
    public interface ILookupRepository
    {
        Task<List<ScheduleDiscipline>> GetTeacherDisciplinesAsync(uint teacherId);
        Task<List<TypeDto>> GetGroupsAsync();
        Task<List<TypeDto>> GetTeachersAsync();
        Task<List<TypeDto>> GetGroupsByDisciplineIdAsync(uint disciplineId);
        Task<List<SemesterDto>> GetSemestersByGroupIdAsync(uint groupId);
        Task<List<TypeDto>> GetGroupStudentsAsync(uint groupId);
        Task<List<ScheduleAttestation>> GetAttestTypesAsync();
        Task<TypeDto> GetGroupByIdAsync(uint groupId);
        Task<TypeDto> GetDisciplineByIdAsync(uint disciplineId);
        Task<Dictionary<uint, VrStudent>> GetStudentsByIdsAsync(IEnumerable<uint> studentIds);
        Task<TypeDto> GetAttestationByIdAsync(uint typeId);
    }
}