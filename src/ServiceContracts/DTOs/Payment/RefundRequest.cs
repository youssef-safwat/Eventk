using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTOs.Payment
{
    /// <summary>
    /// Request DTO for refunding tickets.
    /// </summary>
    public class RefundRequest
    {
        public int OrderId { get; set; }
        public List<RefundLine> Items { get; set; } = new();
    }
    public class RefundLine
    {
        public int OrderItemId { get; set; }
        public int Quantity { get; set; }
    }
}
