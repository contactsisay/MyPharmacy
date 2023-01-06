using Microsoft.AspNetCore.Mvc;
using MyPharmacy.Data;

namespace MyPharmacy.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Admin Home";
            return View();
        }
    }
}
