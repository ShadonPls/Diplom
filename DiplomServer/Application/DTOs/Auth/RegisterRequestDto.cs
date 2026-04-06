using DiplomServer.Domain.Enums;

namespace DiplomServer.Application.DTOs.Auth
{
    public class RegisterRequestDto
    {
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}