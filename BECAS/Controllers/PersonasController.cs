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
    public class PersonasController : Controller
    {
        private readonly MEOBContext _context;

        public PersonasController(MEOBContext context)
        {
            _context = context;
        }

        // GET: Personas
        public async Task<IActionResult> Index()
        {
            var mEOBContext = _context.Personas.Include(p => p.carrera).Include(p => p.matricula).Include(p => p.programa).Include(p => p.sede).Include(p => p.sexo).Include(p => p.socio);
            return View(await mEOBContext.ToListAsync());
        }

        // GET: Personas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Personas == null)
            {
                return NotFound();
            }

            var persona = await _context.Personas
                .Include(p => p.carrera)
                .Include(p => p.matricula)
                .Include(p => p.programa)
                .Include(p => p.sede)
                .Include(p => p.sexo)
                .Include(p => p.socio)
                .FirstOrDefaultAsync(m => m.IdPersona == id);
            if (persona == null)
            {
                return NotFound();
            }

            return View(persona);
        }

        // GET: Personas/Create
        public IActionResult Create()
        {
            ViewData["CarreraCursoGrado"] = new SelectList(_context.CatCarreras, "IdCatCarrera", "IdCatCarrera");
            ViewData["TipoMatricula"] = new SelectList(_context.TipoMatriculas, "IdTipoMatricula", "IdTipoMatricula");
            ViewData["Programa"] = new SelectList(_context.Programas, "IdPrograma", "IdPrograma");
            ViewData["Sede"] = new SelectList(_context.CatSedes, "IdCatSede", "IdCatSede");
            ViewData["Sexo"] = new SelectList(_context.Sexos, "IdSexo", "IdSexo");
            ViewData["SocioIm"] = new SelectList(_context.SocioImplementadors, "IdImplementador", "IdImplementador");
            return View();
        }

        // POST: Personas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPersona,FechaEntrevista,Id,PIdOim,TipoMatricula,Nombre,Apellido,NombreCompleto,UltimoGradoAprobado,Telefono1,Telefono2,Sexo,FechaNacimiento,Edad,Discapacidad,VictimaViolencia,MigranteRetornado,PiensaMigrar,FamiliaresMigrantes,FamiliaresRetornados,Empleo,Dui,Nie,Correo,Refiere,Departamento,Programa,Cohorte,Sede,SocioIm,Zona,EstadoInscripcion,EstadoMf,CarreraCursoGrado,EstadoPersona,IdCarga")] Persona persona)
        {
            if (ModelState.IsValid)
            {
                _context.Add(persona);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarreraCursoGrado"] = new SelectList(_context.CatCarreras, "IdCatCarrera", "IdCatCarrera", persona.CarreraCursoGrado);
            ViewData["TipoMatricula"] = new SelectList(_context.TipoMatriculas, "IdTipoMatricula", "IdTipoMatricula", persona.TipoMatricula);
            ViewData["Programa"] = new SelectList(_context.Programas, "IdPrograma", "IdPrograma", persona.Programa);
            ViewData["Sede"] = new SelectList(_context.CatSedes, "IdCatSede", "IdCatSede", persona.Sede);
            ViewData["Sexo"] = new SelectList(_context.Sexos, "IdSexo", "IdSexo", persona.Sexo);
            ViewData["SocioIm"] = new SelectList(_context.SocioImplementadors, "IdImplementador", "IdImplementador", persona.SocioIm);
            return View(persona);
        }

        // GET: Personas/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Personas == null)
            {
                return NotFound();
            }

            var persona = await _context.Personas.FirstOrDefaultAsync(x=> x.PIdOim.Equals(id));
            if (persona == null)
            {
                return NotFound();
            }
            ViewData["CarreraCursoGrado"] = new SelectList(_context.CatCarreras, "IdCatCarrera", "IdCatCarrera", persona.CarreraCursoGrado);
            ViewData["TipoMatricula"] = new SelectList(_context.TipoMatriculas, "IdTipoMatricula", "IdTipoMatricula", persona.TipoMatricula);
            ViewData["Programa"] = new SelectList(_context.Programas, "IdPrograma", "IdPrograma", persona.Programa);
            ViewData["Sede"] = new SelectList(_context.CatSedes, "IdCatSede", "IdCatSede", persona.Sede);
            ViewData["Sexo"] = new SelectList(_context.Sexos, "IdSexo", "IdSexo", persona.Sexo);
            ViewData["SocioIm"] = new SelectList(_context.SocioImplementadors, "IdImplementador", "IdImplementador", persona.SocioIm);
            return View(persona);
        }

        // POST: Personas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPersona,FechaEntrevista,Id,PIdOim,TipoMatricula,Nombre,Apellido,NombreCompleto,UltimoGradoAprobado,Telefono1,Telefono2,Sexo,FechaNacimiento,Edad,Discapacidad,VictimaViolencia,MigranteRetornado,PiensaMigrar,FamiliaresMigrantes,FamiliaresRetornados,Empleo,Dui,Nie,Correo,Refiere,Departamento,Programa,Cohorte,Sede,SocioIm,Zona,EstadoInscripcion,EstadoMf,CarreraCursoGrado,EstadoPersona,IdCarga")] Persona persona)
        {
            if (id != persona.IdPersona)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(persona);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonaExists(persona.IdPersona))
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
            ViewData["CarreraCursoGrado"] = new SelectList(_context.CatCarreras, "IdCatCarrera", "IdCatCarrera", persona.CarreraCursoGrado);
            ViewData["TipoMatricula"] = new SelectList(_context.TipoMatriculas, "IdTipoMatricula", "IdTipoMatricula", persona.TipoMatricula);
            ViewData["Programa"] = new SelectList(_context.Programas, "IdPrograma", "IdPrograma", persona.Programa);
            ViewData["Sede"] = new SelectList(_context.CatSedes, "IdCatSede", "IdCatSede", persona.Sede);
            ViewData["Sexo"] = new SelectList(_context.Sexos, "IdSexo", "IdSexo", persona.Sexo);
            ViewData["SocioIm"] = new SelectList(_context.SocioImplementadors, "IdImplementador", "IdImplementador", persona.SocioIm);
            return View(persona);
        }

        // GET: Personas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Personas == null)
            {
                return NotFound();
            }

            var persona = await _context.Personas
                .Include(p => p.carrera)
                .Include(p => p.matricula)
                .Include(p => p.programa)
                .Include(p => p.sede)
                .Include(p => p.sexo)
                .Include(p => p.socio)
                .FirstOrDefaultAsync(m => m.IdPersona == id);
            if (persona == null)
            {
                return NotFound();
            }

            return View(persona);
        }

        // POST: Personas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Personas == null)
            {
                return Problem("Entity set 'MEOBContext.Personas'  is null.");
            }
            var persona = await _context.Personas.FindAsync(id);
            if (persona != null)
            {
                _context.Personas.Remove(persona);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonaExists(int id)
        {
          return _context.Personas.Any(e => e.IdPersona == id);
        }
    }
}
