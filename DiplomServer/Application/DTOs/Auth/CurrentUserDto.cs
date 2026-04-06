namespace DiplomServer.Application.DTOs.Auth
{
    public class CurrentUserDto
    {
        public uint Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public uint? TeacherId { get; set; }
    }
}