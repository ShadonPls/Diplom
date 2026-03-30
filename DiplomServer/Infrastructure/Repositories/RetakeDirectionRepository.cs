using DiplomServer.Domain.Entities;
using DiplomServer.Domain.Enums;
using DiplomServer.Infrastructure.Data;
using DiplomServer.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DiplomServer.Infrastructure.Repositories
{
    public class RetakeDirectionRepository : IRetakeDirectionRepository
    {
        private readonly AppDbContext _context;

        public RetakeDirectionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<RetakeDirection>> GetMyDraftsAsync(uint userId)
        {
            return await _context.RetakeDirections
                .Include(rd => rd.GroupDiscipline).ThenInclude(gd => gd.Group)
                .Include(rd => rd.GroupDiscipline).ThenInclude(gd => gd.Discipline)
                .Include(rd => rd.GroupDiscipline).ThenInclude(gd => gd.AttestType)
                .Include(rd => rd.RetakeDirectionStudents).ThenInclude(rds => rds.Student)
                .Where(rd => rd.CreatedById == userId && rd.Status == RetakeDirectionStatus.Draft)
                .OrderByDescending(rd => rd.CreatedAt)
                .ToListAsync();
        }

        public async Task<RetakeDirection?> GetByIdWithIncludesAsync(uint id)
        {
            return await _context.RetakeDirections
                .Include(rd => rd.GroupDiscipline).ThenInclude(gd => gd.Group)
                .Include(rd => rd.GroupDiscipline).ThenInclude(gd => gd.Discipline)
                .Include(rd => rd.GroupDiscipline).ThenInclude(gd => gd.AttestType)
                .Include(rd => rd.RetakeDirectionStudents).ThenInclude(rds => rds.Student)
                .FirstOrDefaultAsync(rd => rd.Id == id);
        }

        public async Task<uint> CreateAsync(RetakeDirection direction)
        {
            _context.RetakeDirections.Add(direction);
            await _context.SaveChangesAsync();
            return direction.Id;
        }

        public async Task UpdateAsync(RetakeDirection direction)
        {
            _context.RetakeDirections.Update(direction);
            await _context.SaveChangesAsync();
        }

        public async Task<uint> GetOrCreateGroupDisciplineIdAsync(
            uint groupId,
            uint disciplineId,
            uint teacherId,
            uint attestTypeId,
            int semester,
            string studyYear)
        {
            var groupExists = await _context.Groups.AnyAsync(g => g.Id == groupId);
            if (!groupExists)
                throw new ArgumentException($"Группа с ID {groupId} не существует.");

            var disciplineExists = await _context.Disciplines.AnyAsync(d => d.Id == disciplineId);
            if (!disciplineExists)
                throw new ArgumentException($"Дисциплина с ID {disciplineId} не существует.");

            var attestTypeExists = await _context.AttestTypes.AnyAsync(a => a.Id == attestTypeId);
            if (!attestTypeExists)
                throw new ArgumentException($"Тип аттестации с ID {attestTypeId} не существует.");

            var existingId = await _context.GroupDisciplines
                .Where(gd => gd.GroupId == groupId
                          && gd.DisciplineId == disciplineId
                          && gd.TeacherId == teacherId
                          && gd.AttestTypeId == attestTypeId
                          && gd.Semester == semester
                          && gd.StudyYear == studyYear)
                .Select(gd => gd.Id)
                .FirstOrDefaultAsync();

            if (existingId > 0)
                return existingId;

            var entity = new GroupDiscipline
            {
                GroupId = groupId,
                DisciplineId = disciplineId,
                TeacherId = teacherId,
                AttestTypeId = attestTypeId,
                Semester = semester,
                StudyYear = studyYear
            };

            _context.GroupDisciplines.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<List<Group>> GetGroupsAsync()
        {
            return await _context.Groups
                .Where(g => g.Students.Any(s => s.IsActive == true))
                .OrderBy(g => g.Name)
                .ToListAsync();
        }

        public async Task<List<Student>> GetGroupStudentsAsync(uint groupId)
        {
            return await _context.Students
                .Where(s => s.GroupId == groupId && s.IsActive == true)
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToListAsync();
        }

        public async Task<List<Discipline>> GetTeacherDisciplinesAsync(uint teacherId)
        {
            return await _context.Disciplines
                .Join(_context.GroupDisciplines,
                    d => d.Id,
                    gd => gd.DisciplineId,
                    (d, gd) => new { d, gd })
                .Where(x => x.gd.TeacherId == teacherId)
                .Select(x => x.d)
                .Distinct()
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task RemoveStudentsAsync(uint directionId)
        {
            var students = await _context.RetakeDirectionStudents
                .Where(x => x.RetakeDirectionId == directionId)
                .ToListAsync();

            _context.RetakeDirectionStudents.RemoveRange(students);
            await _context.SaveChangesAsync();
        }

        public async Task AddStudentsAsync(IEnumerable<RetakeDirectionStudent> students)
        {
            await _context.RetakeDirectionStudents.AddRangeAsync(students);
            await _context.SaveChangesAsync();
        }
    }
}