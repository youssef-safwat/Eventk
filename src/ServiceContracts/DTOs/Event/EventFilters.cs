using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTOs.Event
{
    public class EventFilters
    {
        public string? Name { get; set; }
        public IEnumerable<int>? CategoriesIds { get; set; }
        public double? Latitude { get; set; } 
        public double? Longitude { get; set; }
        public int Radius { get; set; } = 1000;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? PageNumber { get; set; } = 0;
        public int? PageSize { get; set; } = 20;
        public string? SortBy { get; set; }
        public bool IsAscending { get; set; } = true;
        public bool? IsPaid { get; set; }
        public double? MinPrice { get; set; } = 0;
        public double? MaxPrice { get; set; } = 1000000;
    }
}

