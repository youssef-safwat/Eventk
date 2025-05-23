using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTOs.Payment
{
    public class PaymentIntentionRequest
    {
        public double amount { get; set; }
        public string currency { get; set; } = "EGP";
        public IEnumerable<int> payment_methods { get; set; } = [5076709];
        public IEnumerable<Item> items { get; set; } = new List<Item>();
        public BillingData billing_data { get; set; }
    }

    public class TicketQuantity
    {
        public int Quantity { get; set; }
        public int TicketTypeId { get; set; }
    }

    public class BillingData
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
    }

    public class Item
    {
        public string name { get; set; }
        public double amount { get; set; }
        public string description { get; set; }
        public int quantity { get; set; }
    }
}
