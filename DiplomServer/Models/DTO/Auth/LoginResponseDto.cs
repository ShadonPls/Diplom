namespace DiplomServer.Models.DTO.Auth
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = "";
        public string Role { get; set; } = "";
    }
}
