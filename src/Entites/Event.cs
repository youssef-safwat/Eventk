
using NetTopologySuite.Geometries;

namespace Entites;

public class Event
{
    public int EventId { get; set; }
    public string? EventName { get; set; }
    public string? Description { get; set; }
    public Uri? EventPicture { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Status { get; set; }
    public DateTime CreateAt { get; set; }
    public Point? Location { get; set; } // Spatial point for latitude and longitude
    public Organization Organization { get; set; }  // nav prop 
    public int OrganizationID { get; set; } // fk 
    public ICollection<EventCategory> EventCategory { get; set; } = new List<EventCategory>();

    public ICollection<Interests> Interests { get; set; } = new List<Interests>();
    public ICollection<TicketType> TicketTypes { get; set; } = new List<TicketType>();

}
