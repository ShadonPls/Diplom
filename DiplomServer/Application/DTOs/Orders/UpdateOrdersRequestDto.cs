using DiplomServer.Application.DTOs.Common;

namespace DiplomServer.Application.DTOs.Orders
{
    public class UpdateOrdersRequestDto
    {
        public string? Number { get; set; }
        public DateOnly? DateCreate { get; set; }
        public uint? StudentId { get; set; }
        public uint? DisciplineId { get; set; }
        public DateTime? DateReceptionTeacher { get; set; }
        public DateTime? DateReceptionCommission { get; set; }
        public List<TeacherDto> ReceptionTeachers { get; set; } = new();
        public List<TeacherDto> ReceptionCommission { get; set; } = new();
    }
}
