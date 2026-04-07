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
            var userId = GetUserId();
            var items = await _repository.GetMyDraftsAsync(userId);

            var results = new List<RetakeDirectionDetailsDto>();

            foreach (var item in items)
            {
                var group = await _lookupService.GetGroupByIdAsync(item.GroupDiscipline.GroupId);
                var discipline = await _lookupService.GetDisciplineByIdAsync(item.GroupDiscipline.DisciplineId);
                var attestation = await _lookupService.GetAttestationByIdAsync(item.GroupDiscipline.AttestTypeId);

                var studentIds = item.RetakeDirectionStudents.Select(s => s.StudentId);
                var studentVrDict = await _lookupService.GetStudentsDictionaryByIdAsync(studentIds);

                var students = item.RetakeDirectionStudents.Select(s => new RetakeDirectionStudentResponseDto
                {
                    StudentId = s.StudentId,
                    StudentName = studentVrDict[s.StudentId].Name,
                    GradeValue = s.RetakeGradeValue,
                    IsPassed = s.RetakeIsPassed,
                    GradeDate = s.RetakeGradeDate
                }).ToList();

                var direction = MapToResponse(item);

                results.Add(new RetakeDirectionDetailsDto
                {
                    Direction = direction,
                    Group = group,
                    Discipline = discipline,
                    AttestationType = attestation,
                    Students = students
                });
            }

            return results;
        }

        public async Task<RetakeDirectionDetailsDto> GetByIdAsync(uint id)
        {
            var userId = GetUserId();
            var entity = await _repository.GetByIdWithIncludesAsync(id);

            if (entity is null || entity.CreatedById != userId)
                throw new KeyNotFoundException("Направление не найдено.");

            var direction = MapToResponse(entity);

            var group = await _lookupService.GetGroupByIdAsync(entity.GroupDiscipline.GroupId);
            var discipline = await _lookupService.GetDisciplineByIdAsync(entity.GroupDiscipline.DisciplineId);
            var attestation = await _lookupService.GetAttestationByIdAsync(entity.GroupDiscipline.AttestTypeId);
            var studentIds = entity.RetakeDirectionStudents.Select(s => s.StudentId);
            var studentVrDict = await _lookupService.GetStudentsDictionaryByIdAsync(studentIds);

            var students = entity.RetakeDirectionStudents.Select(s => new RetakeDirectionStudentResponseDto
            {
                StudentId = s.StudentId,
                StudentName = studentVrDict[s.StudentId].Name,
                GradeValue = s.RetakeGradeValue,
                IsPassed = s.RetakeIsPassed,
                GradeDate = s.RetakeGradeDate
            }).ToList();

            return new RetakeDirectionDetailsDto
            {
                Direction = direction,
                Group = group,
                Discipline = discipline,
                AttestationType = attestation,
                Students = students
            };
        }

        public async Task<RetakeDirectionDetailsDto> CreateAsync(CreateRetakeDirectionRequestDto dto)
        {
            var userId = GetUserId();

            if (dto.Students == null || dto.Students.Count == 0)
                throw new ArgumentException("Необходимо добавить хотя бы одного студента.");

            var groupDisciplineId = await _repository.GetOrCreateGroupDisciplineIdAsync(
                dto.GroupId,
                dto.DisciplineId,
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

            var group = await _lookupService.GetGroupByIdAsync(saved.GroupDiscipline.GroupId);
            var discipline = await _lookupService.GetDisciplineByIdAsync(saved.GroupDiscipline.DisciplineId);
            var attestation = await _lookupService.GetAttestationByIdAsync(saved.GroupDiscipline.AttestTypeId);

            var studentIds = saved.RetakeDirectionStudents.Select(s => s.StudentId);
            var studentVrDict = await _lookupService.GetStudentsDictionaryByIdAsync(studentIds);

            var studentsDetails = saved.RetakeDirectionStudents.Select(s => new RetakeDirectionStudentResponseDto
            {
                StudentId = s.StudentId,
                StudentName = studentVrDict[s.StudentId].Name,
                GradeValue = s.RetakeGradeValue,
                IsPassed = s.RetakeIsPassed,
                GradeDate = s.RetakeGradeDate
            }).ToList();

            var directionDto = MapToResponse(saved);

            return new RetakeDirectionDetailsDto
            {
                Direction = directionDto,
                Group = group,
                Discipline = discipline,
                AttestationType = attestation,
                Students = studentsDetails
            };
        }
        public async Task<RetakeDirectionDetailsDto> UpdateDraftAsync(uint id, UpdateRetakeDirectionRequestDto dto)
        {
            var userId = GetUserId();

            var direction = await _repository.GetByIdWithIncludesAsync(id);
            if (direction is null || direction.CreatedById != userId)
                throw new KeyNotFoundException("Направление не найдено.");

            if (direction.Status != RetakeDirectionStatus.Draft)
                throw new InvalidOperationException("Редактировать можно только черновик.");

            var groupDisciplineId = await _repository.GetOrCreateGroupDisciplineIdAsync(
                dto.GroupId,
                dto.DisciplineId,
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

            var group = await _lookupService.GetGroupByIdAsync(saved.GroupDiscipline.GroupId);
            var discipline = await _lookupService.GetDisciplineByIdAsync(saved.GroupDiscipline.DisciplineId);
            var attestation = await _lookupService.GetAttestationByIdAsync(saved.GroupDiscipline.AttestTypeId);

            var studentIds = saved.RetakeDirectionStudents.Select(s => s.StudentId);
            var studentVrDict = await _lookupService.GetStudentsDictionaryByIdAsync(studentIds);

            var studentsDetails = saved.RetakeDirectionStudents.Select(s => new RetakeDirectionStudentResponseDto
            {
                StudentId = s.StudentId,
                StudentName = studentVrDict[s.StudentId].Name,
                GradeValue = s.RetakeGradeValue,
                IsPassed = s.RetakeIsPassed,
                GradeDate = s.RetakeGradeDate
            }).ToList();

            var directionDto = MapToResponse(saved);

            return new RetakeDirectionDetailsDto
            {
                Direction = directionDto,
                Group = group,
                Discipline = discipline,
                AttestationType = attestation,
                Students = studentsDetails
            };
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

        public async Task DeleteAsync(uint id)
        {
            var userId = GetUserId();

            var direction = await _repository.GetByIdWithIncludesAsync(id);
            if (direction is null || direction.CreatedById != userId)
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
    }
}