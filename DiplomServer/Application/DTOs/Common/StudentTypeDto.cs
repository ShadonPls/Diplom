namespace DiplomServer.Application.DTOs.Common
{
    public class StudentTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Group { get; set; } = "";
        public static StudentTypeDto From(dynamic entity) => new() { Id = (int)entity.Id, Name = entity.Name, Group = entity.Group};
    }
}
