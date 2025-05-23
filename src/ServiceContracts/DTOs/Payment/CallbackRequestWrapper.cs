using System;
using Newtonsoft.Json;

namespace ServiceContracts.DTOs.Payment
{
    // Top-level wrapper matching the JSON structure
    public class CallbackWrapper
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("obj")]
        public TransactionObj Obj { get; set; }
    }

    // Represents the "obj" section
    public class TransactionObj
    {
        [JsonProperty("id")]
        public int TransactionId { get; set; }

        [JsonProperty("pending")]
        public bool Pending { get; set; }

        [JsonProperty("amount_cents")]
        public int AmountCents { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("is_auth")]
        public bool IsAuth { get; set; }

        [JsonProperty("is_capture")]
        public bool IsCapture { get; set; }

        [JsonProperty("is_standalone_payment")]
        public bool IsStandalonePayment { get; set; }

        [JsonProperty("is_voided")]
        public bool IsVoided { get; set; }

        [JsonProperty("is_refunded")]
        public bool IsRefunded { get; set; }

        [JsonProperty("is_3d_secure")]
        public bool Is3DSecure { get; set; }

        [JsonProperty("integration_id")]
        public int IntegrationId { get; set; }

        [JsonProperty("profile_id")]
        public int ProfileId { get; set; }

        [JsonProperty("has_parent_transaction")]
        public bool HasParentTransaction { get; set; }

        [JsonProperty("order")]
        public OrderRef Order { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("source_data")]
        public SourceData SourceData { get; set; }

        [JsonProperty("api_source")]
        public string ApiSource { get; set; }

        [JsonProperty("terminal_id")]
        public int? TerminalId { get; set; }

        [JsonProperty("merchant_commission")]
        public decimal MerchantCommission { get; set; }

        [JsonProperty("installment")]
        public object Installment { get; set; }

        [JsonProperty("discount_details")]
        public object[] DiscountDetails { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }

        [JsonProperty("error_occured")]
        public bool ErrorOccured { get; set; }

        [JsonProperty("transaction_processed_callback_responses")]
        public object[] TransactionProcessedCallbackResponses { get; set; }

        [JsonProperty("is_hidden")]
        public bool IsHidden { get; set; }

        [JsonProperty("refunded_amount_cents")]
        public int RefundedAmountCents { get; set; }

        [JsonProperty("source_id")]
        public int SourceId { get; set; }

        [JsonProperty("is_captured")]
        public bool IsCaptured { get; set; }

        [JsonProperty("captured_amount")]
        public int CapturedAmount { get; set; }

        [JsonProperty("merchant_staff_tag")]
        public object MerchantStaffTag { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("is_settled")]
        public bool IsSettled { get; set; }

        [JsonProperty("bill_balanced")]
        public bool BillBalanced { get; set; }

        [JsonProperty("is_bill")]
        public bool IsBill { get; set; }

        [JsonProperty("owner")]
        public int Owner { get; set; }

        [JsonProperty("parent_transaction")]
        public object ParentTransaction { get; set; }

        [JsonProperty("payment_key_claims")]
        public PaymentKeyClaims PaymentKeyClaims { get; set; }
    }

    // Represents the "order" object with only the id
    public class OrderRef
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }

    // Represents "source_data" section
    public class SourceData
    {
        [JsonProperty("pan")]
        public string Pan { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("sub_type")]
        public string SubType { get; set; }
    }

    // Re-add payment_key_claims
    public class PaymentKeyClaims
    {
        [JsonProperty("user_id")]
        public int UserId { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("order_id")]
        public int OrderId { get; set; }

        [JsonProperty("amount_cents")]
        public int AmountCents { get; set; }
    }
}
