using DiplomServer.Models;

namespace DiplomServer.Interfaces.Repository
{
    public interface IRetakeDirectionRepository
    {
        Task<List<RetakeDirection>> GetMyDraftsAsync(uint teacherId);
        Task<RetakeDirection?> GetByIdWithIncludesAsync(uint id);
        Task<uint> CreateAsync(RetakeDirection direction);
        Task UpdateNumberAsync(uint id, string number);
        Task<List<Group>> GetGroupsAsync();
        Task<List<Student>> GetGroupStudentsAsync(uint groupId);
        Task<GroupDiscipline?> GetGroupDisciplineAsync(uint disciplineId, uint groupId, uint attestTypeId, int semester, string studyYear);
        Task<GroupDiscipline> CreateGroupDisciplineAsync(GroupDiscipline groupDiscipline);
    }
}
