using BALibrary.Report;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyPharmacy.Data;
using MyPharmacy.Models;

namespace MyPharmacy.Areas.Report.Controllers
{
    [Area("Report")]
    public class PurchaseReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchaseReportsController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PreviewReport(string? FromDate, string? ToDate, string? EmployeeId)
        {
            var queryResult = from invD in _context.InvoiceDetails
                              join inv in _context.Invoices.Include(a => a.InvoiceType).Include(a => a.Customer) on invD.InvoiceId equals inv.Id
                              join e in _context.Employees on inv.EmployeeId equals e.Id
                              join pb in _context.ProductBatches.Include(pb => pb.Product) on invD.ProductBatchId equals pb.Id
                              select new
                              {
                                  inv.InvoiceNo,
                                  InvoiceTypeName = inv.InvoiceType.Name,
                                  ProductName = pb.Product.Name + "",
                                  ProductCode = pb.Product.Code,
                                  CustomerTINNo = inv.Customer.TINNo,
                                  CustomerName = inv.Customer.Name + "",
                                  ProductBatchNo = pb.BatchNo,
                                  inv.InvoiceDate,
                                  EmployeeFullName = e.FirstName + e.MiddleName + e.LastName,
                                  e.Gender,
                                  ItemQuantity = invD.Quantity,
                                  ItemSellingPrice = invD.SellingPrice,
                                  InvoiceRowTotal = invD.RowTotal
                              };

            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName", EmployeeId);
            ViewData["Title"] = "Sales Report";
            ViewData["queryResult"] = queryResult;

            return View();
        }

        // GET: Report/PurchaseReports
        public async Task<IActionResult> Index()
        {
            return View(await _context.PurchaseReports.ToListAsync());
        }

        // GET: Report/PurchaseReports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PurchaseReports == null)
            {
                return NotFound();
            }

            var purchaseReport = await _context.PurchaseReports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchaseReport == null)
            {
                return NotFound();
            }

            return View(purchaseReport);
        }

        // GET: Report/PurchaseReports/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Report/PurchaseReports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FromDate,ToDate,EmployeeId")] PurchaseReport purchaseReport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(purchaseReport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(purchaseReport);
        }

        // GET: Report/PurchaseReports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PurchaseReports == null)
            {
                return NotFound();
            }

            var purchaseReport = await _context.PurchaseReports.FindAsync(id);
            if (purchaseReport == null)
            {
                return NotFound();
            }
            return View(purchaseReport);
        }

        // POST: Report/PurchaseReports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FromDate,ToDate,EmployeeId")] PurchaseReport purchaseReport)
        {
            if (id != purchaseReport.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(purchaseReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseReportExists(purchaseReport.Id))
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
            return View(purchaseReport);
        }

        // GET: Report/PurchaseReports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PurchaseReports == null)
            {
                return NotFound();
            }

            var purchaseReport = await _context.PurchaseReports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchaseReport == null)
            {
                return NotFound();
            }

            return View(purchaseReport);
        }

        // POST: Report/PurchaseReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PurchaseReports == null)
            {
                return Problem("Entity set 'ApplicationDbContext.PurchaseReports'  is null.");
            }
            var purchaseReport = await _context.PurchaseReports.FindAsync(id);
            if (purchaseReport != null)
            {
                _context.PurchaseReports.Remove(purchaseReport);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseReportExists(int id)
        {
            return _context.PurchaseReports.Any(e => e.Id == id);
        }
    }
}
