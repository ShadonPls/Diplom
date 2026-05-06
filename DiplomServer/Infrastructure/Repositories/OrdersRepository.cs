using DiplomServer.Application.DTOs.Common;
using DiplomServer.Application.DTOs.Orders;
using DiplomServer.Domain.Entities;
using DiplomServer.Infrastructure.Data;
using DiplomServer.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DiplomServer.Infrastructure.Repositories
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly AppDbContext _context;

        public OrdersRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Orders>> GetAllAsync(uint userId)
        {
            return await _context.Orders
                .Include(o => o.ReceptionTeachers)
                .Include(o => o.ReceptionCommissions)
                .Where(rd => rd.CreatedById == userId)
                .ToListAsync();
        }

        public async Task<Orders?> GetByIdAsync(uint id)
        {
            return await _context.Orders
                .Include(o => o.ReceptionTeachers)
                .Include(o => o.ReceptionCommissions)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<uint> CreateAsync(Orders entity)
        {
            _context.Orders.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task CreateReceptionAsync(IEnumerable<ReceptionTeacher> receptionTeacher, IEnumerable<ReceptionCommission> receptionCommission)
        {
            _context.ReceptionCommissions.AddRangeAsync(receptionCommission);
            _context.ReceptionTeachers.AddRangeAsync(receptionTeacher);
            await _context.SaveChangesAsync();
        } 

        public async Task UpdateAsync(Orders entity)
        {
            _context.Orders.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(uint id)
        {
            var entity = await _context.Orders
                .Include(o => o.ReceptionTeachers)
                .Include(o => o.ReceptionCommissions)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (entity != null)
            {
                _context.Orders.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public void UpdateReceptionTeachers(ICollection<ReceptionTeacher> teachers, List<TeacherDto> dtoTeachers)
        {
            int targetCount = dtoTeachers.Count;

            while (teachers.Count > targetCount)
            {
                teachers.Remove(teachers.Last());
            }

            while (teachers.Count < targetCount)
            {
                int index = teachers.Count;
                teachers.Add(new ReceptionTeacher
                {
                    TeacherId = dtoTeachers[index].TeacherId,
                });
            }

            for (int i = 0; i < targetCount; i++)
            {
                teachers.ElementAt(i).TeacherId = dtoTeachers[i].TeacherId;
            }
        }

        public void UpdateReceptionCommissions(ICollection<ReceptionCommission> commissions, List<TeacherDto> dtoCommissions)
        {
            int targetCount = dtoCommissions.Count;

            while (commissions.Count > targetCount)
            {
                commissions.Remove(commissions.Last());
            }

            while (commissions.Count < targetCount)
            {
                int index = commissions.Count;
                commissions.Add(new ReceptionCommission
                {
                    TeacherId = dtoCommissions[index].TeacherId
                });
            }

            for (int i = 0; i < targetCount; i++)
            {
                commissions.ElementAt(i).TeacherId = dtoCommissions[i].TeacherId;
            }
        }
    }
}