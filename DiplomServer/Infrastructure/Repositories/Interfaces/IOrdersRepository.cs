using DiplomServer.Application.DTOs.Common;
using DiplomServer.Application.DTOs.Orders;
using DiplomServer.Domain.Entities;

namespace DiplomServer.Infrastructure.Repositories.Interfaces
{
    public interface IOrdersRepository
    {
        Task<List<Orders>> GetAllAsync(uint userId);
        Task<Orders?> GetByIdAsync(uint id);
        Task<uint> CreateAsync(Orders entity);
        Task CreateReceptionAsync(IEnumerable<ReceptionTeacher> receptionTeacher, IEnumerable<ReceptionCommission> receptionCommission);
        Task UpdateAsync(Orders entity);
        void UpdateReceptionCommissions(ICollection<ReceptionCommission> commissions, List<TeacherDto> dtoCommissions);
        void UpdateReceptionTeachers(ICollection<ReceptionTeacher> teachers, List<TeacherDto> dtoTeachers);
        Task DeleteAsync(uint id);
    }
}