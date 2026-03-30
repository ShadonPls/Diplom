namespace DiplomServer.Domain.Entities
{
    public class User
    {
        public uint Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public uint? TeacherId { get; set; }
        public string Role { get; set; } = string.Empty;

        public Teacher? Teacher { get; set; }

        public ICollection<RetakeDirection> RetakeDirectionsCreated { get; set; } = new List<RetakeDirection>();
    }
}