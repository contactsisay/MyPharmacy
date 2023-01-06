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
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionVariable.SessionKeyUserId)))
            {
                HttpContext.Session.SetString("MessageType", "danger");
                HttpContext.Session.SetString("Message", "Session expired! Please Login Again.");
                return RedirectToAction("Index", "Home", new { area = "Default" });
            }

            if (Common.isAuthorized(HttpContext.Session.GetString(SessionVariable.SessionKeyUserId), this.ControllerContext))
                return View();
            else
            {
                HttpContext.Session.SetString("MessageType", "danger");
                HttpContext.Session.SetString("Message", "Sorry! You are not Authorized for this operation.");
                return RedirectToAction("Index", "Home", new { area = "Dashboard" });
            }
        }

    }
}
