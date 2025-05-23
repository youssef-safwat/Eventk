using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTOs.Payment
{
    public class PaymentRefundRequest
    {
        public string transaction_id { get; set; }
        public string amount_cents { get; set; }
        
    }
}
