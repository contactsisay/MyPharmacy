using Microsoft.AspNetCore.Mvc;
using MyPharmacy.Models;

namespace MyPharmacy.Areas.Report.Controllers
{
    [Area("Report")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Reports Home";
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);
            return View();
        }
    }
}
