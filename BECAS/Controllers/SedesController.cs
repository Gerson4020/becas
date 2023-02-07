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
    public class SedesController : Controller
    {
        private readonly MEOBContext _context;

        public SedesController(MEOBContext context)
        {
            _context = context;
        }

        // GET: Sedes
        public async Task<IActionResult> Index()
        {
            var mEOBContext = _context.Sedes.Include(s => s.catsede).Include(s => s.programa).Include(s => s.socio).Include(s => s.zona);
            return View(await mEOBContext.ToListAsync());
        }

        // GET: Sedes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Sedes == null)
            {
                return NotFound();
            }

            var sede = await _context.Sedes
                .Include(s => s.catsede)
                .Include(s => s.programa)
                .Include(s => s.socio)
                .Include(s => s.zona)
                .FirstOrDefaultAsync(m => m.IdSede == id);
            if (sede == null)
            {
                return NotFound();
            }

            return View(sede);
        }

        // GET: Sedes/Create
        public IActionResult Create()
        {
            ViewData["IdCatSede"] = new SelectList(_context.CatSedes, "IdCatSede", "Nombre");
            ViewData["IdPrograma"] = new SelectList(_context.Programas, "IdPrograma", "Nombre");
            ViewData["IdSocio"] = new SelectList(_context.SocioImplementadors, "IdImplementador", "Nombre");
            ViewData["IdZona"] = new SelectList(_context.Zonas, "IdZona", "Nombre");
            return View();
        }

        // POST: Sedes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdSede,Activo,IdSocio,IdZona,IdPrograma,IdCatSede")] Sede sede)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sede);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCatSede"] = new SelectList(_context.CatSedes, "IdCatSede", "IdCatSede", sede.IdCatSede);
            ViewData["IdPrograma"] = new SelectList(_context.Programas, "IdPrograma", "IdPrograma", sede.IdPrograma);
            ViewData["IdSocio"] = new SelectList(_context.SocioImplementadors, "IdImplementador", "IdImplementador", sede.IdSocio);
            ViewData["IdZona"] = new SelectList(_context.Zonas, "IdZona", "IdZona", sede.IdZona);
            return View(sede);
        }

        // GET: Sedes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Sedes == null)
            {
                return NotFound();
            }

            var sede = await _context.Sedes.FindAsync(id);
            if (sede == null)
            {
                return NotFound();
            }
            ViewData["IdCatSede"] = new SelectList(_context.CatSedes, "IdCatSede", "IdCatSede", sede.IdCatSede);
            ViewData["IdPrograma"] = new SelectList(_context.Programas, "IdPrograma", "IdPrograma", sede.IdPrograma);
            ViewData["IdSocio"] = new SelectList(_context.SocioImplementadors, "IdImplementador", "IdImplementador", sede.IdSocio);
            ViewData["IdZona"] = new SelectList(_context.Zonas, "IdZona", "IdZona", sede.IdZona);
            return View(sede);
        }

        // POST: Sedes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdSede,Activo,IdSocio,IdZona,IdPrograma,IdCatSede")] Sede sede)
        {
            if (id != sede.IdSede)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sede);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SedeExists(sede.IdSede))
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
            ViewData["IdCatSede"] = new SelectList(_context.CatSedes, "IdCatSede", "IdCatSede", sede.IdCatSede);
            ViewData["IdPrograma"] = new SelectList(_context.Programas, "IdPrograma", "IdPrograma", sede.IdPrograma);
            ViewData["IdSocio"] = new SelectList(_context.SocioImplementadors, "IdImplementador", "IdImplementador", sede.IdSocio);
            ViewData["IdZona"] = new SelectList(_context.Zonas, "IdZona", "IdZona", sede.IdZona);
            return View(sede);
        }

        // GET: Sedes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Sedes == null)
            {
                return NotFound();
            }

            var sede = await _context.Sedes
                .Include(s => s.catsede)
                .Include(s => s.programa)
                .Include(s => s.socio)
                .Include(s => s.zona)
                .FirstOrDefaultAsync(m => m.IdSede == id);
            if (sede == null)
            {
                return NotFound();
            }

            return View(sede);
        }

        // POST: Sedes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Sedes == null)
            {
                return Problem("Entity set 'MEOBContext.Sedes'  is null.");
            }
            var sede = await _context.Sedes.FindAsync(id);
            if (sede != null)
            {
                _context.Sedes.Remove(sede);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SedeExists(int id)
        {
          return _context.Sedes.Any(e => e.IdSede == id);
        }
    }
}
