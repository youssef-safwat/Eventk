using Newtonsoft.Json;
using ServiceContracts.DTOs.Payment;
using ServiceContracts.DTOs;
using System.Security.Cryptography;
using System.Text;

public class PaymobProcessedHmacVerifier
{
    private readonly string _secret;
    private static readonly string[] HmacKeys = new string[]
    {
        "amount_cents", "created_at", "currency", "error_occured", "has_parent_transaction", "obj.id",
        "integration_id", "is_3d_secure", "is_auth", "is_capture", "is_refunded", "is_standalone_payment",
        "is_voided", "order.id", "owner", "pending", "source_data.pan", "source_data.sub_type",
        "source_data.type", "success"
    };

    public PaymobProcessedHmacVerifier(string secret)
    {
        _secret = secret ?? throw new ArgumentNullException(nameof(secret));
    }

    public CallBackRequest? VerifyHmac(CallbackWrapper? callbackWrapper, string hmac)
    {
        if (string.IsNullOrEmpty(hmac))
        {
            return null;
        }

        // Read JSON payload from request body


        // Sort keys and concatenate values
        var concatenated = string.Join("", HmacKeys.Select(key => GetValue(callbackWrapper, key)));

        // Calculate HMAC-SHA512
        var computedHmac = ComputeHmac(concatenated);

        // Compare HMACs
        if (!string.Equals(hmac, computedHmac, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        // Map CallbackWrapper to PaymentCallbackRequest
        return MapToPaymentCallbackRequest(callbackWrapper);
    }

    private string GetValue(CallbackWrapper wrapper, string key)
    {
        var obj = wrapper.Obj;
        switch (key)
        {
            case "obj.id": return obj.TransactionId.ToString();
            case "amount_cents": return obj.AmountCents.ToString();
            case "success": return obj.Success.ToString().ToLower();
            case "created_at": return obj.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.ffffff");
            case "currency": return obj.Currency ?? ""; // Optional property
            case "error_occured": return obj.ErrorOccured.ToString().ToLower(); // Optional property
            case "has_parent_transaction": return obj.HasParentTransaction.ToString().ToLower(); // Optional property
            case "integration_id": return obj.IntegrationId.ToString(); // Optional property
            case "is_3d_secure": return obj.Is3DSecure.ToString().ToLower(); // Optional property
            case "is_auth": return obj.IsAuth.ToString().ToLower(); // Optional property
            case "is_capture": return obj.IsCapture.ToString().ToLower(); // Optional property
            case "is_refunded": return obj.IsRefunded.ToString().ToLower(); // Optional property
            case "is_standalone_payment": return obj.IsStandalonePayment.ToString().ToLower(); // Optional property
            case "is_voided": return obj.IsVoided.ToString().ToLower(); // Optional property
            case "order.id": return obj.Order.Id.ToString(); // Optional property
            case "owner": return obj.Owner.ToString(); // Optional property
            case "pending": return obj.Pending.ToString().ToLower(); // Optional property
            case "source_data.pan": return obj.SourceData.Pan.ToString() ?? ""; // Optional property
            case "source_data.sub_type": return obj.SourceData.SubType ?? ""; // Optional property
            case "source_data.type": return obj.SourceData.Type ?? ""; // Optional property
            default: return "";
        }
    }

    private string ComputeHmac(string data)
    {
        using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_secret)))
        {
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }

    private CallBackRequest MapToPaymentCallbackRequest(CallbackWrapper wrapper)
    {
        var obj = wrapper.Obj;
        return new CallBackRequest
        {
            TransactionId = obj.TransactionId,
            IsSuccessful = obj.Success,
            PaidAt = obj.CreatedAt,
            OrderId = obj.Order.Id,
        };
    }
}