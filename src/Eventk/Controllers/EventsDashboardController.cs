using Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using ServiceContracts.DTOs.Dashboard;
using ServiceContracts.Helpers;
using ServiceContracts.ServicesContracts;
using System.Security.Claims;

namespace Eventk.Controllers
{
    [Authorize(Roles = "SuperAdmin,OrganizationAdmin")]
    public class EventsDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ICloudinaryService _cloudinaryService;

        public EventsDashboardController(ApplicationDbContext context, IConfiguration configuration , ICloudinaryService cloudinaryService)
        {
            _context = context;
            _configuration = configuration;
            _cloudinaryService = cloudinaryService;
        }
        
        private async Task<bool> IsOrganizationProfileComplete()
        {
            if (!User.IsInRole("OrganizationAdmin")) return true;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var organization = await _context.Organization.FirstOrDefaultAsync(o => o.UserId == userId);
            return organization != null && !string.IsNullOrWhiteSpace(organization.Name) && !string.IsNullOrWhiteSpace(organization.Description);
        }

        private async Task<IActionResult> RedirectIfProfileIncomplete()
        {
            if (!await IsOrganizationProfileComplete())
                return RedirectToAction("Edit", "OrganizationDashboard");
            return null;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var redirect = await RedirectIfProfileIncomplete();
            if (redirect != null) return redirect;
            ViewBag.IsProfileComplete = redirect == null ? true : false;
            var eventsQuery = _context.Events.AsQueryable();
            
            if (User.IsInRole("OrganizationAdmin"))
            {
                // Organization admin can only see their own events
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var organization = await _context.Organization
                    .FirstOrDefaultAsync(o => o.UserId == userId);
                
                if (organization != null)
                {
                    eventsQuery = eventsQuery.Where(e => e.OrganizationID == organization.OrganizationId);
                }
                else
                {
                    return View(new List<EventDashboardResponse>());
                }
            }
            
            // Include related data
            var events = await eventsQuery
                .Include(e => e.Organization)
                .Include(e => e.EventCategory)
                    .ThenInclude(ec => ec.Category)
                .Include(e => e.Interests)
                .Select(e => new EventDashboardResponse
                {
                    EventId = e.EventId,
                    EventName = e.EventName,
                    Description = e.Description,
                    EventPicture = e.EventPicture,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    CreateAt = e.CreateAt,
                    Location = e.Location != null ? $"({e.Location.X}, {e.Location.Y})" : "No location",
                    OrganizationID = e.OrganizationID,
                    OrganizationName = e.Organization.Name,
                    Categories = e.EventCategory.Select(ec => ec.Category.CategoryName).ToList(),
                    InterestedCount = e.Interests.Count
                })
                .ToListAsync();
            
            return View(events);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
           
            if (id == null)
            {
                return NotFound();
            }

            var eventEntity = await _context.Events
                .Include(e => e.Organization)
                .Include(e => e.EventCategory)
                    .ThenInclude(ec => ec.Category)
                .Include(e => e.Interests)
                .FirstOrDefaultAsync(m => m.EventId == id);
                
            if (eventEntity == null)
            {
                return NotFound();
            }
            
            // Check if organization admin is trying to access an event that doesn't belong to them
            if (User.IsInRole("OrganizationAdmin"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var organization = await _context.Organization
                    .FirstOrDefaultAsync(o => o.UserId == userId);
                
                if (organization == null || eventEntity.OrganizationID != organization.OrganizationId)
                {
                    return Forbid();
                }
            }
            
            var eventResponse = new EventDashboardResponse
            {
                EventId = eventEntity.EventId,
                EventName = eventEntity.EventName,
                Description = eventEntity.Description,
                EventPicture = eventEntity.EventPicture,
                StartDate = eventEntity.StartDate,
                EndDate = eventEntity.EndDate,                CreateAt = eventEntity.CreateAt,
                Location = eventEntity.Location != null ? $"({eventEntity.Location.X}, {eventEntity.Location.Y})" : "No location",
                Latitude = eventEntity.Location?.Y ?? 0,
                Longitude = eventEntity.Location?.X ?? 0,
                OrganizationID = eventEntity.OrganizationID,
                OrganizationName = eventEntity.Organization.Name,
                Categories = eventEntity.EventCategory.Select(ec => ec.Category.CategoryName).ToList(),
                InterestedCount = eventEntity.Interests.Count
            };


            return View(eventResponse);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!User.IsInRole("OrganizationAdmin"))
            {
                return Forbid();
            }
            var redirect = await RedirectIfProfileIncomplete();
            if (redirect != null) return redirect;
            ViewBag.IsProfileComplete = redirect == null ? true : false;
            // Load categories for dropdown
            ViewBag.Categories = _context.Categories.ToList();
            
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(EventDashboardEditRequest eventModel)
        {
            if (!User.IsInRole("OrganizationAdmin"))
            {
                return Forbid();
            }
            
            if (!ModelState.IsValid)
            {
                // Reload categories for dropdown
                ViewBag.Categories = await _context.Categories.ToListAsync();
               
                return View(eventModel);
            }
            
            // Set the organization ID for the event
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var organization = await _context.Organization
                .FirstOrDefaultAsync(o => o.UserId == userId);
            
            if (organization == null)
            {
                return NotFound("Organization not found");
            }
            
            var newEvent = new Event
            {
                EventName = eventModel.EventName,
                Description = eventModel.Description,
                StartDate = eventModel.StartDate,
                EndDate = eventModel.EndDate,
                Location = eventModel.Location != null ? new Point(eventModel.Longitude, eventModel.Latitude) {SRID = 4326} : null,
                CreateAt = DateTime.Now,
                OrganizationID = organization.OrganizationId
            };
            
            
            // Handle image upload
            if (eventModel.EventPictureFile != null)
            {
                newEvent.EventPicture = await _cloudinaryService.UploadImageAsync(eventModel.EventPictureFile);
            }
            
            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();
            
            // Add categories if provided
            if (eventModel.CategoryIds != null && eventModel.CategoryIds.Any())
            {
                foreach (var categoryId in eventModel.CategoryIds)
                {
                    _context.EventCategories.Add(new EventCategory
                    {
                        EventId = newEvent.EventId,
                        CategoryId = categoryId
                    });
                }
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!User.IsInRole("OrganizationAdmin"))
            {
                return Forbid();
            }
            var redirect = await RedirectIfProfileIncomplete();
            if (redirect != null) return redirect;
            ViewBag.IsProfileComplete = redirect == null ? true : false;

            if (id == null)
            {
                return NotFound();
            }

            var eventEntity = await _context.Events
                .Include(e => e.EventCategory)
                .FirstOrDefaultAsync(m => m.EventId == id);
                
            if (eventEntity == null)
            {
                return NotFound();
            }
            
            // Check if the event belongs to the organization admin's organization
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var organization = await _context.Organization
                .FirstOrDefaultAsync(o => o.UserId == userId);
            
            if (organization == null || eventEntity.OrganizationID != organization.OrganizationId)
            {
                return Forbid();
            }
            
            // Load categories for dropdown
            ViewBag.Categories = await _context.Categories.ToListAsync();
            
           
            
            var eventEditRequest = new EventDashboardEditRequest
            {
                EventId = eventEntity.EventId,
                EventName = eventEntity.EventName,
                Description = eventEntity.Description,
                StartDate = eventEntity.StartDate,
                EndDate = eventEntity.EndDate,
                CategoryIds = eventEntity.EventCategory.Select(ec => ec.CategoryId).ToList(),
            };
            
            if (eventEntity.Location != null)
            {
                eventEditRequest.Latitude = eventEntity.Location.Y;
                eventEditRequest.Longitude = eventEntity.Location.X;
                eventEditRequest.Location = $"({eventEntity.Location.X}, {eventEntity.Location.Y})";
            }
            
            return View(eventEditRequest);
        }

        [HttpPost]
      
        public async Task<IActionResult> Edit(int id, EventDashboardEditRequest eventModel)
        {
            if (!User.IsInRole("OrganizationAdmin"))
            {
                return Forbid();
            }
            
            if (id != eventModel.EventId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                // Reload categories for dropdown
                ViewBag.Categories = await _context.Categories.ToListAsync();
                
             
                
                return View(eventModel);
            }
            
            var eventEntity = await _context.Events
                .Include(e => e.EventCategory)
                .FirstOrDefaultAsync(e => e.EventId == id);
                
            if (eventEntity == null)
            {
                return NotFound();
            }
            
            // Check if the event belongs to the organization admin's organization
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var organization = await _context.Organization
                .FirstOrDefaultAsync(o => o.UserId == userId);
            
            if (organization == null || eventEntity.OrganizationID != organization.OrganizationId)
            {
                return Forbid();
            }
            
            // Update event properties
            eventEntity.EventName = eventModel.EventName;
            eventEntity.Description = eventModel.Description;
            eventEntity.StartDate = eventModel.StartDate;
            eventEntity.EndDate = eventModel.EndDate;
            eventEntity.Location = eventModel.Location != null ? new Point(eventModel.Longitude, eventModel.Latitude) { SRID = 4326 } : null;

            
            // Handle image upload
            if (eventModel.EventPictureFile != null )
            {
                if(eventEntity.EventPicture is not null)
                {
                    var publicId = ImageRemoveHelper.ExtractRootPublicIdFromUrl(eventEntity.EventPicture.ToString());
                    _ = await _cloudinaryService.DeleteImageAsync(publicId);
                }
                    eventEntity.EventPicture = await _cloudinaryService.UploadImageAsync(eventModel.EventPictureFile);
            }
            
            // Update categories
            // First remove existing categories
            _context.EventCategories.RemoveRange(eventEntity.EventCategory);
            
            // Then add new categories
            if (eventModel.CategoryIds != null && eventModel.CategoryIds.Any())
            {
                foreach (var categoryId in eventModel.CategoryIds)
                {
                    _context.EventCategories.Add(new EventCategory
                    {
                        EventId = eventEntity.EventId,
                        CategoryId = categoryId
                    });
                }
            }
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(eventModel.EventId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventEntity = await _context.Events
                .Include(e => e.Organization)
                .Include(e => e.EventCategory)
                    .ThenInclude(ec => ec.Category)
                .FirstOrDefaultAsync(m => m.EventId == id);
                
            if (eventEntity == null)
            {
                return NotFound();
            }
            
            // Check if organization admin is trying to delete an event that doesn't belong to them
            if (User.IsInRole("OrganizationAdmin"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var organization = await _context.Organization
                    .FirstOrDefaultAsync(o => o.UserId == userId);
                
                if (organization == null || eventEntity.OrganizationID != organization.OrganizationId)
                {
                    return Forbid();
                }
            }
            
            var eventResponse = new EventDashboardResponse
            {
                EventId = eventEntity.EventId,
                EventName = eventEntity.EventName,
                Description = eventEntity.Description,
                EventPicture = eventEntity.EventPicture,
                StartDate = eventEntity.StartDate,
                EndDate = eventEntity.EndDate,
                CreateAt = eventEntity.CreateAt,
                Location = eventEntity.Location != null ? $"({eventEntity.Location.X}, {eventEntity.Location.Y})" : "No location",
                OrganizationID = eventEntity.OrganizationID,
                OrganizationName = eventEntity.Organization.Name,
                Categories = eventEntity.EventCategory.Select(ec => ec.Category.CategoryName).ToList()
            };

            return View(eventResponse);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventEntity = await _context.Events.FindAsync(id);
            
            if (eventEntity == null)
            {
                return NotFound();
            }
            
            // Check if organization admin is trying to delete an event that doesn't belong to them
            if (User.IsInRole("OrganizationAdmin"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var organization = await _context.Organization
                    .FirstOrDefaultAsync(o => o.UserId == userId);
                
                if (organization == null || eventEntity.OrganizationID != organization.OrganizationId)
                {
                    return Forbid();
                }
            }
            
            _context.Events.Remove(eventEntity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}
