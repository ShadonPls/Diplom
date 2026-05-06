using DiplomServer.Application.DTOs.Common;
using DiplomServer.Application.Interfaces;
using DiplomServer.Infrastructure.Repositories.Interfaces;
using System.Text.RegularExpressions;

namespace DiplomServer.Application.Services
{
    public class LookupService : ILookupService
    {
        private readonly ILookupRepository _lookupRepository;
        private readonly ICurrentUserService _currentUserService;

        public LookupService(
            ILookupRepository lookupRepository,
            ICurrentUserService currentUserService)
        {
            _lookupRepository = lookupRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<SelectListItemDto>> GetTeacherDisciplinesAsync()
        {
            var disciplines = await _lookupRepository.GetTeacherDisciplinesAsync(_currentUserService.TeacherId.Value);

            return disciplines.Select(d => new SelectListItemDto
            {
                Value = (uint)d.Id,
                Text = d.Name
            }).ToList();
        }

        public async Task<List<SelectListItemDto>> GetTeacherByDisciplinesAsync(uint disciplineId)
        {
            var teacher = await _lookupRepository.GetTeachersByDisciplineAsync(disciplineId);

            return teacher.Select(d => new SelectListItemDto
            {
                Value = (uint)d.Id,
                Text = d.Name
            }).ToList();
        }

        public async Task<List<TypeDto>> GetAttestTypesAsync() => await _lookupRepository.GetAttestTypesAsync();
        public async Task<List<TypeDto>> GetGroupsAsync() => await _lookupRepository.GetGroupsAsync();
        public async Task<List<TypeDto>> GetTeachersByIdAsync(uint teacherId) => await _lookupRepository.GetTeachersByIdAsync(teacherId);
        public async Task<List<TypeDto>> GetTeachersAsync() => await _lookupRepository.GetTeachersAsync();
        public async Task<List<SelectListItemDto>> GetSemestrByGroupAsync(uint groupId)
        {
            var groups = await _lookupRepository.GetSemestersByGroupIdAsync(groupId);

            return groups.Select(g => new SelectListItemDto
            {
                Value = (uint)g.SemesterNumber,
                Text = g.StudyYear
            }).ToList();
        }
        public async Task<List<SelectListItemDto>> GetStudentsDebts()
        {
            var students = await _lookupRepository.GetStudentsWithDebtsAsync();
            return students.Select(s => new SelectListItemDto
            {
                Value = (uint)s.Id,
                Text = s.Name
            }).ToList();
        }

        public async Task<List<SelectListItemDto>> GetDisciplinesStudentsDebts(uint studentId)
        {
            var disciplines = await _lookupRepository.GetStudentDebtsDisciplinesAsync(studentId);
            return disciplines.Select(s => new SelectListItemDto
            {
                Value = (uint)s.Id,
                Text = s.Name
            }).ToList();
        }
            
        public async Task<List<SelectListItemDto>> GetGroupStudentsAsync(uint groupId)
        {
            var students = await _lookupRepository.GetGroupStudentsAsync(groupId);
            return students.Select(s => new SelectListItemDto
            {
                Value = (uint)s.Id,
                Text = s.Name
            }).ToList();
        }

        
        public async Task<List<SelectListItemDto>> GetGroupsByDisciplineIdAsync(uint disciplineId)
        {
            var groups = await _lookupRepository.GetGroupsByDisciplineIdAsync(disciplineId);
            return groups.Select(g => new SelectListItemDto
            {
                Value = (uint)g.Id,
                Text = g.Name
            }).ToList();
        }

        public async Task<StudentTypeDto> GetStudentByIdAsync(uint studentIds)
        {
            var students = await _lookupRepository.GetStudentById(studentIds);
            return students;
        }

        public async Task<Dictionary<uint, TypeDto>> GetStudentsDictionaryByIdAsync(IEnumerable<uint> studentIds)
        {
            var students = await _lookupRepository.GetStudentsByIdsAsync(studentIds);
            return students.ToDictionary(
                s => s.Key,
                s => new TypeDto
                {
                    Id = (int)s.Value.Id,
                    Name = $"{s.Value.Lastname} {s.Value.Firstname} {s.Value.Surname}"
                });
        }

        public async Task<Dictionary<uint, TypeDto>> GetTeachersDictionaryByIdAsync(IEnumerable<uint> teachersId)
        {
            var teachers = await _lookupRepository.GetTeachersByIdsAsync(teachersId);
            return teachers.ToDictionary(
                s => s.Key,
                s => new TypeDto
                {
                    Id = (int)s.Value.Id,
                    Name = $"{s.Value.first_name} {s.Value.middle_name} {s.Value.last_name}"
                });
        }

        public async Task<TypeDto> GetDisciplineByIdAsync(uint disciplineId) =>
            await GetByIdOrThrow(disciplineId, _lookupRepository.GetDisciplineByIdAsync, "Дисциплина");

        public async Task<TypeDto> GetAttestationByIdAsync(uint attestationTypeId) =>
            await GetByIdOrThrow(attestationTypeId, _lookupRepository.GetAttestationByIdAsync, "Тип атестации");
        public async Task<TypeDto> GetGroupByIdAsync(uint groupId) =>
            await GetByIdOrThrow(groupId, _lookupRepository.GetGroupByIdAsync, "Группа");

        private async Task<T> GetByIdOrThrow<T>(uint id, Func<uint, Task<T>> repositoryCall, string entityName)
        {
            var result = await repositoryCall(id);
            if (result == null) throw new KeyNotFoundException($"{entityName} {id} не найдена.");
            return result;
        }
    }
}