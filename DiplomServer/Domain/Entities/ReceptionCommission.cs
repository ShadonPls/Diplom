namespace DiplomServer.Domain.Entities
{
    public class ReceptionCommission
    {
        public uint Id { get; set; }
        public uint OrderId { get; set; }
        public uint TeacherId { get; set; }
        public Orders Order { get; set; }
    }
}
