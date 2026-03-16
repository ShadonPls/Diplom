using System.ComponentModel.DataAnnotations;

namespace DiplomServer.Models.DTO.RetakeDirection
{
    public class CreateRetakeDirectionFormDto
    {
        [Required] public uint AttestTypeId { get; set; }
        [Required] public string TeacherFullName { get; set; } = null!;
        [Required] public uint DisciplineId { get; set; }
        [Range(1, 10)] public int Semester { get; set; }
        [Required] public string StudyYear { get; set; } = null!;
        [Required] public uint GroupId { get; set; }
        public List<StudentGradeDto> Students { get; set; } = new();
        public DateTime? RetakeDate { get; set; }
    }
    public class StudentGradeDto
    {
        [Required] public uint StudentId { get; set; }
        [Range(2, 5)] public int GradeValue { get; set; } = 2;
        public DateTime GradeDate { get; set; } = DateTime.Now;
    }
}
