using ServiceContracts.DTOs;
using ServiceContracts.DTOs.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.ServicesContracts
{
    public interface IPaymentService
    {
        Task<ServiceResponse<string>> PaymentIntent(PaymentRequest paymentRequest, string userId);
        Task<ServiceResponse<Response>> PaymentRefund(RefundRequest request, string userId);
        Task HandleCallBack(CallBackRequest request);
    }
}
