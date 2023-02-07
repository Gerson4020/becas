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
    public class SocioImplementadorsController : Controller
    {
        private readonly MEOBContext _context;

        public SocioImplementadorsController(MEOBContext context)
        {
            _context = context;
        }

        // GET: SocioImplementadors
        public async Task<IActionResult> Index()
        {
              return View(await _context.SocioImplementadors.ToListAsync());
        }

        // GET: SocioImplementadors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SocioImplementadors == null)
            {
                return NotFound();
            }

            var socioImplementador = await _context.SocioImplementadors
                .FirstOrDefaultAsync(m => m.IdImplementador == id);
            if (socioImplementador == null)
            {
                return NotFound();
            }

            return View(socioImplementador);
        }

        // GET: SocioImplementadors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SocioImplementadors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdImplementador,Nombre,Activo")] SocioImplementador socioImplementador)
        {
            if (ModelState.IsValid)
            {
                _context.Add(socioImplementador);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(socioImplementador);
        }

        // GET: SocioImplementadors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SocioImplementadors == null)
            {
                return NotFound();
            }

            var socioImplementador = await _context.SocioImplementadors.FindAsync(id);
            if (socioImplementador == null)
            {
                return NotFound();
            }
            return View(socioImplementador);
        }

        // POST: SocioImplementadors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdImplementador,Nombre,Activo")] SocioImplementador socioImplementador)
        {
            if (id != socioImplementador.IdImplementador)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(socioImplementador);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SocioImplementadorExists(socioImplementador.IdImplementador))
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
            return View(socioImplementador);
        }

        // GET: SocioImplementadors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SocioImplementadors == null)
            {
                return NotFound();
            }

            var socioImplementador = await _context.SocioImplementadors
                .FirstOrDefaultAsync(m => m.IdImplementador == id);
            if (socioImplementador == null)
            {
                return NotFound();
            }

            return View(socioImplementador);
        }

        // POST: SocioImplementadors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SocioImplementadors == null)
            {
                return Problem("Entity set 'MEOBContext.SocioImplementadors'  is null.");
            }
            var socioImplementador = await _context.SocioImplementadors.FindAsync(id);
            if (socioImplementador != null)
            {
                _context.SocioImplementadors.Remove(socioImplementador);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SocioImplementadorExists(int id)
        {
          return _context.SocioImplementadors.Any(e => e.IdImplementador == id);
        }
    }
}
