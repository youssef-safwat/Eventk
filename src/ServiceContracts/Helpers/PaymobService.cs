using Microsoft.Extensions.Configuration;
using ServiceContracts.DTOs.Payment;

namespace ServiceContracts.Helpers
{
    public class PaymobService
    {
        private readonly IPaymobApi _paymobApi;
        private readonly IConfiguration _configuration;

        public PaymobService(IPaymobApi paymobApi , IConfiguration configuration)
        {
            _paymobApi = paymobApi;
            _configuration = configuration;
        }

        public async Task<PaymentIntentionReponse> CreatePaymentIntentionAsync(PaymentIntentionRequest request)
        {
            var response = await _paymobApi.CreatePaymentIntentionAsync(request , _configuration["Paymob:SecretKey"]);
            return response;
        }
        public async Task RefundTransactionAsync(string transactionId, string amountCents)
        {
            var request = new PaymentRefundRequest { transaction_id = transactionId  , amount_cents = amountCents };
            await _paymobApi.RefundTransactionAsync(request, _configuration["Paymob:SecretKey"]);
        }
    }
}
