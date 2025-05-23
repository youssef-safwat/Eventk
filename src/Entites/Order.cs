using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entites
{
    public class Order
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }      // when “Reserve & Pay” clicked
        public DateTime PaidAt { get; set; }
        public DateTime ExpiresAt { get; set; }      // = CreatedAt + TTL (e.g. 15m)
        public OrderStatus Status { get; set; }      // Reserved, Paid, Expired
        public double TotalAmount { get; set; }
        public int? PaymentOrderId { get; set; }      // Paymob transaction id
        public int PaymentTransactionId { get; set; }
        public ICollection<OrderItem> Items { get; set; }
        public ApplicationUser User { get; set; }
        // … no separate Payment entity needed …
    }
    public enum OrderStatus
    {
        Reserved,     // tickets held; awaiting payment
        Paid,         // paid & tickets issued
        Expired       // reservation timed‑out
    }
}
