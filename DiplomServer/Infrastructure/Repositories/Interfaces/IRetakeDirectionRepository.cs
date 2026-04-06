using DiplomServer.Domain.Entities;

namespace DiplomServer.Infrastructure.Repositories.Interfaces
{
    public interface IRetakeDirectionRepository
    {
        Task<List<RetakeDirection>> GetMyDraftsAsync(uint userId);
        Task<RetakeDirection?> GetByIdWithIncludesAsync(uint id);
        Task<uint> CreateAsync(RetakeDirection direction);
        Task UpdateAsync(RetakeDirection direction);
        Task<uint> GetOrCreateGroupDisciplineIdAsync(
            uint groupId,
            uint disciplineId,
            uint attestTypeId,
            int semester,
            string studyYear);
        
        Task RemoveStudentsAsync(uint directionId);
        Task AddStudentsAsync(IEnumerable<RetakeDirectionStudent> students);
    }
}