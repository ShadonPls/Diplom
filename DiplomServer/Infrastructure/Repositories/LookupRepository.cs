using DiplomServer.Application.DTOs.Common;
using DiplomServer.Domain.Entities;
using DiplomServer.Infrastructure.Data;
using DiplomServer.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace DiplomServer.Infrastructure.Repositories
{
    public class LookupRepository : ILookupRepository
    {
        private readonly ScheduleDbContext _scheduleContext;
        private readonly VrDbContext _vrContext;

        public LookupRepository(
            ScheduleDbContext scheduleContext,
            VrDbContext vrContext)
        {
            _scheduleContext = scheduleContext;
            _vrContext = vrContext;
        }
        public async Task<List<ScheduleDiscipline>> GetTeacherDisciplinesAsync(uint teacherId)
        {
            return await _scheduleContext.Disciplines
                .Join(
                    _scheduleContext.PlansDisciplines,
                    d => d.Id,
                    pd => pd.id_discipline,
                    (d, pd) => new { d, pd })
                .Join(
                    _scheduleContext.TeachersDisciplines,
                    x => x.pd.Id,
                    td => td.id_plan_discipline,
                    (x, td) => new { x.d, td })
                .Where(x => x.td.id_teacher == (int)teacherId)
                .Select(x => x.d)
                .Distinct()
                .OrderBy(d => d.Name)
                .ToListAsync();
        }
        public async Task<List<TypeDto>> GetGroupsAsync()
        {
            var rawGroups = await _scheduleContext.Groups
            .Join(
                _scheduleContext.AcademicPlans,
                g => g.id_academic_plans,
                ap => ap.Id,
                (g, ap) => new
                {
                    Id = g.Id,
                    AcademicPlanName = ap.Name,
                    Number = g.Number
                })
            .OrderBy(x => x.AcademicPlanName)
            .ThenBy(x => x.Number)
            .ToListAsync();

            var groups = rawGroups.Select(x => new
            {
                Id = x.Id,
                Name = $"{x.AcademicPlanName}-{x.Number}"
            }).ToList();

            return groups.Select(g => new TypeDto
            {
                Id = (int)g.Id,
                Name = g.Name
            }).ToList();
        }

        public async Task<List<TypeDto>> GetGroupStudentsAsync(uint groupId)
        {
            var groupRow = await _scheduleContext.Groups
                .Join(
                    _scheduleContext.AcademicPlans,
                    g => g.id_academic_plans,
                    ap => ap.Id,
                    (g, ap) => new
                    {
                        GroupId = g.Id,
                        GroupName = $"{ap.Name}-{g.Number}"
                    })
                .Where(x => x.GroupId == (int)groupId)
                .FirstOrDefaultAsync();

            if (groupRow is null)
                return new List<TypeDto>();

            return await _vrContext.Students
                .Where(s => s.Group == groupRow.GroupName)
                .OrderBy(s => s.Lastname)
                .Select(s => new TypeDto
                {
                    Id = (int)s.Id,
                    Name = $"{s.Lastname} {s.Firstname} {s.Surname}",
                })
                .ToListAsync();
        }
        public async Task<Dictionary<uint, VrStudent>> GetStudentsByIdsAsync(IEnumerable<uint> studentIds)
        {
            if (!studentIds.Any())
                return new Dictionary<uint, VrStudent>();

            var students = await _vrContext.Students
                .Where(s => studentIds.Contains((uint)s.Id))
                .Select(s => new VrStudent
                {
                    Id = (uint)s.Id,
                    Firstname = s.Firstname,
                    Lastname = s.Lastname,
                    Surname = s.Surname,
                    Group = s.Group
                })
                .ToListAsync();

            return students.ToDictionary(s => s.Id, s => s);
        }

        public async Task<List<ScheduleAttestation>> GetAttestTypesAsync()
        {
            return await _scheduleContext.Attestations
                .OrderBy(a => a.Name)
                .Select(a => new ScheduleAttestation
                {
                    Id = a.Id,
                    Name = a.Name
                })
                .ToListAsync();
        }

        public async Task<List<TypeDto>> GetTeachersAsync()
        {
            return await _scheduleContext.Teachers
                .OrderBy(a => a.first_name)
                .Where(x=>x.Delete == 0 && x.Id >0 && x.middle_name != "" && x.first_name != "" && x.last_name != "")
                .Select(a => new TypeDto
                {
                    Id = (int)a.Id,
                    Name = (a.first_name ?? "") + " " +
                   (a.middle_name ?? "") + " " +
                   (a.last_name ?? "")
                })
                .ToListAsync();
        }


        public async Task<TypeDto> GetGroupByIdAsync(uint groupId)
        {
            var groupRow = await _scheduleContext.Groups
                .Join(
                    _scheduleContext.AcademicPlans,
                    g => g.id_academic_plans,
                    ap => ap.Id,
                    (g, ap) => new
                    {
                        Id = g.Id,
                        Name = $"{ap.Name}-{g.Number}"
                    })
                .Where(x => x.Id == (int)groupId)
                .FirstOrDefaultAsync();

            if (groupRow is null)
                throw new KeyNotFoundException($"Группа {groupId} не найдена.");

            return new TypeDto
            {
                Id = (int)groupRow.Id,
                Name = groupRow.Name
            };
        }

        public async Task<TypeDto> GetDisciplineByIdAsync(uint disciplineId)
        {
            var discipline = await _scheduleContext.Disciplines
                .Where(d => d.Id == (int)disciplineId)
                .FirstOrDefaultAsync();

            if (discipline is null)
                throw new KeyNotFoundException($"Дисциплина {disciplineId} не найдена.");

            return new TypeDto
            {
                Id = (int)discipline.Id,
                Name = discipline.Name
            };
        }
        public async Task<List<TypeDto>> GetGroupsByDisciplineIdAsync(uint disciplineId)
        {
            var rawGroups = await _scheduleContext.Groups
                .Join(
                    _scheduleContext.AcademicPlans,
                    g => g.id_academic_plans,
                    ap => ap.Id,
                    (g, ap) => new { g, ap }
                )
                .Join(
                    _scheduleContext.PlansDisciplines,
                    x => x.ap.Id,
                    pd => pd.id_academic_plan,
                    (x, pd) => new { x.g, x.ap, pd }
                )
                .Where(x => x.pd.id_discipline == (int)disciplineId &&
                            x.pd.Delete == 0 &&
                            x.g.Delete == 0)
                .Select(x => new
                {
                    x.g.Id,
                    x.ap.Name,
                    x.g.Number
                })
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Number)
                .ToListAsync();

            return rawGroups.Select(g => new TypeDto
            {
                Id = (int)g.Id,
                Name = $"{g.Name}-{g.Number}"
            }).ToList();
        }
        public async Task<List<SemesterDto>> GetSemestersByGroupIdAsync(uint groupId)
        {
            var rawGroup = await _scheduleContext.Groups
                .Join(
                    _scheduleContext.AcademicPlans,
                    g => (int)g.id_academic_plans,
                    ap => (int?)ap.Id,
                    (g, ap) => new { g, ap })
                .Where(x => x.g.Id == (int)groupId && x.g.Delete == 0)
                .Select(x => new
                {
                    YearGroup = x.g.Year,
                    AcademicPlanName = x.ap.Name
                })
                .FirstOrDefaultAsync();

            if (rawGroup == null)
                throw new KeyNotFoundException($"Группа {groupId} не найдена.");

            var yearGroup = rawGroup.YearGroup;

            var now = DateTime.UtcNow;
            var currentAcademicYearStart = now.Month >= 9 ? now.Year : now.Year - 1;
            var course = currentAcademicYearStart - yearGroup + 1;

            var semesters = new List<SemesterDto>();

            var fallSemester = (course * 2) - 1;
            var springSemester = course * 2;

            semesters.Add(new SemesterDto
            {
                SemesterNumber = fallSemester,
                StudyYear = $"{currentAcademicYearStart}/{currentAcademicYearStart + 1}"
            });

            semesters.Add(new SemesterDto
            {
                SemesterNumber = springSemester,
                StudyYear = $"{currentAcademicYearStart}/{currentAcademicYearStart + 1}"
            });

            if (course > 1)
            {
                var prevYearStart = currentAcademicYearStart - 1;
                var prevFallSemester = fallSemester - 2;
                var prevSpringSemester = springSemester - 2;

                semesters.Add(new SemesterDto
                {
                    SemesterNumber = prevFallSemester,
                    StudyYear = $"{prevYearStart}/{prevYearStart + 1}"
                });

                semesters.Add(new SemesterDto
                {
                    SemesterNumber = prevSpringSemester,
                    StudyYear = $"{prevYearStart}/{prevYearStart + 1}"
                });
            }

            return semesters.OrderBy(x => x.SemesterNumber).ToList();
        }
        public async Task<TypeDto> GetAttestationByIdAsync(uint typeId)
        {
            var type = await _scheduleContext.Attestations
                .Where(a => a.Id == (int)typeId)
                .FirstOrDefaultAsync();

            if (type is null)
                throw new KeyNotFoundException($"Тип аттестации {typeId} не найден.");

            return new TypeDto
            {
                Id = (int)type.Id,
                Name = type.Name
            };
        }
    }
}