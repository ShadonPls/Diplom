using System.Diagnostics;

namespace DiplomServer.Models
{
    public class GroupDiscipline
    {
        public uint Id { get; set; }
        public uint GroupId { get; set; }
        public uint DisciplineId { get; set; }
        public uint TeacherId { get; set; }
        public uint AttestTypeId { get; set; }
        public int Semester { get; set; }
        public string StudyYear { get; set; } = null!; // "2025/2026"

        public Group Group { get; set; } = null!;
        public Discipline Discipline { get; set; } = null!;
        public Teacher Teacher { get; set; } = null!;
        public AttestType AttestType { get; set; } = null!;
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
        public ICollection<RetakeDirection> RetakeDirections { get; set; } = new List<RetakeDirection>();
    }
}
