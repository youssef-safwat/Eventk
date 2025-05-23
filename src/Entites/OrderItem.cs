namespace Entites
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }  // PK
        public int OrderId { get; set; }  // FK → Order
        public int TicketTypeId { get; set; }  // FK → TicketType
        public int Quantity { get; set; }  // how many of this type
        public double UnitPrice { get; set; }  // snapshot of TicketType.Price at purchase
        public double TotalPrice { get; set; }  // Quantity * UnitPrice
        public OrderItemRefundStatus OrderItemRefundStatus { get; set; }          
        // navigation properties
        public Order Order { get; set; }
        public TicketType TicketType { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public int RefundedQuantity { get; set; }  // updated on each refund
        public double RefundedAmount { get; set; }  // sum of per‑ticket refund amounts
    }
    public enum OrderItemRefundStatus
    {
        None,             // no tickets refunded
        PartiallyRefunded,// some tickets refunded
        Refunded          // all tickets refunded
    }
}
