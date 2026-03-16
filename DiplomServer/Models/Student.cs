namespace DiplomServer.Models
{
    public class Student
    {
        public uint Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Surname { get; set; }
        public uint GroupId { get; set; }
        public bool IsActive { get; set; }

        public Group Group { get; set; } = null!;
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
        public ICollection<RetakeDirectionStudent> RetakeDirectionStudents { get; set; } = new List<RetakeDirectionStudent>();
    }
}
