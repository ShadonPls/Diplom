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
                .Include(rd => rd.GroupDiscipline)
                .Include(rd => rd.RetakeDirectionStudents)
                .Where(rd => rd.CreatedById == userId &&
                             rd.Status == RetakeDirectionStatus.Draft)
                .OrderByDescending(rd => rd.CreatedAt)
                .ToListAsync();
        }

        public async Task<RetakeDirection?> GetByIdWithIncludesAsync(uint id)
        {
            return await _context.RetakeDirections
                .Include(rd => rd.GroupDiscipline)
                .Include(rd => rd.RetakeDirectionStudents)
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
            uint attestTypeId,
            int semester,
            string studyYear)
        {
            var existingId = await _context.GroupDisciplines
                .Where(gd => gd.GroupId == groupId &&
                             gd.DisciplineId == disciplineId &&
                             gd.AttestTypeId == attestTypeId &&
                             gd.Semester == semester &&
                             gd.StudyYear == studyYear)
                .Select(gd => gd.Id)
                .FirstOrDefaultAsync();

            if (existingId > 0)
                return existingId;

            var entity = new GroupDiscipline
            {
                GroupId = groupId,
                DisciplineId = disciplineId,
                AttestTypeId = attestTypeId,
                Semester = semester,
                StudyYear = studyYear
            };

            _context.GroupDisciplines.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }
        public async Task DeleteAsync(uint id)
        {
            var entity = await _context.RetakeDirections.FindAsync(id);
            if (entity is not null)
            {
                _context.RetakeDirections.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveStudentsAsync(uint directionId)
        {
            var students = await _context.RetakeDirectionStudents
                .Where(rr => rr.RetakeDirectionId == directionId)
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