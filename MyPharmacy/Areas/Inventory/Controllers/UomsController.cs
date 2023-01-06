using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BALibrary.Inventory;
using MyPharmacy.Data;

namespace MyPharmacy.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    public class UomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Inventory/Uoms
        public async Task<IActionResult> Index()
        {
              return View(await _context.Uoms.ToListAsync());
        }

        // GET: Inventory/Uoms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Uoms == null)
            {
                return NotFound();
            }

            var uom = await _context.Uoms
                .FirstOrDefaultAsync(m => m.Id == id);
            if (uom == null)
            {
                return NotFound();
            }

            return View(uom);
        }

        // GET: Inventory/Uoms/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inventory/Uoms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Uom uom)
        {
            if (ModelState.IsValid)
            {
                _context.Add(uom);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(uom);
        }

        // GET: Inventory/Uoms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Uoms == null)
            {
                return NotFound();
            }

            var uom = await _context.Uoms.FindAsync(id);
            if (uom == null)
            {
                return NotFound();
            }
            return View(uom);
        }

        // POST: Inventory/Uoms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Uom uom)
        {
            if (id != uom.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(uom);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UomExists(uom.Id))
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
            return View(uom);
        }

        // GET: Inventory/Uoms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Uoms == null)
            {
                return NotFound();
            }

            var uom = await _context.Uoms
                .FirstOrDefaultAsync(m => m.Id == id);
            if (uom == null)
            {
                return NotFound();
            }

            return View(uom);
        }

        // POST: Inventory/Uoms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Uoms == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Uoms'  is null.");
            }
            var uom = await _context.Uoms.FindAsync(id);
            if (uom != null)
            {
                _context.Uoms.Remove(uom);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UomExists(int id)
        {
          return _context.Uoms.Any(e => e.Id == id);
        }
    }
}
