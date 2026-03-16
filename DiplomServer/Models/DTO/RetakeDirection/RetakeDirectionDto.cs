namespace DiplomServer.Models.DTO.RetakeDirection
{
    public class RetakeDirectionResponseDto
    {
        public uint Id { get; set; }
        public string Number { get; set; } = null!;
        public string DisciplineName { get; set; } = null!;
        public string TeacherFullName { get; set; } = null!;
        public string GroupName { get; set; } = null!;
        public string AttestTypeName { get; set; } = null!;
        public string StudyYear { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? RetakeDate { get; set; }
        public string Status { get; set; } = null!;
        public List<RetakeDirectionStudentDto> Students { get; set; } = new();
    }

    
}
