namespace DiplomServer.Models
{
    public class Teacher
    {
        public uint Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Surname { get; set; }
        public ICollection<GroupDiscipline> GroupDisciplines { get; set; } = new List<GroupDiscipline>();
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
