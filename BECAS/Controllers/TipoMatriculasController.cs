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
    public class TipoMatriculasController : Controller
    {
        private readonly MEOBContext _context;

        public TipoMatriculasController(MEOBContext context)
        {
            _context = context;
        }

        // GET: TipoMatriculas
        public async Task<IActionResult> Index()
        {
              return View(await _context.TipoMatriculas.ToListAsync());
        }

        // GET: TipoMatriculas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TipoMatriculas == null)
            {
                return NotFound();
            }

            var tipoMatricula = await _context.TipoMatriculas
                .FirstOrDefaultAsync(m => m.IdTipoMatricula == id);
            if (tipoMatricula == null)
            {
                return NotFound();
            }

            return View(tipoMatricula);
        }

        // GET: TipoMatriculas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoMatriculas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTipoMatricula,Nombre,Activo")] TipoMatricula tipoMatricula)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoMatricula);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoMatricula);
        }

        // GET: TipoMatriculas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TipoMatriculas == null)
            {
                return NotFound();
            }

            var tipoMatricula = await _context.TipoMatriculas.FindAsync(id);
            if (tipoMatricula == null)
            {
                return NotFound();
            }
            return View(tipoMatricula);
        }

        // POST: TipoMatriculas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTipoMatricula,Nombre,Activo")] TipoMatricula tipoMatricula)
        {
            if (id != tipoMatricula.IdTipoMatricula)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoMatricula);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoMatriculaExists(tipoMatricula.IdTipoMatricula))
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
            return View(tipoMatricula);
        }

        // GET: TipoMatriculas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TipoMatriculas == null)
            {
                return NotFound();
            }

            var tipoMatricula = await _context.TipoMatriculas
                .FirstOrDefaultAsync(m => m.IdTipoMatricula == id);
            if (tipoMatricula == null)
            {
                return NotFound();
            }

            return View(tipoMatricula);
        }

        // POST: TipoMatriculas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TipoMatriculas == null)
            {
                return Problem("Entity set 'MEOBContext.TipoMatriculas'  is null.");
            }
            var tipoMatricula = await _context.TipoMatriculas.FindAsync(id);
            if (tipoMatricula != null)
            {
                _context.TipoMatriculas.Remove(tipoMatricula);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoMatriculaExists(int id)
        {
          return _context.TipoMatriculas.Any(e => e.IdTipoMatricula == id);
        }
    }
}
