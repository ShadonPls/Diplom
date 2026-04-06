namespace DiplomServer.Domain.Entities
{
    public class RetakeDirectionStudent
    {
        public uint Id { get; set; }

        public uint RetakeDirectionId { get; set; }
        public uint StudentId { get; set; }

        public int RetakeGradeValue { get; set; }
        public bool RetakeIsPassed { get; set; }
        public DateTime RetakeGradeDate { get; set; }

        public RetakeDirection RetakeDirection { get; set; } = null!;
    }
}