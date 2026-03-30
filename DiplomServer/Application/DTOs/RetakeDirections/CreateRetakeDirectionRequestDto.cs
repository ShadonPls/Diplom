namespace DiplomServer.Application.DTOs.RetakeDirections
{
    public class CreateRetakeDirectionRequestDto
    {
        public uint GroupId { get; set; }
        public uint DisciplineId { get; set; }
        public uint AttestTypeId { get; set; }
        public int Semester { get; set; }
        public string StudyYear { get; set; } = string.Empty;
        public DateTime? RetakeDate { get; set; }
        public List<RetakeDirectionStudentRequestDto> Students { get; set; } = new();
    }
}