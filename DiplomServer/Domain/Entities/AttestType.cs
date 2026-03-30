namespace DiplomServer.Domain.Entities
{
    public class AttestType
    {
        public uint Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<GroupDiscipline> GroupDisciplines { get; set; } = new List<GroupDiscipline>();
    }
}