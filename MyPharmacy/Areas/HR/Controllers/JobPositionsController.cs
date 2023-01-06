﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BALibrary.HR;
using MyPharmacy.Data;

namespace MyPharmacy.Areas.HR.Controllers
{
    [Area("HR")]
    public class JobPositionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobPositionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HR/JobPositions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.JobPositions.Include(j => j.Department);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: HR/JobPositions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.JobPositions == null)
            {
                return NotFound();
            }

            var jobPosition = await _context.JobPositions
                .Include(j => j.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobPosition == null)
            {
                return NotFound();
            }

            return View(jobPosition);
        }

        // GET: HR/JobPositions/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            return View();
        }

        // POST: HR/JobPositions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DepartmentId,Name,InitialSalary")] JobPosition jobPosition)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jobPosition);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", jobPosition.DepartmentId);
            return View(jobPosition);
        }

        // GET: HR/JobPositions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.JobPositions == null)
            {
                return NotFound();
            }

            var jobPosition = await _context.JobPositions.FindAsync(id);
            if (jobPosition == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", jobPosition.DepartmentId);
            return View(jobPosition);
        }

        // POST: HR/JobPositions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DepartmentId,Name,InitialSalary")] JobPosition jobPosition)
        {
            if (id != jobPosition.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jobPosition);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobPositionExists(jobPosition.Id))
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
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", jobPosition.DepartmentId);
            return View(jobPosition);
        }

        // GET: HR/JobPositions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.JobPositions == null)
            {
                return NotFound();
            }

            var jobPosition = await _context.JobPositions
                .Include(j => j.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobPosition == null)
            {
                return NotFound();
            }

            return View(jobPosition);
        }

        // POST: HR/JobPositions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.JobPositions == null)
            {
                return Problem("Entity set 'ApplicationDbContext.JobPositions'  is null.");
            }
            var jobPosition = await _context.JobPositions.FindAsync(id);
            if (jobPosition != null)
            {
                _context.JobPositions.Remove(jobPosition);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobPositionExists(int id)
        {
          return _context.JobPositions.Any(e => e.Id == id);
        }
    }
}
