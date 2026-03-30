namespace DiplomServer.Domain.Entities
{
    public class Student
    {
        public uint Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public uint GroupId { get; set; }
        public bool IsActive { get; set; } = true;

        public Group Group { get; set; } = null!;

        public ICollection<RetakeDirectionStudent> RetakeDirectionStudents { get; set; } = new List<RetakeDirectionStudent>();
    }
}