using Microsoft.AspNetCore.Mvc;
using MyPharmacy.Data;

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

            return View();
        }

    }
}
