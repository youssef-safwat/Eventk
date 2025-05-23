using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace ServiceContracts.DTOs.Dashboard
{
    public class EventDashboardEditRequest
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public string Description { get; set; }
        public IFormFile? EventPictureFile { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<int> CategoryIds { get; set; } = new List<int>();
    }
}