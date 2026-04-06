using DiplomServer.Application.DTOs.Common;
using DiplomServer.Application.Interfaces;
using DiplomServer.Infrastructure.Repositories.Interfaces;

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
            if (!_currentUserService.TeacherId.HasValue)
                throw new UnauthorizedAccessException("TeacherId отсутствует.");

            var disciplines = await _lookupRepository.GetTeacherDisciplinesAsync(_currentUserService.TeacherId.Value);

            return disciplines.Select(d => new SelectListItemDto
            {
                Value = (uint)d.Id,
                Text = d.Name
            }).ToList();
        }

        public async Task<List<TypeDto>> GetAttestTypesAsync()
        {
            var attestTypes = await _lookupRepository.GetAttestTypesAsync();

            return attestTypes.Select(a => new TypeDto
            {
                Id = (int)a.Id,
                Name = a.Name
            }).ToList();
        }

        public async Task<List<TypeDto>> GetGroupsAsync()
        {
            var groups = await _lookupRepository.GetGroupsAsync();

            return groups.Select(g => new TypeDto
            {
                Id = g.Id,
                Name = g.Name
            }).ToList();
        }

        public async Task<List<TypeDto>> GetTeachersAsync()
        {
            var groups = await _lookupRepository.GetTeachersAsync();

            return groups.Select(g => new TypeDto
            {
                Id = g.Id,
                Name = g.Name
            }).ToList();
        }
        public async Task<List<TypeDto>> GetSemestrByGroupAsync(uint groupId)
        {
            var groups = await _lookupRepository.GetSemestersByGroupIdAsync(groupId);

            return groups.Select(g => new TypeDto
            {
                Id = g.SemesterNumber,
                Name = g.StudyYear
            }).ToList();
        }

        public async Task<List<TypeDto>> GetGroupStudentsAsync(uint groupId)
        {
            var students = await _lookupRepository.GetGroupStudentsAsync(groupId);
            return students.Select(s => new TypeDto
            {
                Id = s.Id,
                Name = s.Name
            }).ToList();
        }


        public async Task<TypeDto> GetGroupByIdAsync(uint groupId)
        {
            var group = await _lookupRepository.GetGroupByIdAsync(groupId);
            return new TypeDto
            {
                Id = group.Id,
                Name = group.Name
            };
        }
        public async Task<List<TypeDto>> GetGroupsByDisciplineIdAsync(uint disciplineId)
        {
            var groups = await _lookupRepository.GetGroupsByDisciplineIdAsync(disciplineId);
            return groups.Select(g => new TypeDto
            {
                Id = g.Id,
                Name = g.Name
            }).ToList();
        }
        
        public async Task<TypeDto> GetDisciplineByIdAsync(uint disciplineId)
        {
            var discipline = await _lookupRepository.GetDisciplineByIdAsync(disciplineId);
            return new TypeDto
            {
                Id = discipline.Id,
                Name = discipline.Name
            };
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

        public async Task<TypeDto> GetAttestationByIdAsync(uint attestationTypeId)
        {
            var type = await _lookupRepository.GetAttestationByIdAsync(attestationTypeId);
            return new TypeDto
            {
                Id = type.Id,
                Name = type.Name
            };
        }

    }
}