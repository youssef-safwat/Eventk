namespace ServiceContracts.DTOs.Payment
{
    public class PaymentRequest
    {
        public IEnumerable<Items> Items { get; set; } = new List<Items>();
        public string? PhoneNumber { get; set; }

    }

    public class Items
    {
        public int TicketTypeId { get; set; }
        public int Quantity { get; set; }
    }
}
