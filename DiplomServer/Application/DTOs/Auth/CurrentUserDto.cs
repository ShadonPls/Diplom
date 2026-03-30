namespace DiplomServer.Application.DTOs.Auth
{
    public class CurrentUserDto
    {
        public uint Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public uint? TeacherId { get; set; }
    }
}