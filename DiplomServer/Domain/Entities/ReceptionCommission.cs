namespace DiplomServer.Domain.Entities
{
    public class ReceptionCommission
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int TeacherId { get; set; }
        public Orders Order { get; set; }
    }
}
