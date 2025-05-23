namespace ServiceContracts.DTOs.Payment
{
    public class CallBackRequest
    {
        public int TransactionId { get; set; }
        // obj.success
        public bool IsSuccessful { get; set; }
        // obj.created_at
        public DateTime PaidAt { get; set; }
        public int OrderId { get; set; }
    }

}
