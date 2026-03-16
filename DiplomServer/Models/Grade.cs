namespace DiplomServer.Models
{
    public class Grade
    {
        public uint Id { get; set; }
        public uint StudentId { get; set; }
        public uint GroupDisciplineId { get; set; }
        public int GradeValue { get; set; }   // 2..5
        public bool? IsPassed { get; set; }
        public DateTime GradeDate { get; set; }

        public Student Student { get; set; } = null!;
        public GroupDiscipline GroupDiscipline { get; set; } = null!;
    }
}
