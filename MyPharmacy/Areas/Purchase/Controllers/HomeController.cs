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
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionVariable.SessionKeyUserId)))
            {
                TempData["MessageType"] = "success";
                TempData["Message"] = "Session expired! Please Login Again.";
                return RedirectToAction("Index", "Home", new { area = "Default" });
            }

            if (Common.isAuthorized(HttpContext.Session.GetString(SessionVariable.SessionKeyUserId), this.ControllerContext))
                return View();
            else
            {
                TempData["MessageType"] = "danger";
                TempData["Message"] = "Sorry! You are not Authorized for this operation.";
                return RedirectToAction("Index", "Home", new { area = "Dashboard" });
            }
        }
    }
}
