using DiplomServer.Domain.Entities;

namespace DiplomServer.Infrastructure.Repositories.Interfaces
{
    public interface IRetakeDirectionRepository
    {
        Task<List<RetakeDirection>> GetMyDraftsAsync(uint userId);  // ← ИСПРАВЛЕНО: userId
        Task<RetakeDirection?> GetByIdWithIncludesAsync(uint id);
        Task<uint> CreateAsync(RetakeDirection direction);
        Task UpdateAsync(RetakeDirection direction);
        Task<uint> GetOrCreateGroupDisciplineIdAsync(
            uint groupId,
            uint disciplineId,
            uint teacherId,
            uint attestTypeId,
            int semester,
            string studyYear);

        Task<List<Group>> GetGroupsAsync();
        Task<List<Student>> GetGroupStudentsAsync(uint groupId);
        Task<List<Discipline>> GetTeacherDisciplinesAsync(uint teacherId);

        Task RemoveStudentsAsync(uint directionId);
        Task AddStudentsAsync(IEnumerable<RetakeDirectionStudent> students);
    }
}