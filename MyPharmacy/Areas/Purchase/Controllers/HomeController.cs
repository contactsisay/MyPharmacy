using Microsoft.AspNetCore.Mvc;

namespace MyPharmacy.Areas.Purchase.Controllers
{
    [Area("Purchase")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
