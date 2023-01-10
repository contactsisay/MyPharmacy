using Microsoft.AspNetCore.Mvc;
using MyPharmacy.Data;
using MyPharmacy.Models;

namespace MyPharmacy.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Dashboard";
            int userRoleId = Convert.ToInt32(HttpContext.Session.GetString(SessionVariable.SessionKeyUserRoleId));
            var roleModules = _context.RoleModules.Where(rm => rm.RoleId == userRoleId).ToList();
            ViewData["UserRoleModules"] = roleModules;
            return View();
        }

        public IActionResult ViewSummary()
        {
            ViewData["Title"] = "View Summary";
            return View();
        }

        public IActionResult NotAuthorized()
        {
            int userRoleId = Convert.ToInt32(HttpContext.Session.GetString(SessionVariable.SessionKeyUserRoleId));
            var roleModules = _context.RoleModules.Where(rm => rm.RoleId == userRoleId).ToList();
            ViewData["UserRoleModules"] = roleModules;
            return View();
        }

    }
}
