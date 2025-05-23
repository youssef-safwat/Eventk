using System.Text.Json.Serialization;
namespace ServiceContracts.DTOs.Event;
public class EventSummaryResponse
{
    public int EventId { get; set; }
    public string? EventName { get; set; }
    public DateTime? StartDate { get; set; }
    public Uri? EventPicture { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsInterested { get; set; } = null;
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
}
