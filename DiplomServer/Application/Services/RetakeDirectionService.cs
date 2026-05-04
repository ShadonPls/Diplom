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
        private readonly ILookupService _lookupService;
        private readonly ICurrentUserService _currentUserService;

        public RetakeDirectionService(
        IRetakeDirectionRepository repository,
        ILookupService lookupService,
        ICurrentUserService currentUserService)
        {
            _repository = repository;
            _lookupService = lookupService;
            _currentUserService = currentUserService;
        }

        public async Task<List<RetakeDirectionDetailsDto>> GetMyDraftsAsync()
        {
            var items = await _repository.GetMyDraftsAsync(GetUserId());

            var result = new List<RetakeDirectionDetailsDto>();
            foreach (var item in items) result.Add(await MapToDetailsAsync(item));
            return result;
        }

        public async Task<RetakeDirectionDetailsDto> GetByIdAsync(uint id)
        {
            var entity = await _repository.GetByIdWithIncludesAsync(id);
            if (entity == null || entity.CreatedById != GetUserId())
                throw new KeyNotFoundException("Направление не найдено.");
            return await MapToDetailsAsync(entity);
        }

        public async Task<RetakeDirectionDetailsDto> CreateAsync(CreateRetakeDirectionRequestDto dto)
        {
            if (dto.Students == null || dto.Students.Count == 0)
                throw new ArgumentException("Необходимо добавить хотя бы одного студента.");

            var groupDisciplineId = await _repository.GetOrCreateGroupDisciplineIdAsync(
                dto.GroupId, dto.DisciplineId, dto.AttestTypeId, dto.Semester, dto.StudyYear);

            var direction = new RetakeDirection
            {
                GroupDisciplineId = groupDisciplineId,
                CreatedById = GetUserId(),
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

            return await GetByIdAsync(directionId);
        }
        public async Task<RetakeDirectionDetailsDto> UpdateDraftAsync(uint id, UpdateRetakeDirectionRequestDto dto)
        {
            var direction = await _repository.GetByIdWithIncludesAsync(id);

            if (direction is null || direction.CreatedById != GetUserId())
                throw new KeyNotFoundException("Направление не найдено.");

            if (direction.Status != RetakeDirectionStatus.Draft)
                throw new InvalidOperationException("Редактировать можно только черновик.");

            direction.GroupDisciplineId = await _repository.GetOrCreateGroupDisciplineIdAsync(
                dto.GroupId, dto.DisciplineId, dto.AttestTypeId, dto.Semester, dto.StudyYear);

            direction.Number = string.IsNullOrWhiteSpace(dto.Number) ? direction.Number : dto.Number.Trim();
            direction.RetakeDate = dto.RetakeDate ?? direction.RetakeDate;

            await _repository.UpdateAsync(direction);

            await _repository.UpdateStudentsAsync(direction, dto);

            return await MapToDetailsAsync(await _repository.GetByIdWithIncludesAsync(id));
        }

        public async Task PublishAsync(uint id)
        {
            var direction = await _repository.GetByIdWithIncludesAsync(id);
            if (direction is null || direction.CreatedById != GetUserId())
                throw new KeyNotFoundException("Направление не найдено.");

            if (direction.Status != RetakeDirectionStatus.Draft)
                throw new InvalidOperationException("Публиковать можно только черновик.");

            direction.Status = RetakeDirectionStatus.Published;
            await _repository.UpdateAsync(direction);
        }

        public async Task DeleteAsync(uint id)
        {
            var direction = await _repository.GetByIdWithIncludesAsync(id);
            if (direction is null || direction.CreatedById != GetUserId())
                throw new KeyNotFoundException("Направление не найдено.");

            if (direction.Status != RetakeDirectionStatus.Draft)
                throw new InvalidOperationException("Удалять можно только черновики.");

            await _repository.DeleteAsync(id);
        }

        private uint GetUserId()
        {
            return _currentUserService.UserId;
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
                DisciplineId = entity.GroupDiscipline.DisciplineId,
                AttestTypeId = entity.GroupDiscipline.AttestTypeId,
                Semester = entity.GroupDiscipline.Semester,
                StudyYear = entity.GroupDiscipline.StudyYear
            };
        }

        private async Task<RetakeDirectionDetailsDto> MapToDetailsAsync(RetakeDirection entity)
        {
            var groupTask = _lookupService.GetGroupByIdAsync(entity.GroupDiscipline.GroupId);
            var discTask = _lookupService.GetDisciplineByIdAsync(entity.GroupDiscipline.DisciplineId);
            var attTask = _lookupService.GetAttestationByIdAsync(entity.GroupDiscipline.AttestTypeId);

            var studentIds = entity.RetakeDirectionStudents.Select(s => s.StudentId);
            var dictTask = _lookupService.GetStudentsDictionaryByIdAsync(studentIds);

            await Task.WhenAll(groupTask, discTask, attTask, dictTask);

            return new RetakeDirectionDetailsDto
            {
                Direction = MapToResponse(entity),
                Group = groupTask.Result,
                Discipline = discTask.Result,
                AttestationType = attTask.Result,
                Students = entity.RetakeDirectionStudents.Select(s => new RetakeDirectionStudentResponseDto
                {
                    StudentId = s.StudentId,
                    StudentName = dictTask.Result[s.StudentId].Name,
                    GradeValue = s.RetakeGradeValue,
                    IsPassed = s.RetakeIsPassed,
                    GradeDate = s.RetakeGradeDate
                }).ToList()
            };
        }
    }
}