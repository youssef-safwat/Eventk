namespace ServiceContracts.DTOs
{
    public class OrderDetailsResponse
    {
        public string TicketTypeName { get; set; }  // FK → TicketType
        public string TicketTypeDetails { get; set; }
        public int Quantity { get; set; }  // how many of this type
        public double UnitPrice { get; set; }  // snapshot of TicketType.Price at purchase
        public bool IsRefunded { get; set; }  // true if all tickets refunded
        public int OrderItemId { get; set; }
        public IEnumerable<TicketResponse> Tickets { get; set; } = new List<TicketResponse>();
    }
}