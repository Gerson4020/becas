using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BECASLC;

namespace BECAS.Controllers
{
    public class CohortesController : Controller
    {
        private readonly MEOBContext _context;

        public CohortesController(MEOBContext context)
        {
            _context = context;
        }

        // GET: Cohortes
        public async Task<IActionResult> Index()
        {
              return View(await _context.Cohortes.ToListAsync());
        }

        // GET: Cohortes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Cohortes == null)
            {
                return NotFound();
            }

            var cohorte = await _context.Cohortes
                .FirstOrDefaultAsync(m => m.IdCohorte == id);
            if (cohorte == null)
            {
                return NotFound();
            }

            return View(cohorte);
        }

        // GET: Cohortes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cohortes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCohorte,Nombre,Activo")] Cohorte cohorte)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cohorte);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cohorte);
        }

        // GET: Cohortes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Cohortes == null)
            {
                return NotFound();
            }

            var cohorte = await _context.Cohortes.FindAsync(id);
            if (cohorte == null)
            {
                return NotFound();
            }
            return View(cohorte);
        }

        // POST: Cohortes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCohorte,Nombre,Activo")] Cohorte cohorte)
        {
            if (id != cohorte.IdCohorte)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cohorte);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CohorteExists(cohorte.IdCohorte))
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
            return View(cohorte);
        }

        // GET: Cohortes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Cohortes == null)
            {
                return NotFound();
            }

            var cohorte = await _context.Cohortes
                .FirstOrDefaultAsync(m => m.IdCohorte == id);
            if (cohorte == null)
            {
                return NotFound();
            }

            return View(cohorte);
        }

        // POST: Cohortes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Cohortes == null)
            {
                return Problem("Entity set 'MEOBContext.Cohortes'  is null.");
            }
            var cohorte = await _context.Cohortes.FindAsync(id);
            if (cohorte != null)
            {
                _context.Cohortes.Remove(cohorte);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CohorteExists(int id)
        {
          return _context.Cohortes.Any(e => e.IdCohorte == id);
        }
    }
}
