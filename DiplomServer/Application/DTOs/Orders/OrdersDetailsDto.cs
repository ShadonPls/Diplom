using DiplomServer.Application.DTOs.Common;

namespace DiplomServer.Application.DTOs.Orders
{
    public class OrdersDetailsDto
    {
        public uint Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public DateOnly DateCreate { get; set; }
        public TypeDto StudentId { get; set; }
        public TypeDto DisciplineId { get; set; }
        public uint CreatedById { get; set; }
        public DateTime DateReceptionTeacher { get; set; }
        public DateTime DateReceptionCommission { get; set; }
    }
}
