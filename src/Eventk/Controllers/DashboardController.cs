using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventk.Controllers
{
    [Authorize(Roles = "SuperAdmin,OrganizationAdmin")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {

            return View();
        }

        [Authorize(Roles = "SuperAdmin")]
        public IActionResult SuperAdminPanel()
        {
            return View();
        }

        [Authorize(Roles = "OrganizationAdmin")]
        public IActionResult OrganizationAdminPanel()
        {
            return View();
        }
    }
}