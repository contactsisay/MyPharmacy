using BALibrary.HR;
using Microsoft.AspNetCore.Mvc;
using MyPharmacy.Data;
using MyPharmacy.Models;
using System.Diagnostics;

namespace MyPharmacy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string emlAddress, string passWrd)
        {
            if (!string.IsNullOrEmpty(emlAddress) && !string.IsNullOrEmpty(passWrd))
            {
                List<Employee> emps = _context.Employees.Where(e => e.EmailAddress == emlAddress).ToList();
                if (emps.Count > 0)
                {
                    bool passed = false;
                    foreach (Employee emp in emps)
                    {
                        //checking if password is correct (verifying password)
                        bool isValidPassword = BCrypt.Net.BCrypt.Verify(passWrd, emp.Password);
                        if (isValidPassword)
                        {
                            HttpContext.Session.SetString("SessionKeyUserId", emp.Id.ToString());
                            HttpContext.Session.SetString("SessionKeyUserEmail", emp.EmailAddress.ToString());
                            HttpContext.Session.SetString("SessionKeyUserRoleId", emp.RoleId.ToString());
                            HttpContext.Session.SetString("SessionKeySessionId", Guid.NewGuid().ToString());

                            passed = true;
                            break;//stopping loop if employee password is verified and correct
                        }
                    }

                    if (passed)
                    {
                        TempData["MessageType"] = "success";
                        TempData["Message"] = "Successfully Logged In";
                        return RedirectToAction("Index", "Home", new { area = "Dashboard" });
                    }
                }
            }

            TempData["MessageType"] = "error";
            TempData["Message"] = "Account not found! Please check your email address and/or password";
            return RedirectToAction("Index", "Home", new { area = "Default" });
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyUserId);
            HttpContext.Session.Remove(SessionVariable.SessionKeyUserEmail);
            HttpContext.Session.Remove(SessionVariable.SessionKeyUserRoleId);
            HttpContext.Session.Remove(SessionVariable.SessionKeySessionId);

            TempData["MessageType"] = "success";
            TempData["Message"] = "Successfully Logged Out";
            return RedirectToAction("Index", "Home", new { area = "Default" });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}