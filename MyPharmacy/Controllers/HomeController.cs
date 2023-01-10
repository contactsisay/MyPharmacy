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
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);

            #region Database Cleared? No Problem. I will create one for you
            //List<Employee> employees = _context.Employees.ToList();
            //if (employees.Count == 0)
            //{
            //    //user type
            //    UserType ut = new UserType();
            //    ut.Name = "Staff";
            //    ut.Status = 1;
            //    _context.UserTypes.Add(ut);
            //    _context.SaveChanges();

            //    //department
            //    Department dept = new Department();
            //    dept.Name = "HR";
            //    dept.Status = 1;
            //    _context.Departments.Add(dept);
            //    _context.SaveChanges();

            //    //job position
            //    JobPosition jp = new JobPosition();
            //    jp.DepartmentId = dept.Id;
            //    jp.InitialSalary = 5500;
            //    jp.Name = "Administrator";
            //    jp.Status = 1;
            //    _context.JobPositions.Add(jp);
            //    _context.SaveChanges();

            //    //employment type
            //    EmploymentType et = new EmploymentType();
            //    et.Name = "Permanent";
            //    et.Status = 1;
            //    _context.EmploymentTypes.Add(et);
            //    _context.SaveChanges();

            //    //role
            //    Role role = new Role();
            //    role.Name = "Administrator Role";
            //    role.Description = "";
            //    role.Status = 1;
            //    _context.Roles.Add(role);
            //    _context.SaveChanges();

            //    //role module
            //    RoleModule rm = new RoleModule();
            //    rm.RoleId = role.Id;
            //    rm.ModuleId = (int)Common.ModuleName.ADMIN;
            //    rm.Status = 1;
            //    _context.RoleModules.Add(rm);
            //    _context.SaveChanges();

            //    //employee
            //    Employee employee = new Employee();
            //    employee.FirstName = "Sisay";
            //    employee.MiddleName = "Mersha";
            //    employee.LastName = "Mekonnen";
            //    employee.Gender = "Male";
            //    employee.JoinedDate = DateTime.Now;
            //    employee.PhoneNo = "251913011370";
            //    employee.PermanentAddress = "Ziway";
            //    employee.Qualification = "Bsc";
            //    employee.UserTypeId = ut.Id;
            //    employee.JobPositionId = jp.Id;
            //    employee.EmploymentTypeId = et.Id;
            //    employee.RoleId = role.Id;
            //    employee.Code = "EMP-001";
            //    employee.WorkExperience = 10;
            //    employee.PhotoPath = "photo.png";
            //    employee.MaritalStatus = "Married";
            //    employee.CurrentAddress = "Ziway";
            //    employee.Note = "a";
            //    employee.EmergencyContactNo = "251913011370";
            //    employee.EmailAddress = "contactsisay@gmail.com";
            //    employee.Password = BCrypt.Net.BCrypt.HashPassword("asdf");
            //    employee.Status = 1;
            //    _context.Employees.Add(employee);
            //    _context.SaveChanges();
            //}
            #endregion
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