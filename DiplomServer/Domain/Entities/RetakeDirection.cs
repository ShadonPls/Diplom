namespace DiplomServer.Domain.Entities
{
    public class RetakeDirection
    {
        public uint Id { get; set; }
        public string? Number { get; set; }

        public uint GroupDisciplineId { get; set; }
        public uint CreatedById { get; set; }

        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime RetakeDate { get; set; }

        public GroupDiscipline GroupDiscipline { get; set; } = null!;
        public User CreatedBy { get; set; } = null!;

        public ICollection<RetakeDirectionStudent> RetakeDirectionStudents { get; set; } = new List<RetakeDirectionStudent>();
    }
}