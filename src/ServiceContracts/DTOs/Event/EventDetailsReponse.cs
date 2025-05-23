using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ServiceContracts.DTOs.Event
{
    public class EventDetailsReponse
    {
        public int EventId { get; set; }
        public About? About { get; set; }
        public string? EventName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Uri? EventPicture { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IsInterested { get; set; } = null;
        public string? OrganizationName { get; set; }
        public int OrganizationId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int InterestedPeople {  get; set; }
        public bool IsPaid {  get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
    }
}
