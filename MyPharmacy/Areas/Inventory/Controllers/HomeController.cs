using Microsoft.AspNetCore.Mvc;

namespace MyPharmacy.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
