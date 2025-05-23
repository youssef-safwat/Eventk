namespace ServiceContracts.DTOs
{
    public class TicketTypeResponse
    {
        public int TicketTypeId { get; set; }
        public string TicketName { get; set; } = string.Empty;
        public double Price { get; set; }
        public int AvailableTickets { get; set; } 
    }
}
