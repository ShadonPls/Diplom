using DiplomServer.Application.DTOs.Common;
using DiplomServer.Application.DTOs.RetakeDirections;

namespace DiplomServer.Application.DTOs.Orders
{
    public class CreateOrdersRequestDto
    {
        public string Number { get; set; } = string.Empty;
        public DateOnly DateCreate { get; set; }
        public uint StudentId { get; set; }
        public uint DisciplineId { get; set; }
        public DateTime DateReceptionTeacher { get; set; }
        public DateTime DateReceptionCommission { get; set; }
        public List<TeacherDto> ReceptionTeachers { get; set; } = new();
        public List<TeacherDto> ReceptionCommission { get; set; } = new();
    }
}
