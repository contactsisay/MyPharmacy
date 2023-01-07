using Microsoft.AspNetCore.Mvc;
using MyPharmacy.Models;

namespace MyPharmacy.Areas.Purchase.Controllers
{
    [Area("Purchase")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Purchase Home";
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);
            return View();
        }
    }
}
