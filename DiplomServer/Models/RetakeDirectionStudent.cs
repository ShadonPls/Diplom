namespace DiplomServer.Models
{
    public class RetakeDirectionStudent
    {
        public uint Id { get; set; }
        public uint RetakeDirectionId { get; set; }
        public uint StudentId { get; set; }
        public int RetakeGradeValue { get; set; }  // 2..5, для зачёта 5
        public bool RetakeIsPassed { get; set; }
        public DateTime RetakeGradeDate { get; set; }

        public RetakeDirection RetakeDirection { get; set; } = null!;
        public Student Student { get; set; } = null!;
    }
}
