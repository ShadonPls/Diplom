using DiplomServer.Domain.Entities;
using DiplomServer.Infrastructure.Data;
using DiplomServer.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DiplomServer.Infrastructure.Repositories
{
    public class LookupRepository : ILookupRepository
    {
        private readonly AppDbContext _context;

        public LookupRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Discipline>> GetTeacherDisciplinesAsync(uint teacherId)
        {
            return await _context.Disciplines
                .Join(_context.GroupDisciplines,
                    d => d.Id,
                    gd => gd.DisciplineId,
                    (d, gd) => new { Discipline = d, GroupDiscipline = gd })
                .Where(x => x.GroupDiscipline.TeacherId == teacherId)
                .Select(x => x.Discipline)
                .Distinct()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<List<AttestType>> GetAttestTypesAsync()
        {
            return await _context.AttestTypes
                .OrderBy(x => x.Name)
                .ToListAsync();
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
    }
}