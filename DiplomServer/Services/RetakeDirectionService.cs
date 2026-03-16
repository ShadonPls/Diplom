using DiplomServer.Interfaces.Repository;
using DiplomServer.Interfaces.Service;
using DiplomServer.Interfaces;
using DiplomServer.Mappers;
using DiplomServer.Models;
using DiplomServer.Models.DTO.RetakeDirection;
using DiplomServer.Models.DTO.Select;

namespace DiplomServer.Services
{
    public class RetakeDirectionService : IRetakeDirectionService
    {
        private readonly IRetakeDirectionRepository _repository;

        public RetakeDirectionService(IRetakeDirectionRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<RetakeDirectionResponseDto>> GetMyDraftsAsync(uint teacherId)
        {
            var directions = await _repository.GetMyDraftsAsync(teacherId);
            return directions.Select(RetakeDirectionMapper.ToResponseDto).ToList();
        }

        public async Task<RetakeDirectionResponseDto?> GetByIdAsync(uint id)
        {
            var direction = await _repository.GetByIdWithIncludesAsync(id);
            if (direction == null) return null;
            return RetakeDirectionMapper.ToResponseDto(direction);
        }

        public async Task<RetakeDirectionResponseDto> CreateQuickAsync(CreateRetakeDirectionRequestDto dto, uint teacherId)
        {
            var direction = RetakeDirectionMapper.ToEntity(dto, teacherId);
            uint id = await _repository.CreateAsync(direction);


            string number = $"RD-{DateTime.Now:yyyyMMdd}-{id}";
            await _repository.UpdateNumberAsync(id, number);

            var savedDirection = await _repository.GetByIdWithIncludesAsync(id);
            return RetakeDirectionMapper.ToResponseDto(savedDirection!);
        }

        public async Task<RetakeDirectionResponseDto> CreateFormAsync(CreateRetakeDirectionFormDto dto, uint teacherId)
        {
            var groupDiscipline = await _repository.GetGroupDisciplineAsync(
                dto.DisciplineId, dto.GroupId, dto.AttestTypeId, dto.Semester, dto.StudyYear);

            if (groupDiscipline == null)
            {
                var newGd = new GroupDiscipline
                {
                    GroupId = dto.GroupId,
                    DisciplineId = dto.DisciplineId,
                    TeacherId = teacherId,
                    AttestTypeId = dto.AttestTypeId,
                    Semester = dto.Semester,
                    StudyYear = dto.StudyYear
                };
                groupDiscipline = await _repository.CreateGroupDisciplineAsync(newGd);
            }

            var direction = RetakeDirectionMapper.ToEntityForm(dto, groupDiscipline.Id, teacherId);

            uint id = await _repository.CreateAsync(direction);

            string number = $"RD-{DateTime.Now:yyyyMMdd}-{id}";
            await _repository.UpdateNumberAsync(id, number);

            var savedDirection = await _repository.GetByIdWithIncludesAsync(id);
            return RetakeDirectionMapper.ToResponseDto(savedDirection!);
        }


        public async Task<byte[]> GeneratePdfAsync(uint id)
        {
            return Array.Empty<byte>();
        }

        public async Task<List<SelectListItemDto>> GetGroupsAsync()
        {
            var groups = await _repository.GetGroupsAsync();
            return groups.Select(g => new SelectListItemDto
            {
                Value = g.Id,
                Text = g.Name
            }).ToList();
        }

        public async Task<List<SelectListItemDto>> GetGroupStudentsAsync(uint groupId)
        {
            var students = await _repository.GetGroupStudentsAsync(groupId);
            return students.Select(s => new SelectListItemDto
            {
                Value = s.Id,
                Text = $"{s.LastName} {s.FirstName}".Trim()
            }).ToList();
        }
    }
}
