using DiplomServer.Application.DTOs.Common;

namespace DiplomServer.Application.DTOs.Orders
{
    public class OrdersResponseDto
    {
        public uint Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public DateOnly DateCreate { get; set; }
        public StudentTypeDto StudentId { get; set; }
        public TypeDto DisciplineId { get; set; }
        public uint CreatedById { get; set; }
        public DateTime DateReceptionTeacher { get; set; }
        public DateTime DateReceptionCommission { get; set; }

        public List<ReceptionDto> ReceptionTeachers { get; set; } = new();
        public List<ReceptionDto> ReceptionCommissions { get; set; } = new();
    }
}
