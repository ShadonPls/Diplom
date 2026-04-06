namespace DiplomServer.Domain.Entities
{
    public class GroupDiscipline
    {
        public uint Id { get; set; }

        public uint GroupId { get; set; }
        public uint DisciplineId { get; set; }
        public uint AttestTypeId { get; set; }

        public int Semester { get; set; }
        public string StudyYear { get; set; } = string.Empty;

        public ICollection<RetakeDirection> RetakeDirections { get; set; } = new List<RetakeDirection>();
    }
}