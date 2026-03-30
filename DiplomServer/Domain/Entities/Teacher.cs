namespace DiplomServer.Domain.Entities
{
    public class Teacher
    {
        public uint Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;

        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<GroupDiscipline> GroupDisciplines { get; set; } = new List<GroupDiscipline>();
    }
}