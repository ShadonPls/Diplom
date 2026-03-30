namespace DiplomServer.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public CurrentUserDto User { get; set; } = new();
    }
}