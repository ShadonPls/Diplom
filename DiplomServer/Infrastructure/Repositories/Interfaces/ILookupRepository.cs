using DiplomServer.Domain.Entities;

namespace DiplomServer.Infrastructure.Repositories.Interfaces
{
    public interface ILookupRepository
    {
        Task<List<Discipline>> GetTeacherDisciplinesAsync(uint teacherId);
        Task<List<AttestType>> GetAttestTypesAsync();
        Task<List<Group>> GetGroupsAsync();
        Task<List<Student>> GetGroupStudentsAsync(uint groupId);
    }
}