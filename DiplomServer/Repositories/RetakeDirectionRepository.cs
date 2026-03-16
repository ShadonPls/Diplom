using DiplomServer.Data;
using DiplomServer.Interfaces;
using DiplomServer.Interfaces.Repository;
using DiplomServer.Models;
using Microsoft.EntityFrameworkCore;

namespace DiplomServer.Repositories
{
    public class RetakeDirectionRepository : IRetakeDirectionRepository
    {
        private readonly AppDbContext _context;

        public RetakeDirectionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<RetakeDirection>> GetMyDraftsAsync(uint teacherId)
        {
            return await _context.RetakeDirections
                .Include(rd => rd.GroupDiscipline)
                    .ThenInclude(gd => gd.Discipline)
                .Include(rd => rd.GroupDiscipline.Teacher)
                .Include(rd => rd.GroupDiscipline.Group)
                .Include(rd => rd.GroupDiscipline.AttestType)
                .Include(rd => rd.RetakeDirectionStudents)
                    .ThenInclude(rds => rds.Student)
                .Where(rd => rd.CreatedById == teacherId && rd.Status == "draft")
                .OrderByDescending(rd => rd.CreatedAt)
                .ToListAsync();
        }

        public async Task<RetakeDirection?> GetByIdWithIncludesAsync(uint id)
        {
            return await _context.RetakeDirections
                .Include(rd => rd.GroupDiscipline)
                    .ThenInclude(gd => gd.Discipline)
                .Include(rd => rd.GroupDiscipline.Teacher)
                .Include(rd => rd.GroupDiscipline.Group)
                .Include(rd => rd.GroupDiscipline.AttestType)
                .Include(rd => rd.RetakeDirectionStudents)
                    .ThenInclude(rds => rds.Student)
                .FirstOrDefaultAsync(rd => rd.Id == id);
        }

        public async Task<uint> CreateAsync(RetakeDirection direction)
        {
            _context.RetakeDirections.Add(direction);
            await _context.SaveChangesAsync();
            return direction.Id;
        }

        public async Task UpdateNumberAsync(uint id, string number)
        {
            var direction = await _context.RetakeDirections.FindAsync(id);
            if (direction != null)
            {
                direction.Number = number;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Group>> GetGroupsAsync()
        {
            return await _context.Groups
                .Where(g => g.Students.Any(s => s.IsActive))
                .ToListAsync();
        }

        public async Task<List<Student>> GetGroupStudentsAsync(uint groupId)
        {
            return await _context.Students
                .Where(s => s.GroupId == groupId && s.IsActive)
                .ToListAsync();
        }

        public async Task<GroupDiscipline?> GetGroupDisciplineAsync(uint disciplineId, uint groupId, uint attestTypeId, int semester, string studyYear)
        {
            return await _context.GroupDisciplines
                .FirstOrDefaultAsync(gd => gd.DisciplineId == disciplineId &&
                                           gd.GroupId == groupId &&
                                           gd.AttestTypeId == attestTypeId &&
                                           gd.Semester == semester &&
                                           gd.StudyYear == studyYear);
        }

        public async Task<GroupDiscipline> CreateGroupDisciplineAsync(GroupDiscipline groupDiscipline)
        {
            _context.GroupDisciplines.Add(groupDiscipline);
            await _context.SaveChangesAsync();
            return groupDiscipline;
        }
    }
}
