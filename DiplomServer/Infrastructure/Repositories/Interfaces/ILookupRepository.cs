using DiplomServer.Application.DTOs.Common;
using DiplomServer.Domain.Entities;
using DiplomServer.Infrastructure.Data;
using System.Text.RegularExpressions;

namespace DiplomServer.Infrastructure.Repositories.Interfaces
{
    public interface ILookupRepository
    {
        Task<List<TypeDto>> GetTeacherDisciplinesAsync(uint teacherId);
        Task<List<TypeDto>> GetGroupsAsync();
        Task<List<TypeDto>> GetTeachersAsync();
        Task<bool> TestNumber(uint number);
        Task<List<TypeDto>> GetTeachersByIdAsync(uint teacherId);
        Task<Dictionary<uint, ScheduleTeacher>> GetTeachersByIdsAsync(IEnumerable<uint> teachersId);
        Task<List<TypeDto>> GetStudentsWithDebtsAsync();
        Task<List<TypeDto>> GetStudentDebtsDisciplinesAsync(uint studentId);
        Task<List<TypeDto>> GetTeachersByDisciplineAsync(uint disciplineId);
        Task<List<TypeDto>> GetGroupsByDisciplineIdAsync(uint disciplineId);
        Task<List<SemesterDto>> GetSemestersByGroupIdAsync(uint groupId);
        Task<List<TypeDto>> GetGroupStudentsAsync(uint groupId);
        Task<List<TypeDto>> GetAttestTypesAsync();
        Task<StudentTypeDto> GetStudentById(uint studentIds);
        Task<TypeDto> GetGroupByIdAsync(uint groupId);
        Task<TypeDto> GetDisciplineByIdAsync(uint disciplineId);
        Task<Dictionary<uint, VrStudent>> GetStudentsByIdsAsync(IEnumerable<uint> studentIds);
        Task<TypeDto> GetAttestationByIdAsync(uint typeId);
    }
}