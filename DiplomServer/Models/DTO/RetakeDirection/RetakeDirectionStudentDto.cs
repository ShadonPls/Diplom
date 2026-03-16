namespace DiplomServer.Models.DTO.RetakeDirection
{
    public class RetakeDirectionStudentDto
    {
        public string FullName { get; set; } = null!;
        public int RetakeGradeValue { get; set; }
        public DateTime RetakeGradeDate { get; set; }
    }
}
