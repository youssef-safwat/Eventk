using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTOs.Payment
{
    public class PaymentIntentionReponse
    {
        public string client_secret { get; set; }
        public int intention_order_id { get; set; }
        // Add other fields if returned by Paymob
    }
}
