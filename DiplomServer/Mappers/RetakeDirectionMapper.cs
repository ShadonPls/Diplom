using DiplomServer.Data;
using DiplomServer.Models;
using DiplomServer.Models.DTO.RetakeDirection;
using Microsoft.EntityFrameworkCore;

namespace DiplomServer.Mappers;

public static class RetakeDirectionMapper
{
    /// <summary>
    /// Модель → Response DTO (для GET списков/деталей)
    /// Требует Include всех связанных сущностей!
    /// </summary>
    public static RetakeDirectionResponseDto ToResponseDto(RetakeDirection entity)
    {
        return new RetakeDirectionResponseDto
        {
            Id = entity.Id,
            Number = entity.Number ?? "",
            DisciplineName = entity.GroupDiscipline.Discipline.Name,
            TeacherFullName = $"{entity.GroupDiscipline.Teacher.LastName} {entity.GroupDiscipline.Teacher.FirstName}".Trim(),
            GroupName = entity.GroupDiscipline.Group.Name,
            AttestTypeName = entity.GroupDiscipline.AttestType.Name,
            StudyYear = entity.GroupDiscipline.StudyYear,
            CreatedAt = entity.CreatedAt,
            RetakeDate = entity.RetakeDate,
            Status = entity.Status,
            Students = entity.RetakeDirectionStudents.Select(ToStudentDto).ToList()
        };
    }

    /// <summary>
    /// Студент пересдачи → DTO для таблицы
    /// </summary>
    private static RetakeDirectionStudentDto ToStudentDto(RetakeDirectionStudent entity)
    {
        return new RetakeDirectionStudentDto
        {
            FullName = $"{entity.Student.LastName} {entity.Student.FirstName}".Trim(),
            RetakeGradeValue = entity.RetakeGradeValue,
            RetakeGradeDate = entity.RetakeGradeDate
        };
    }

    /// <summary>
    /// Request DTO (v1 quick) → Модель
    /// </summary>
    public static RetakeDirection ToEntity(CreateRetakeDirectionRequestDto dto, uint createdById)
    {
        var direction = new RetakeDirection
        {
            GroupDisciplineId = dto.GroupDisciplineId,
            CreatedById = createdById,
            RetakeDate = dto.RetakeDate ?? DateTime.Now.AddDays(7),
            Status = "draft",
            RetakeDirectionStudents = dto.StudentIds.Select(id => new RetakeDirectionStudent
            {
                StudentId = id,
                RetakeGradeValue = 2,  // всегда "2" для пересдачи
                RetakeIsPassed = false,
                RetakeGradeDate = DateTime.Now
            }).ToList()
        };

        return direction;
    }

    // Mappers/RetakeDirectionMapper.cs — ИСПРАВЛЕННЫЙ
    public static RetakeDirection ToEntityForm(CreateRetakeDirectionFormDto dto, uint groupDisciplineId, uint createdById)
    {
        return new RetakeDirection
        {
            GroupDisciplineId = groupDisciplineId,        // ← uint, не DbContext!
            CreatedById = createdById,
            RetakeDate = dto.RetakeDate ?? DateTime.Now.AddDays(7),
            Status = "draft",
            RetakeDirectionStudents = dto.Students.Select(s => new RetakeDirectionStudent
            {
                StudentId = s.StudentId,
                RetakeGradeValue = s.GradeValue,
                RetakeIsPassed = s.GradeValue == 5,     // 5 = Зачет
                RetakeGradeDate = s.GradeDate
            }).ToList()
        };
    }


    /// <summary>
    /// Внутренняя: Discipline+Group+Teacher+... → GroupDisciplineId
    /// </summary>
    private static async Task<GroupDiscipline> GetOrCreateGroupDiscipline(
        CreateRetakeDirectionFormDto dto,
        AppDbContext context)
    {
        var gd = await context.GroupDisciplines
            .FirstOrDefaultAsync(g =>
                g.DisciplineId == dto.DisciplineId &&
                g.GroupId == dto.GroupId &&
                g.AttestTypeId == dto.AttestTypeId &&
                g.Semester == dto.Semester &&
                g.StudyYear == dto.StudyYear);

        if (gd == null)
        {
            // TODO: взять TeacherId из createdById → Users → Teachers
            uint teacherId = 1; // заглушка
            gd = new GroupDiscipline
            {
                GroupId = dto.GroupId,
                DisciplineId = dto.DisciplineId,
                TeacherId = teacherId,
                AttestTypeId = dto.AttestTypeId,
                Semester = dto.Semester,
                StudyYear = dto.StudyYear
            };
            context.GroupDisciplines.Add(gd);
            await context.SaveChangesAsync();
        }

        return gd;
    }
}
