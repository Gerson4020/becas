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
    public class CatSedesController : Controller
    {
        private readonly MEOBContext _context;

        public CatSedesController(MEOBContext context)
        {
            _context = context;
        }

        // GET: CatSedes
        public async Task<IActionResult> Index()
        {
              return View(await _context.CatSedes.ToListAsync());
        }

        // GET: CatSedes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CatSedes == null)
            {
                return NotFound();
            }

            var catSede = await _context.CatSedes
                .FirstOrDefaultAsync(m => m.IdCatSede == id);
            if (catSede == null)
            {
                return NotFound();
            }

            return View(catSede);
        }

        // GET: CatSedes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CatSedes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCatSede,Nombre,Activo")] CatSede catSede)
        {
            if (ModelState.IsValid)
            {
                _context.Add(catSede);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(catSede);
        }

        // GET: CatSedes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CatSedes == null)
            {
                return NotFound();
            }

            var catSede = await _context.CatSedes.FindAsync(id);
            if (catSede == null)
            {
                return NotFound();
            }
            return View(catSede);
        }

        // POST: CatSedes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCatSede,Nombre,Activo")] CatSede catSede)
        {
            if (id != catSede.IdCatSede)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(catSede);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CatSedeExists(catSede.IdCatSede))
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
            return View(catSede);
        }

        // GET: CatSedes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CatSedes == null)
            {
                return NotFound();
            }

            var catSede = await _context.CatSedes
                .FirstOrDefaultAsync(m => m.IdCatSede == id);
            if (catSede == null)
            {
                return NotFound();
            }

            return View(catSede);
        }

        // POST: CatSedes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CatSedes == null)
            {
                return Problem("Entity set 'MEOBContext.CatSedes'  is null.");
            }
            var catSede = await _context.CatSedes.FindAsync(id);
            if (catSede != null)
            {
                _context.CatSedes.Remove(catSede);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CatSedeExists(int id)
        {
          return _context.CatSedes.Any(e => e.IdCatSede == id);
        }
    }
}
