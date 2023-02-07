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
    public class GradoSedesController : Controller
    {
        private readonly MEOBContext _context;

        public GradoSedesController(MEOBContext context)
        {
            _context = context;
        }

        // GET: GradoSedes
        public async Task<IActionResult> Index()
        {
            var mEOBContext = _context.GradoSedes.Include(g => g.grado).Include(s=> s.sede);
            return View(await mEOBContext.ToListAsync());
        }

        // GET: GradoSedes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GradoSedes == null)
            {
                return NotFound();
            }

            var gradoSede = await _context.GradoSedes
                .Include(g => g.grado)
                .FirstOrDefaultAsync(m => m.IdGradoSede == id);
            if (gradoSede == null)
            {
                return NotFound();
            }

            return View(gradoSede);
        }

        // GET: GradoSedes/Create
        public IActionResult Create()
        {
            ViewData["IdGrado"] = new SelectList(_context.Grados, "IdGrado", "Nombre");
            ViewData["IdSede"] = new SelectList(_context.CatSedes, "IdCatSede", "Nombre");
            return View();
        }

        // POST: GradoSedes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdGradoSede,IdGrado,IdSede,Activo")] GradoSede gradoSede)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gradoSede);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdGrado"] = new SelectList(_context.Grados, "IdGrado", "IdGrado", gradoSede.IdGrado);
            return View(gradoSede);
        }

        // GET: GradoSedes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GradoSedes == null)
            {
                return NotFound();
            }

            var gradoSede = await _context.GradoSedes.FindAsync(id);
            if (gradoSede == null)
            {
                return NotFound();
            }
            ViewData["IdGrado"] = new SelectList(_context.Grados, "IdGrado", "IdGrado", gradoSede.IdGrado);
            return View(gradoSede);
        }

        // POST: GradoSedes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdGradoSede,IdGrado,IdSede,Activo")] GradoSede gradoSede)
        {
            if (id != gradoSede.IdGradoSede)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gradoSede);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GradoSedeExists(gradoSede.IdGradoSede))
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
            ViewData["IdGrado"] = new SelectList(_context.Grados, "IdGrado", "IdGrado", gradoSede.IdGrado);
            return View(gradoSede);
        }

        // GET: GradoSedes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.GradoSedes == null)
            {
                return NotFound();
            }

            var gradoSede = await _context.GradoSedes
                .Include(g => g.grado)
                .FirstOrDefaultAsync(m => m.IdGradoSede == id);
            if (gradoSede == null)
            {
                return NotFound();
            }

            return View(gradoSede);
        }

        // POST: GradoSedes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.GradoSedes == null)
            {
                return Problem("Entity set 'MEOBContext.GradoSedes'  is null.");
            }
            var gradoSede = await _context.GradoSedes.FindAsync(id);
            if (gradoSede != null)
            {
                _context.GradoSedes.Remove(gradoSede);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GradoSedeExists(int id)
        {
          return _context.GradoSedes.Any(e => e.IdGradoSede == id);
        }
    }
}
