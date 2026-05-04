using DiplomServer.Application.Interfaces.Mappers;

namespace DiplomServer.Application.DTOs.Common
{
    public class TypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public static TypeDto From(dynamic entity) => new() { Id = (int)entity.Id, Name = entity.Name };
    }
}

