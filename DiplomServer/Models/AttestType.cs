namespace DiplomServer.Models
{
    public class AttestType
    {
        public uint Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<GroupDiscipline> GroupDisciplines { get; set; } = new List<GroupDiscipline>();
    }

}
