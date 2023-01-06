using Microsoft.AspNetCore.Mvc;

namespace MyPharmacy.Areas.Sales.Controllers
{
    [Area("Sales")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
