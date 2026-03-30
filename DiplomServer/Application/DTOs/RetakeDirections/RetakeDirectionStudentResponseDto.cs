namespace DiplomServer.Application.DTOs.RetakeDirections
{
    public class RetakeDirectionStudentResponseDto
    {
        public uint StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int GradeValue { get; set; }
        public bool IsPassed { get; set; }
        public DateTime GradeDate { get; set; }
    }
}