namespace DiplomServer.Application.DTOs.RetakeDirections
{
    public class RetakeDirectionResponseDto
    {
        public uint Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime RetakeDate { get; set; }

        public uint GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;

        public uint DisciplineId { get; set; }
        public string DisciplineName { get; set; } = string.Empty;

        public uint AttestTypeId { get; set; }
        public string AttestTypeName { get; set; } = string.Empty;

        public int Semester { get; set; }
        public string StudyYear { get; set; } = string.Empty;

        public List<RetakeDirectionStudentResponseDto> Students { get; set; } = new();
    }
}