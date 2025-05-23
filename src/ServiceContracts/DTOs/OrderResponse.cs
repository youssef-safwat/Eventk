
namespace ServiceContracts.DTOs
{
    public class OrderResponse
    {
        public int OrderId { get; set; }
        public DateTime PaidAt { get; set; }
        public double TotalAmount { get; set; }
        public int EventId { get; set; }
        public string? EventName { get; set; }
        public Uri? EventPicture { get; set; }
    }
}
