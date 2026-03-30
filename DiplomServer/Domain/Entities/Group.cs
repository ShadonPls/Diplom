namespace DiplomServer.Domain.Entities
{
    public class Group
    {
        public uint Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<GroupDiscipline> GroupDisciplines { get; set; } = new List<GroupDiscipline>();
    }
}