namespace DiplomServer.Domain.Entities
{
    public class Orders
    {
        public uint Id { get; set; }
        public string Number { get; set; }
        public DateOnly DateCreate { get; set; }
        public uint StudentId { get; set; }
        public uint DisciplineId { get; set; }
        public uint CreatedById { get; set; }
        public DateTime DateReceptionTeacher { get; set; }
        public DateTime DateReceptionCommission { get; set; }
        public ICollection<ReceptionTeacher> ReceptionTeachers { get; set; } = new List<ReceptionTeacher>();
        public ICollection<ReceptionCommission> ReceptionCommissions { get; set; } = new List<ReceptionCommission>();
    }
}
