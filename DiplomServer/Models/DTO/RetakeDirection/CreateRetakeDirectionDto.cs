using System.ComponentModel.DataAnnotations;

namespace DiplomServer.Models.DTO.RetakeDirection
{
    public class CreateRetakeDirectionRequestDto
    {
        [Required] public uint GroupDisciplineId { get; set; }
        [MinLength(1)] public List<uint> StudentIds { get; set; } = new();
        public DateTime? RetakeDate { get; set; }
    }
}
