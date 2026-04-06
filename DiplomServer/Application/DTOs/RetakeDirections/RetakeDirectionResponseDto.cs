namespace DiplomServer.Application.DTOs.RetakeDirections
{
    public class RetakeDirectionResponseDto
    {
        public uint Id { get; set; }
        public string Number { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime RetakeDate { get; set; }
        public uint GroupId { get; set; }
        public uint DisciplineId { get; set; }
        public uint AttestTypeId { get; set; }
        public int Semester { get; set; }
        public string StudyYear { get; set; } = "";
    }
}