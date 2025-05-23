using System;
using System.Collections.Generic;

namespace ServiceContracts.DTOs.Dashboard
{
    public class EventDashboardResponse
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public string Description { get; set; }
        public Uri EventPicture { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DateTime CreateAt { get; set; }
        public string Location { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int OrganizationID { get; set; }
        public string OrganizationName { get; set; }
        public List<string> Categories { get; set; } = new List<string>();
        public int InterestedCount { get; set; }
    }
}