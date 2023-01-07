using BALibrary.Report;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyPharmacy.Data;

namespace MyPharmacy.Areas.Report.Controllers
{
    [Area("Report")]
    public class SavedReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SavedReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Report/SavedReports
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SavedReports.Include(s => s.ReportType);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Report/SavedReports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SavedReports == null)
            {
                return NotFound();
            }

            var savedReport = await _context.SavedReports
                .Include(s => s.ReportType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (savedReport == null)
            {
                return NotFound();
            }

            return View(savedReport);
        }

        // GET: Report/SavedReports/Create
        public IActionResult Create()
        {
            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "Name");
            return View();
        }

        // POST: Report/SavedReports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ReportTypeId,FromDate,ToDate,EmployeeId")] SavedReport savedReport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(savedReport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "Name", savedReport.ReportTypeId);
            return View(savedReport);
        }

        // GET: Report/SavedReports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SavedReports == null)
            {
                return NotFound();
            }

            var savedReport = await _context.SavedReports.FindAsync(id);
            if (savedReport == null)
            {
                return NotFound();
            }
            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "Name", savedReport.ReportTypeId);
            return View(savedReport);
        }

        // POST: Report/SavedReports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ReportTypeId,FromDate,ToDate,EmployeeId")] SavedReport savedReport)
        {
            if (id != savedReport.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(savedReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SavedReportExists(savedReport.Id))
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
            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "Name", savedReport.ReportTypeId);
            return View(savedReport);
        }

        // GET: Report/SavedReports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SavedReports == null)
            {
                return NotFound();
            }

            var savedReport = await _context.SavedReports
                .Include(s => s.ReportType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (savedReport == null)
            {
                return NotFound();
            }

            return View(savedReport);
        }

        // POST: Report/SavedReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SavedReports == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SavedReport'  is null.");
            }
            var savedReport = await _context.SavedReports.FindAsync(id);
            if (savedReport != null)
            {
                _context.SavedReports.Remove(savedReport);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SavedReportExists(int id)
        {
            return _context.SavedReports.Any(e => e.Id == id);
        }
    }
}
