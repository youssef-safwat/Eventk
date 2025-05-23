using Refit;
using ServiceContracts.DTOs.Payment;

namespace ServiceContracts.Helpers
{ 
    public interface IPaymobApi
    {
        [Post("/v1/intention/")]
        Task<PaymentIntentionReponse> CreatePaymentIntentionAsync([Body] PaymentIntentionRequest request, 
            [Header("Authorization")] string bearerToken);

        [Post("/api/acceptance/void_refund/refund")]
        Task RefundTransactionAsync([Body] PaymentRefundRequest request,[Header("Authorization")] string bearerToken);
    }
}
