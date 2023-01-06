using BALibrary.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyPharmacy.Data;

namespace MyPharmacy.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleModulesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoleModulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserModule/RoleModules
        public async Task<IActionResult> Index(int id)
        {
            HttpContext.Session.SetInt32("_SelectedId", id);
            ViewData["Roles"] = _context.Roles.Where(r => r.Id == id).ToList();
            ViewData["ModuleNames"] = MyPharmacy.Models.Common.GetModuleNames();
            var applicationDbContext = _context.RoleModules.Include(r => r.Role).Where(rm => rm.RoleId == id);
            return View(await applicationDbContext.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveRoleModules(IFormCollection form)
        {
            int id = (int)HttpContext.Session.GetInt32("_SelectedId");
            if (!string.IsNullOrEmpty(form["roleId"]))
            {
                int roleId = Convert.ToInt32(form["roleId"]);
                string[] moduleNames = MyPharmacy.Models.Common.GetModuleNames();
                foreach (string moduleName in moduleNames)
                {
                    string m = form[moduleName.ToUpper() + "_" + roleId];
                    if (!string.IsNullOrEmpty(m))
                    {
                        int moduleId = (int)MyPharmacy.Models.Common.GetModuleId(moduleName);
                        List<RoleModule> roleModules = _context.RoleModules.Where(rm => rm.RoleId == roleId).Where(rm => rm.ModuleId == moduleId).ToList();

                        if (roleModules.Count <= 0)
                        {
                            RoleModule roleM = new RoleModule();
                            roleM.RoleId = roleId;
                            roleM.ModuleId = moduleId;
                            _context.Add(roleM);
                            await _context.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        int moduleId = (int)MyPharmacy.Models.Common.GetModuleId(moduleName);
                        List<RoleModule> roleModules = _context.RoleModules.Where(rm => rm.RoleId == roleId).Where(rm => rm.ModuleId == moduleId).ToList();
                        foreach (RoleModule rm in roleModules)
                        {
                            _context.RoleModules.Remove(rm);
                            await _context.SaveChangesAsync();

                            //Looking in role module exceptions
                            List<RoleModuleException> roleMExceptions = _context.RoleModuleExceptions.Where(r => r.RoleModuleId == rm.Id).ToList();
                            if (roleMExceptions.Count > 0)
                            {
                                _context.RoleModuleExceptions.RemoveRange(roleMExceptions);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
            }

            return RedirectToAction("Index", "Roles", new { area = "HR" });
        }

        public async Task<IActionResult> SaveRoleModuleExceptions(IFormCollection form)
        {
            int id = (int)HttpContext.Session.GetInt32("_SelectedId");
            if (!string.IsNullOrEmpty(form["roleId"]))
            {
                int roleId = Convert.ToInt32(form["roleId"]);
                string[] moduleNames = MyPharmacy.Models.Common.GetModuleNames();
                foreach (string moduleName in moduleNames)
                {
                    string m = form[moduleName.ToUpper() + "_" + roleId];
                    if (!string.IsNullOrEmpty(m))
                    {
                        int moduleId = (int)MyPharmacy.Models.Common.GetModuleId(moduleName);
                        List<RoleModule> roleModules = _context.RoleModules.Where(rm => rm.RoleId == roleId).Where(rm => rm.ModuleId == moduleId).ToList();

                        if (roleModules.Count <= 0)
                        {
                            RoleModule roleM = new RoleModule();
                            roleM.RoleId = roleId;
                            roleM.ModuleId = moduleId;
                            _context.Add(roleM);
                            await _context.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        int moduleId = (int)MyPharmacy.Models.Common.GetModuleId(moduleName);
                        List<RoleModule> roleModules = _context.RoleModules.Where(rm => rm.RoleId == roleId).Where(rm => rm.ModuleId == moduleId).ToList();
                        foreach (RoleModule rm in roleModules)
                        {
                            _context.RoleModules.Remove(rm);
                            await _context.SaveChangesAsync();

                            //Looking in role module exceptions
                            List<RoleModuleException> roleMExceptions = _context.RoleModuleExceptions.Where(r => r.RoleModuleId == rm.Id).ToList();
                            if (roleMExceptions.Count > 0)
                            {
                                _context.RoleModuleExceptions.RemoveRange(roleMExceptions);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
            }

            return RedirectToAction("Index", "Roles", new { area = "HR" });
        }

        // GET: UserModule/RoleModules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roleModule = await _context.RoleModules
                .Include(r => r.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (roleModule == null)
            {
                return NotFound();
            }

            return View(roleModule);
        }

        // GET: UserModule/RoleModules/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
            return View();
        }

        // POST: UserModule/RoleModules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RoleId,ModuleId")] RoleModule roleModule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(roleModule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", roleModule.RoleId);
            return View(roleModule);
        }

        public async Task<ActionResult> RoleModuleExceptions(int id)
        {
            ViewData["RoleModules"] = _context.RoleModules.Include(r => r.Role).Where(r => r.Id == id).ToList();
            var applicationDbContext = _context.RoleModuleExceptions.Include(r => r.RoleModule).Where(r => r.RoleModuleId == id);
            ViewData["TableNames"] = MyPharmacy.Models.Common.GetTableNames();
            return View(await applicationDbContext.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoleModuleExceptions(IFormCollection form)
        {
            int id = (int)HttpContext.Session.GetInt32("_SelectedId");
            if (!string.IsNullOrEmpty(form["roleModuleId"]))
            {

                int roleModuleId = Convert.ToInt32(form["roleModuleId"]);
                List<string> tableNames = MyPharmacy.Models.Common.GetTableNames();

                foreach (string tableName in tableNames)
                {
                    string access_right = form[tableName + "_access"];
                    RoleModuleException rme = new RoleModuleException();
                    if (access_right == "1")
                    { //fully denied
                        rme.TableName = tableName.ToUpper();
                        rme.Browse = false;
                        rme.Read = false;
                        rme.Edit = false;
                        rme.Add = false;
                        rme.Delete = false;
                        rme.FullyDenied = true;
                        rme.FullyGranted = false;
                        rme.AccessRightName = 1;
                        rme.Status = 1;

                        rme.RoleModuleId = roleModuleId;
                        _context.RoleModuleExceptions.Add(rme);
                        await _context.SaveChangesAsync();
                    }
                    else if (access_right != null && access_right != "0" && access_right != "1")
                    {
                        rme.TableName = tableName.ToUpper();
                        if (!string.IsNullOrEmpty(form[tableName + "_b"]))
                            rme.Browse = false;
                        else
                            rme.Browse = true;

                        if (!string.IsNullOrEmpty(form[tableName + "_r"]))
                            rme.Read = false;
                        else
                            rme.Read = true;

                        if (!string.IsNullOrEmpty(form[tableName + "_e"]))
                            rme.Edit = false;
                        else
                            rme.Edit = true;

                        if (!string.IsNullOrEmpty(form[tableName + "_a"]))
                            rme.Add = false;
                        else
                            rme.Add = true;

                        if (!string.IsNullOrEmpty(form[tableName + "_d"]))
                            rme.Delete = false;
                        else
                            rme.Delete = true;

                        rme.FullyDenied = false;
                        rme.FullyGranted = false;
                        rme.AccessRightName = 1;
                        rme.Status = 1;
                        rme.RoleModuleId = roleModuleId;

                        _context.RoleModuleExceptions.Add(rme);
                        await _context.SaveChangesAsync();
                    }
                }

                ViewData["RoleModules"] = _context.RoleModules.Include(r => r.Role).Where(r => r.Id == id).ToList();
                var applicationDbContext = _context.RoleModuleExceptions.Include(r => r.RoleModule).Where(r => r.RoleModuleId == id);
                ViewData["TableNames"] = MyPharmacy.Models.Common.GetTableNames();
                //return View(applicationDbContext);

                return RedirectToAction("Index", "Roles", new { area = "HR" });
            }

            return RedirectToAction("Index", "Roles", new { area = "HR" });
        }

        // GET: UserModule/RoleModules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roleModule = await _context.RoleModules.FindAsync(id);
            if (roleModule == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", roleModule.RoleId);
            return View(roleModule);
        }

        // POST: UserModule/RoleModules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RoleId,ModuleId")] RoleModule roleModule)
        {
            if (id != roleModule.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(roleModule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleModuleExists(roleModule.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", roleModule.RoleId);
            return View(roleModule);
        }

        // GET: UserModule/RoleModules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roleModule = await _context.RoleModules
                .Include(r => r.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (roleModule == null)
            {
                return NotFound();
            }

            return View(roleModule);
        }

        // POST: UserModule/RoleModules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var roleModule = await _context.RoleModules.FindAsync(id);
            _context.RoleModules.Remove(roleModule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoleModuleExists(int id)
        {
            return _context.RoleModules.Any(e => e.Id == id);
        }
    }
}
