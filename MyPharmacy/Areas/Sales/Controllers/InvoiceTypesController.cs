using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BALibrary.Sales;
using MyPharmacy.Data;

namespace MyPharmacy.Areas.Sales.Controllers
{
    [Area("Sales")]
    public class InvoiceTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InvoiceTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sales/InvoiceTypes
        public async Task<IActionResult> Index()
        {
              return View(await _context.InvoiceTypes.ToListAsync());
        }

        // GET: Sales/InvoiceTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.InvoiceTypes == null)
            {
                return NotFound();
            }

            var invoiceType = await _context.InvoiceTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invoiceType == null)
            {
                return NotFound();
            }

            return View(invoiceType);
        }

        // GET: Sales/InvoiceTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sales/InvoiceTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] InvoiceType invoiceType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(invoiceType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(invoiceType);
        }

        // GET: Sales/InvoiceTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.InvoiceTypes == null)
            {
                return NotFound();
            }

            var invoiceType = await _context.InvoiceTypes.FindAsync(id);
            if (invoiceType == null)
            {
                return NotFound();
            }
            return View(invoiceType);
        }

        // POST: Sales/InvoiceTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] InvoiceType invoiceType)
        {
            if (id != invoiceType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invoiceType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvoiceTypeExists(invoiceType.Id))
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
            return View(invoiceType);
        }

        // GET: Sales/InvoiceTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.InvoiceTypes == null)
            {
                return NotFound();
            }

            var invoiceType = await _context.InvoiceTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invoiceType == null)
            {
                return NotFound();
            }

            return View(invoiceType);
        }

        // POST: Sales/InvoiceTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.InvoiceTypes == null)
            {
                return Problem("Entity set 'ApplicationDbContext.InvoiceTypes'  is null.");
            }
            var invoiceType = await _context.InvoiceTypes.FindAsync(id);
            if (invoiceType != null)
            {
                _context.InvoiceTypes.Remove(invoiceType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InvoiceTypeExists(int id)
        {
          return _context.InvoiceTypes.Any(e => e.Id == id);
        }
    }
}
