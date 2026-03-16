namespace DiplomServer.Models
{
    public class User
    {
        public uint Id { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public uint? TeacherId { get; set; }
        public string Role { get; set; } = null!;

        public Teacher? Teacher { get; set; }
        public ICollection<RetakeDirection> RetakeDirectionsCreated { get; set; } = new List<RetakeDirection>();
    }
}
