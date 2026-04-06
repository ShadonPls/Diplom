using DiplomServer.Application.DTOs.Common;

namespace DiplomServer.Application.DTOs.RetakeDirections
{
    public class RetakeDirectionDetailsDto
    {
        public RetakeDirectionResponseDto Direction { get; set; } = null!;
        public TypeDto Group { get; set; } = null!;
        public TypeDto Discipline { get; set; } = null!;
        public TypeDto AttestationType { get; set; } = null!;
        public List<RetakeDirectionStudentResponseDto> Students { get; set; } = new();
    }
}
