namespace ServiceContracts.DTOs
{
    public class TicketResponse
    {
        public Guid Code { get; set; }
        public string Status { get; set; }  // Active, Used, Refunded, Cancelled
    }
}