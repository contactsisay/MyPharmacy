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
            //int result = Common.isAuthorized(HttpContext.Session.GetString(SessionVariable.SessionKeyUserId), this.ControllerContext);
            //if (result > 0) //Authorized
            //    return View();
            //else if (result < 0)
            //{
            //    TempData["MessageType"] = "error";
            //    TempData["Message"] = "Session expired! Please Login Again.";
            //    return RedirectToAction("Index", "Home", new { area = "Default" });
            //}
            //else
            //{
            //    TempData["MessageType"] = "error";
            //    TempData["Message"] = "Sorry! You are not Authorized for this operation.";
            //    return RedirectToAction("Index", "Home", new { area = "Dashboard" });
            //}
            int userRoleId = Convert.ToInt32(HttpContext.Session.GetString(SessionVariable.SessionKeyUserRoleId));
            var roleModules = _context.RoleModules.Where(rm => rm.RoleId == userRoleId).ToList();
            ViewData["UserRoleModules"] = roleModules;
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
