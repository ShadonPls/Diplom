using DiplomServer.Application.DTOs.Common;
using DiplomServer.Application.DTOs.RetakeDirections;
using DiplomServer.Application.Interfaces;
using DiplomServer.Domain.Entities;
using DiplomServer.Domain.Enums;
using DiplomServer.Infrastructure.Repositories.Interfaces;

namespace DiplomServer.Application.Services
{
    public class RetakeDirectionService : IRetakeDirectionService
    {
        private readonly IRetakeDirectionRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public RetakeDirectionService(
            IRetakeDirectionRepository repository,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<List<RetakeDirectionResponseDto>> GetMyDraftsAsync()
        {
            var userId = GetUserId();
            var items = await _repository.GetMyDraftsAsync(userId);
            return items.Select(MapToResponse).ToList();
        }

        public async Task<RetakeDirectionResponseDto> GetByIdAsync(uint id)
        {
            var userId = GetUserId();
            var entity = await _repository.GetByIdWithIncludesAsync(id);

            if (entity is null || entity.CreatedById != userId)
                throw new KeyNotFoundException("Направление не найдено.");

            return MapToResponse(entity);
        }

        public async Task<RetakeDirectionResponseDto> CreateAsync(CreateRetakeDirectionRequestDto dto)
        {
            var userId = GetUserId();
            var teacherId = GetTeacherId();

            if (dto.Students == null || dto.Students.Count == 0)
                throw new ArgumentException("Необходимо добавить хотя бы одного студента.");

            var groupDisciplineId = await _repository.GetOrCreateGroupDisciplineIdAsync(
                dto.GroupId,
                dto.DisciplineId,
                teacherId,
                dto.AttestTypeId,
                dto.Semester,
                dto.StudyYear);

            var direction = new RetakeDirection
            {
                GroupDisciplineId = groupDisciplineId,
                CreatedById = userId,
                CreatedAt = DateTime.UtcNow,
                RetakeDate = dto.RetakeDate ?? DateTime.UtcNow.AddDays(14),
                Status = RetakeDirectionStatus.Draft
            };

            var directionId = await _repository.CreateAsync(direction);

            direction.Number = $"RD-{DateTime.UtcNow:yyyyMMdd}-{directionId:D4}";
            await _repository.UpdateAsync(direction);

            var students = dto.Students.Select(s => new RetakeDirectionStudent
            {
                RetakeDirectionId = directionId,
                StudentId = s.StudentId,
                RetakeGradeValue = s.GradeValue,
                RetakeIsPassed = s.GradeValue >= 3,
                RetakeGradeDate = s.GradeDate
            });

            await _repository.AddStudentsAsync(students);

            var saved = await _repository.GetByIdWithIncludesAsync(directionId)
                        ?? throw new KeyNotFoundException("Созданное направление не найдено.");

            return MapToResponse(saved);
        }

        public async Task<RetakeDirectionResponseDto> UpdateDraftAsync(uint id, UpdateRetakeDirectionRequestDto dto)
        {
            var userId = GetUserId();
            var teacherId = GetTeacherId();

            var direction = await _repository.GetByIdWithIncludesAsync(id);
            if (direction is null || direction.CreatedById != userId)
                throw new KeyNotFoundException("Направление не найдено.");

            if (direction.Status != RetakeDirectionStatus.Draft)
                throw new InvalidOperationException("Редактировать можно только черновик.");

            var groupDisciplineId = await _repository.GetOrCreateGroupDisciplineIdAsync(
                dto.GroupId,
                dto.DisciplineId,
                teacherId,
                dto.AttestTypeId,
                dto.Semester,
                dto.StudyYear);

            direction.GroupDisciplineId = groupDisciplineId;
            direction.Number = string.IsNullOrWhiteSpace(dto.Number)
                ? direction.Number
                : dto.Number.Trim();

            direction.RetakeDate = dto.RetakeDate ?? direction.RetakeDate;

            await _repository.UpdateAsync(direction);
            await _repository.RemoveStudentsAsync(id);

            var students = dto.Students.Select(s => new RetakeDirectionStudent
            {
                RetakeDirectionId = id,
                StudentId = s.StudentId,
                RetakeGradeValue = s.GradeValue,
                RetakeIsPassed = s.GradeValue >= 3,
                RetakeGradeDate = s.GradeDate
            });

            await _repository.AddStudentsAsync(students);

            var saved = await _repository.GetByIdWithIncludesAsync(id)
                        ?? throw new KeyNotFoundException("Обновлённое направление не найдено.");

            return MapToResponse(saved);
        }

        public async Task PublishAsync(uint id)
        {
            var userId = GetUserId();

            var direction = await _repository.GetByIdWithIncludesAsync(id);
            if (direction is null || direction.CreatedById != userId)
                throw new KeyNotFoundException("Направление не найдено.");

            if (direction.Status != RetakeDirectionStatus.Draft)
                throw new InvalidOperationException("Публиковать можно только черновик.");

            direction.Status = RetakeDirectionStatus.Published;
            await _repository.UpdateAsync(direction);
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

        public async Task<List<SelectListItemDto>> GetTeacherDisciplinesAsync()
        {
            var teacherId = GetTeacherId();
            var disciplines = await _repository.GetTeacherDisciplinesAsync(teacherId);
            return disciplines.Select(d => new SelectListItemDto
            {
                Value = d.Id,
                Text = d.Name
            }).ToList();
        }

        private uint GetUserId()
        {
            return _currentUserService.UserId;
        }

        private uint GetTeacherId()
        {
            if (!_currentUserService.TeacherId.HasValue)
                throw new UnauthorizedAccessException("TeacherId отсутствует.");
            return _currentUserService.TeacherId.Value;
        }

        private static RetakeDirectionResponseDto MapToResponse(RetakeDirection entity)
        {
            return new RetakeDirectionResponseDto
            {
                Id = entity.Id,
                Number = entity.Number ?? string.Empty,
                Status = entity.Status.ToString(),
                CreatedAt = entity.CreatedAt,
                RetakeDate = entity.RetakeDate,
                GroupId = entity.GroupDiscipline.GroupId,
                GroupName = entity.GroupDiscipline.Group.Name,
                DisciplineId = entity.GroupDiscipline.DisciplineId,
                DisciplineName = entity.GroupDiscipline.Discipline.Name,
                AttestTypeId = entity.GroupDiscipline.AttestTypeId,
                AttestTypeName = entity.GroupDiscipline.AttestType.Name,
                Semester = entity.GroupDiscipline.Semester,
                StudyYear = entity.GroupDiscipline.StudyYear,
                Students = entity.RetakeDirectionStudents.Select(s => new RetakeDirectionStudentResponseDto
                {
                    StudentId = s.StudentId,
                    StudentName = $"{s.Student.LastName} {s.Student.FirstName}".Trim(),
                    GradeValue = s.RetakeGradeValue,
                    IsPassed = s.RetakeIsPassed,
                    GradeDate = s.RetakeGradeDate
                }).ToList()
            };
        }
    }
}
