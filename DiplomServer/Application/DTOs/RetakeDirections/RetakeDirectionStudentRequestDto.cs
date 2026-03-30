namespace DiplomServer.Application.DTOs.RetakeDirections
{
    public class RetakeDirectionStudentRequestDto
    {
        public uint StudentId { get; set; }
        public int GradeValue { get; set; }
        public DateTime GradeDate { get; set; }
    }
}