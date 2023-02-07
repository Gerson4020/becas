using BECAS.Models;
using BECAS.Models.VM;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BECASLC;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using BECAS.Filters;

namespace BECAS.Controllers
{
    [ResponseHeader]
    public class MantenimientosController : Controller
    {
        private readonly MEOBContext _ctx;
        public MantenimientosController(MEOBContext ctx)
        {
            _ctx = ctx;
        }

        #region MTTO PERSONAS
        // GET: MantenimientosController
        public ActionResult Personas()
        {
            PersonasVM vM = new PersonasVM();
            List<PersonTableVM> ListPersonas = new List<PersonTableVM>();
            ListPersonas = _ctx.Personas.Include(x => x.sede).Include(x => x.matricula).Include(x => x.programa).Include(x => x.socio).Include(x => x.carrera).Include(x => x.refiere).Include(x => x.zona).Select(n => new PersonTableVM
            {
                FechaEntrevista = n.FechaEntrevista,
                Id = n.Id,
                PIdOim = n.PIdOim,
                TipoMatricula = n.matricula,
                Nombre = n.Nombre,
                Apellido = n.Apellido,
                NombreCompleto = n.NombreCompleto,
                UltimoGradoAprobado = n.UltimoGradoAprobado,
                Telefono1 = n.Telefono1,
                Telefono2 = n.Telefono2,
                Sexo = _ctx.Sexos.FirstOrDefault(x => x.IdSexo == n.Sexo),
                FechaNacimiento = n.FechaNacimiento,
                Edad = n.Edad,
                Discapacidad = n.Discapacidad,
                VictimaViolencia = n.VictimaViolencia,
                MigranteRetornado = n.MigranteRetornado,
                PiensaMigrar = n.PiensaMigrar,
                FamiliaresMigrantes = n.FamiliaresMigrantes,
                FamiliaresRetornados = n.FamiliaresRetornados,
                Empleo = n.Empleo,
                Dui = n.Dui,
                Nie = n.Nie,
                Correo = n.Correo,
                Refiere = n.refiere,
                Departamento = _ctx.Departamentos.FirstOrDefault(x => x.IdDepartamento == n.Departamento),
                Programa = n.programa,
                Cohorte = n.Cohorte,
                Sede = n.sede,
                SocioIm = n.socio,
                Zona = n.zona,
                EstadoInscripcion = n.EstadoInscripcion,
                CarreraCursoGrado = n.carrera,
                EstadoMf = n.EstadoMf,
                EstadoPersona = n.EstadoPersona,
            }
            ).ToList();


            List<SocioImplementador> SocioImple = new List<SocioImplementador>();
            SocioImple = _ctx.SocioImplementadors.ToList();

            List<Sexo> sexos = new List<Sexo>();
            sexos = _ctx.Sexos.ToList();

            List<Departamento> departamentos = new List<Departamento>();
            departamentos = _ctx.Departamentos.ToList();

            List<TipoMatricula> ttipomatricula = new List<TipoMatricula>();
            ttipomatricula = _ctx.TipoMatriculas.ToList();

            List<Refiere> refiereCMs = new List<Refiere>();
            refiereCMs = _ctx.Refieres.Where(x => x.Activo == true).ToList();

            vM.Personas = ListPersonas;
            vM.Socios = SocioImple;
            vM.sexos = sexos;
            vM.tipomatricula = ttipomatricula;
            vM.departamentos = departamentos;
            vM.dropRefiereCM = refiereCMs;
            return View(vM);
        }
        [HttpPost]
        public ActionResult Personas(List<int> idprograma, List<int> socios, List<int> sedes, List<int> carreras, List<int> sexos, List<int> tipomatricula, List<int> departament, List<int> refiere, List<int> dropzona)
        {
            List<Persona> ListPersonas = new List<Persona>();
            ListPersonas = _ctx.Personas.Include(x => x.sede).Include(x => x.matricula).Include(x => x.programa).Include(x => x.socio).Include(x => x.carrera).Include(x => x.refiere).Include(x => x.zona).ToList();

            List<Refiere> refiereCMs = new List<Refiere>();
            refiereCMs = _ctx.Refieres.Where(x => x.Activo == true).ToList();

            if (idprograma.Count() != 0)
            {
                ListPersonas = ListPersonas.Where(m => m.Programa != null).ToList();
                ListPersonas = ListPersonas.Where(m => idprograma.Contains((int)m.Programa)).ToList();
            }
            if (departament.Count() != 0)
            {
                ListPersonas = ListPersonas.Where(m => m.Departamento != null).ToList();
                ListPersonas = ListPersonas.Where(m => departament.Contains((int)m.Departamento)).ToList();
            }
            if (socios.Count() != 0)
            {
                ListPersonas = ListPersonas.Where(m => m.SocioIm != null).ToList();
                ListPersonas = ListPersonas.Where(m => socios.Contains((int)m.SocioIm)).ToList();
            }
            if (sedes.Count() != 0)
            {
                for (int i = 0; i < sedes.Count; i++)
                {
                    var IdCatSede = (int)_ctx.Sedes.FirstOrDefault(x => x.IdSede.Equals(sedes.First())).IdCatSede;
                    sedes[i] = IdCatSede;
                }

                ListPersonas = ListPersonas.Where(m => m.Sede != null).ToList();
                if (sedes != null)
                {
                    ListPersonas = ListPersonas.Where(m => sedes.Contains((int)m.Sede)).ToList();
                }
                
            }
            if (carreras.Count() != 0)
            {
                ListPersonas = ListPersonas.Where(m => m.CarreraCursoGrado != null).ToList();

                ListPersonas = ListPersonas.Where(m => carreras.Contains((int)m.CarreraCursoGrado)).ToList();
            }
            if (sexos.Count() != 0)
            {
                ListPersonas = ListPersonas.Where(m => m.Sexo != null).ToList();
                ListPersonas = ListPersonas.Where(m => sexos.Contains((int)m.Sexo)).ToList();
            }
            if (tipomatricula.Count() != 0)
            {
                ListPersonas = ListPersonas.Where(m => m.TipoMatricula != null).ToList();
                ListPersonas = ListPersonas.Where(m => tipomatricula.Contains((int)m.TipoMatricula)).ToList();
            }

            if (refiere.Count() != 0)
            {
                ListPersonas = ListPersonas.Where(m => m.Refiere != null).ToList();
                ListPersonas = ListPersonas.Where(m => refiere.Contains((int)m.Refiere)).ToList();
            }

            if (dropzona.Count() != 0)
            {
                ListPersonas = ListPersonas.Where(m => m.Zona != null).ToList();
                ListPersonas = ListPersonas.Where(m => dropzona.Contains((int)m.Zona)).ToList();
            }
            //DROPS
            PersonasVM vM = new PersonasVM();

            List<Sexo> ssexos = new List<Sexo>();
            ssexos = _ctx.Sexos.ToList();

            List<TipoMatricula> ttipomatricula = new List<TipoMatricula>();
            ttipomatricula = _ctx.TipoMatriculas.ToList();

            List<SocioImplementador> SocioImple = new List<SocioImplementador>();
            SocioImple = _ctx.SocioImplementadors.ToList();

            List<Departamento> departamentos = new List<Departamento>();
            departamentos = _ctx.Departamentos.ToList();

            List<PersonTableVM> ListPersonas2 = new List<PersonTableVM>();
            ListPersonas2 = ListPersonas.Select(n => new PersonTableVM
            {
                FechaEntrevista = n.FechaEntrevista,
                Id = n.Id,
                PIdOim = n.PIdOim,
                TipoMatricula = n.matricula,
                Nombre = n.Nombre,
                Apellido = n.Apellido,
                NombreCompleto = n.NombreCompleto,
                UltimoGradoAprobado = n.UltimoGradoAprobado,
                Telefono1 = n.Telefono1,
                Telefono2 = n.Telefono2,
                Sexo = _ctx.Sexos.FirstOrDefault(x => x.IdSexo == n.Sexo),
                FechaNacimiento = n.FechaNacimiento,
                Edad = n.Edad,
                Discapacidad = n.Discapacidad,
                VictimaViolencia = n.VictimaViolencia,
                MigranteRetornado = n.MigranteRetornado,
                PiensaMigrar = n.PiensaMigrar,
                FamiliaresMigrantes = n.FamiliaresMigrantes,
                FamiliaresRetornados = n.FamiliaresRetornados,
                Empleo = n.Empleo,
                Dui = n.Dui,
                Nie = n.Nie,
                Correo = n.Correo,
                Refiere = n.refiere,
                Departamento = _ctx.Departamentos.FirstOrDefault(x => x.IdDepartamento == n.Departamento),
                Programa = n.programa,
                Cohorte = n.Cohorte,
                Sede = n.sede,
                SocioIm = n.socio,
                Zona = n.zona,
                EstadoInscripcion = n.EstadoInscripcion,
                CarreraCursoGrado = n.carrera,
                EstadoMf = n.EstadoMf,
                EstadoPersona = n.EstadoPersona,
            }
           ).ToList();

            vM.sexos = ssexos;
            vM.Socios = SocioImple;
            vM.Personas = ListPersonas2;
            vM.tipomatricula = ttipomatricula;
            vM.departamentos = departamentos;
            vM.dropRefiereCM = refiereCMs;
            return View(vM);
        }

        public ActionResult PersonaDetalle(string id)
        {
            try
            {
                List<CargaEducacion> cargaEducacions = new List<CargaEducacion>();
                cargaEducacions = _ctx.CargaEducacions.Where(x => x.PIdOim.Equals(id)).ToList();

                List<CargaEvaluacionPsicosocial> CargaEvaluacionPsicosocial = new List<CargaEvaluacionPsicosocial>();
                CargaEvaluacionPsicosocial = _ctx.CargaEvaluacionPsicosocials.Where(x => x.PId.Equals(id)).ToList();

                List<CargaSeguimientoPsicosocial> CargaSeguimientoPsicosocial = new List<CargaSeguimientoPsicosocial>();
                CargaSeguimientoPsicosocial = _ctx.CargaSeguimientoPsicosocials.Where(x => x.PId.Equals(id)).ToList();

                List<CargaSeguimientoPracticasPr> CargaSeguimientoPracticasPr = new List<CargaSeguimientoPracticasPr>();
                CargaSeguimientoPracticasPr = _ctx.CargaSeguimientoPracticasPrs.Where(x => x.PId.Equals(id)).ToList();

                List<CargaEstipendio> CargaEstipendio = new List<CargaEstipendio>();
                CargaEstipendio = _ctx.CargaEstipendios.Where(x => x.PId.Equals(id)).ToList();

                List<CargaSeguimientoPasantia> pasantias = new List<CargaSeguimientoPasantia>();
                pasantias = _ctx.CargaSeguimientoPasantias.Where(x => x.PId.Equals(id)).ToList();

                List<CargaSeguimientoAutoempleo> autoempleos = new List<CargaSeguimientoAutoempleo>();
                autoempleos = _ctx.CargaSeguimientoAutoempleos.Where(x => x.PId.Equals(id)).ToList();

                DetallePersonaVM vM = new DetallePersonaVM();
                vM.cargaEducacions = cargaEducacions;
                vM.cargaSeguimientoPsicosocial = CargaSeguimientoPsicosocial;
                vM.cargaSeguimientoPracticasPr = CargaSeguimientoPracticasPr;
                vM.cargaEvaluacionPsicosocial = CargaEvaluacionPsicosocial;
                vM.cargaEstipendio = CargaEstipendio;
                vM.pasantias = pasantias;
                vM.empleo = autoempleos;
                return View(vM);
            }
            catch (Exception ex)
            {
                string msn = ex.Message;
                throw;
            }
        }

        public async Task<JsonResult> GetZona(int id)
        {
            try
            {
                var socio = _ctx.Sedes.Include(x => x.catsede).Include(i => i.zona).Where(x => x.IdSocio == id).Select(z => z.zona).Distinct().ToList();
                var sedes = socio;
                return Json(sedes);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<JsonResult> GetSedes(int id, int ids, int idp)
        {
            try
            {
                var sedes = _ctx.Sedes.Include(x => x.catsede).Where(x => x.IdZona == id && x.IdSocio == ids && x.IdPrograma == idp && x.Activo == true).Distinct().ToList();
                return Json(sedes);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<JsonResult> GetProgramas(int id, int ids)
        {
            try
            {
                var sedes = _ctx.Sedes.Include(x => x.programa).Where(x => x.IdZona == id && x.IdSocio == ids && x.Activo == true).Select(p => p.programa).Distinct().ToList();
                return Json(sedes);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<JsonResult> GetDepartamentos(int id)
        {
            try
            {
                var sedes = _ctx.Sedes.Include(x => x.programa).Where(x => x.IdZona == id && x.Activo == true).Select(p => p.programa).Distinct().ToList();
                return Json(sedes);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<JsonResult> GetCarreras(int id, int ids)
        {
            try
            {
                if (ids != 1 && ids != 3)
                {
                    var Lcarreras = _ctx.Carreras.Include(x => x.carrera).Where(x => x.IdSede.Equals(id)).ToList();
                    List<CarreraListaCm> carreras = new List<CarreraListaCm>();
                    foreach (var c in Lcarreras)
                    {
                        CarreraListaCm cm = new CarreraListaCm();
                        cm.idSede = c.IdSede;
                        cm.idCarrera = (int)c.IdCatCarrera;
                        cm.nombre = c.carrera.Nombre;
                        cm.activo = c.Activo;
                        cm.IsCarrera = true;
                        cm.cohorte = c.Cohorte;
                        carreras.Add(cm);
                    }
                    return Json(carreras);
                }
                else
                {
                    var LGracodos = _ctx.GradoSedes.Include(x => x.grado).Where(x => x.IdSede.Equals(id)).ToList();
                    List<CarreraListaCm> grados = new List<CarreraListaCm>();
                    foreach (var c in LGracodos)
                    {
                        CarreraListaCm cm = new CarreraListaCm();
                        cm.idSede = c.IdSede;
                        cm.idCarrera = (int)c.IdGrado;
                        cm.nombre = c.grado.Nombre;
                        cm.activo = c.Activo;
                        cm.IsCarrera = false;
                        cm.cohorte = 0;
                        grados.Add(cm);
                    }
                    return Json(grados);
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        #endregion

        #region Evaluacion Psicosocial
        public async Task<ActionResult> EvalucionPsicosocial()
        {
            try
            {
                EvalucionPsicosocialVM vM = new EvalucionPsicosocialVM();
                var evpsi = await _ctx.CargaEvaluacionPsicosocials.Select(x =>
                    new EvaluscionPsicosicialTable
                    {
                        persona = _ctx.Personas.Include(x => x.sede).Include(x => x.matricula).Include(x => x.programa).Include(x => x.socio).Include(x => x.carrera).Include(x => x.refiere).FirstOrDefault(n => n.PIdOim == x.PId),
                        IdCargaEvaluacionPsicosocial = x.IdCargaEvaluacionPsicosocial,
                        PId = x.PId,
                        OvParticipacion = x.OvParticipacion,
                        OvPuntajePret = x.OvPuntajePret,
                        OvPuntajePos = x.OvPuntajePos,
                        EpInstrumentoRiesgo = x.EpInstrumentoRiesgo,
                        EpVulnerabilidades = x.EpVulnerabilidades,
                        EpAlertaDesercion = x.EpAlertaDesercion,
                        Mes = x.Mes,
                        Año = x.Año
                    }
                    ).ToListAsync();

                List<Sexo> ssexos = new List<Sexo>();
                ssexos = _ctx.Sexos.ToList();

                List<TipoMatricula> ttipomatricula = new List<TipoMatricula>();
                ttipomatricula = _ctx.TipoMatriculas.ToList();

                List<SocioImplementador> SocioImple = new List<SocioImplementador>();
                SocioImple = _ctx.SocioImplementadors.ToList();

                List<Departamento> departamentos = new List<Departamento>();
                departamentos = _ctx.Departamentos.ToList();

                List<Refiere> refiereCMs = new List<Refiere>();
                refiereCMs = _ctx.Refieres.Where(x => x.Activo == true).ToList();

                vM.ListaEvaPsicosocial = evpsi;

                vM.Socios = SocioImple;
                vM.sexos = ssexos;
                vM.tipomatricula = ttipomatricula;
                vM.departamentos = departamentos;
                vM.dropRefiereCM = refiereCMs;
                return View(vM);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> EvalucionPsicosocial(List<int> idprograma, List<int> socios, List<int> sedes, List<int> carreras, List<int> sexos, List<int> tipomatricula, List<int> departament, List<int> refiere)
        {
            try
            {
                EvalucionPsicosocialVM vM = new EvalucionPsicosocialVM();
                var evpsi = await _ctx.CargaEvaluacionPsicosocials.Select(x =>
                    new EvaluscionPsicosicialTable
                    {
                        persona = _ctx.Personas.Include(x => x.sede).Include(x => x.matricula).Include(x => x.programa).Include(x => x.socio).Include(x => x.carrera).Include(x => x.refiere).FirstOrDefault(n => n.PIdOim == x.PId),
                        IdCargaEvaluacionPsicosocial = x.IdCargaEvaluacionPsicosocial,
                        PId = x.PId,
                        OvParticipacion = x.OvParticipacion,
                        OvPuntajePret = x.OvPuntajePret,
                        OvPuntajePos = x.OvPuntajePos,
                        EpInstrumentoRiesgo = x.EpInstrumentoRiesgo,
                        EpVulnerabilidades = x.EpVulnerabilidades,
                        EpAlertaDesercion = x.EpAlertaDesercion,
                        Mes = x.Mes,
                        Año = x.Año
                    }
                    ).ToListAsync();
                if (socios.Count() != 0)
                {
                    evpsi = evpsi.Where(m => m.persona != null).ToList();
                    evpsi = evpsi.Where(m => m.persona.SocioIm != null).ToList();
                    evpsi = evpsi.Where(x => socios.Contains((int)x.persona.SocioIm)).ToList();
                }

                if (sedes.Count() != 0)
                {
                    evpsi = evpsi.Where(m => m.persona != null).ToList();
                    evpsi = evpsi.Where(m => m.persona.Sede != null).ToList();
                    evpsi = evpsi.Where(x => sedes.Contains((int)x.persona.Sede)).ToList();
                }

                if (carreras.Count() != 0)
                {
                    evpsi = evpsi.Where(m => m.persona != null).ToList();
                    evpsi = evpsi.Where(m => m.persona.CarreraCursoGrado != null).ToList();
                    evpsi = evpsi.Where(x => carreras.Contains((int)x.persona.CarreraCursoGrado)).ToList();
                }

                if (sexos.Count() != 0)
                {
                    evpsi = evpsi.Where(m => m.persona != null).ToList();
                    evpsi = evpsi.Where(m => m.persona.Sexo != null).ToList();
                    evpsi = evpsi.Where(x => sexos.Contains((int)x.persona.Sexo)).ToList();
                }

                if (departament.Count() != 0)
                {
                    evpsi = evpsi.Where(m => m.persona != null).ToList();
                    evpsi = evpsi.Where(m => m.persona.Departamento != null).ToList();
                    evpsi = evpsi.Where(x => departament.Contains((int)x.persona.Departamento)).ToList();
                }

                if (refiere.Count() != 0)
                {
                    evpsi = evpsi.Where(m => m.persona != null).ToList();
                    evpsi = evpsi.Where(m => m.persona.Refiere != null).ToList();
                    evpsi = evpsi.Where(x => refiere.Contains((int)x.persona.Refiere)).ToList();
                }

                if (idprograma.Count() != 0)
                {
                    evpsi = evpsi.Where(m => m.persona != null).ToList();
                    evpsi = evpsi.Where(m => m.persona.Programa != null).ToList();
                    evpsi = evpsi.Where(x => idprograma.Contains((int)x.persona.Programa)).ToList();
                }

                List<SocioImplementador> SocioImple = new List<SocioImplementador>();
                SocioImple = _ctx.SocioImplementadors.ToList();

                List<Sexo> ssexos = new List<Sexo>();
                ssexos = _ctx.Sexos.ToList();

                List<Departamento> departamentos = new List<Departamento>();
                departamentos = _ctx.Departamentos.ToList();

                List<TipoMatricula> ttipomatricula = new List<TipoMatricula>();
                ttipomatricula = _ctx.TipoMatriculas.ToList();

                List<Refiere> refiereCMs = new List<Refiere>();
                refiereCMs = _ctx.Refieres.Where(x => x.Activo == true).ToList();

                vM.ListaEvaPsicosocial = evpsi;

                vM.Socios = SocioImple;
                vM.sexos = ssexos;
                vM.tipomatricula = ttipomatricula;
                vM.departamentos = departamentos;
                vM.dropRefiereCM = refiereCMs;
                return View(vM);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region SeguimientoAutoempleo
        public async Task<IActionResult> SeguimientoAutoempleo()
        {
            try
            {
                SeguimientoAutoempleoVM vM = new SeguimientoAutoempleoVM();
                var auto = await _ctx.CargaSeguimientoAutoempleos.Select(x =>
                    new SeguimientoAutoempleoTable
                    {
                        IdSeguimientoAutoempleo = x.IdSeguimientoAutoempleo,
                        PId = x.PId,
                        AutoempEmpresa = x.AutoempEmpresa,
                        AutoempTipoCapital = x.AutoempTipoCapital,
                        AutoempEstado = x.AutoempEstado,
                        AutoempTipoFinanciamiento = x.AutoempTipoFinanciamiento,
                        AutoempTipoEmpresa = x.AutoempTipoEmpresa,
                        AutoempTipoEmpresaOtro = x.AutoempTipoEmpresaOtro,
                        AutoempPlanNegocios = x.AutoempPlanNegocios,
                        AutoempRegistro = x.AutoempRegistro,
                        AutoempFechaInicio = x.AutoempFechaInicio,
                        Año = x.Año,
                        Mes = x.Mes,
                        persona = _ctx.Personas.Include(x => x.sede).Include(x => x.matricula).Include(x => x.programa).Include(x => x.socio).Include(x => x.carrera).Include(x => x.refiere).FirstOrDefault(n => n.PIdOim == x.PId)
                    }
                    ).ToListAsync();

                List<SocioImplementador> SocioImple = new List<SocioImplementador>();
                SocioImple = _ctx.SocioImplementadors.ToList();

                List<Sexo> ssexos = new List<Sexo>();
                ssexos = _ctx.Sexos.ToList();

                List<Departamento> departamentos = new List<Departamento>();
                departamentos = _ctx.Departamentos.ToList();

                List<TipoMatricula> ttipomatricula = new List<TipoMatricula>();
                ttipomatricula = _ctx.TipoMatriculas.ToList();

                List<Refiere> refiereCMs = new List<Refiere>();
                refiereCMs = _ctx.Refieres.Where(x => x.Activo == true).ToList();

                vM.AutoempleoTables = auto;

                vM.Socios = SocioImple;
                vM.sexos = ssexos;
                vM.tipomatricula = ttipomatricula;
                vM.departamentos = departamentos;
                vM.dropRefiereCM = refiereCMs;
                return View(vM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> SeguimientoAutoempleo(List<int> idprograma, List<int> socios, List<int> sedes, List<int> carreras, List<int> sexos, List<int> tipomatricula, List<int> departament, List<int> refiere)
        {
            try
            {
                SeguimientoAutoempleoVM vM = new SeguimientoAutoempleoVM();
                var auto = await _ctx.CargaSeguimientoAutoempleos.Select(x =>
                    new SeguimientoAutoempleoTable
                    {
                        IdSeguimientoAutoempleo = x.IdSeguimientoAutoempleo,
                        PId = x.PId,
                        AutoempEmpresa = x.AutoempEmpresa,
                        AutoempTipoCapital = x.AutoempTipoCapital,
                        AutoempEstado = x.AutoempEstado,
                        AutoempTipoFinanciamiento = x.AutoempTipoFinanciamiento,
                        AutoempTipoEmpresa = x.AutoempTipoEmpresa,
                        AutoempTipoEmpresaOtro = x.AutoempTipoEmpresaOtro,
                        AutoempPlanNegocios = x.AutoempPlanNegocios,
                        AutoempRegistro = x.AutoempRegistro,
                        AutoempFechaInicio = x.AutoempFechaInicio,
                        Año = x.Año,
                        Mes = x.Mes,
                        persona = _ctx.Personas.Include(f => f.programa).Include(f => f.sexo).Include(f => f.socio).Include(f => f.matricula).Include(f => f.carrera).Include(f => f.sede).FirstOrDefault(n => n.PIdOim == x.PId)
                    }
                    ).ToListAsync();
                if (socios.Count() != 0)
                {
                    auto = auto.Where(m => m.persona != null).ToList();
                    auto = auto.Where(m => m.persona.SocioIm != null).ToList();
                    auto = auto.Where(x => socios.Contains((int)x.persona.SocioIm)).ToList();
                }

                if (sedes.Count() != 0)
                {
                    auto = auto.Where(m => m.persona != null).ToList();
                    auto = auto.Where(m => m.persona.Sede != null).ToList();
                    auto = auto.Where(x => sedes.Contains((int)x.persona.Sede)).ToList();
                }

                if (carreras.Count() != 0)
                {
                    auto = auto.Where(m => m.persona != null).ToList();
                    auto = auto.Where(m => m.persona.CarreraCursoGrado != null).ToList();
                    auto = auto.Where(x => carreras.Contains((int)x.persona.CarreraCursoGrado)).ToList();
                }

                if (sexos.Count() != 0)
                {
                    auto = auto.Where(m => m.persona != null).ToList();
                    auto = auto.Where(m => m.persona.Sexo != null).ToList();
                    auto = auto.Where(x => sexos.Contains((int)x.persona.Sexo)).ToList();
                }

                if (departament.Count() != 0)
                {
                    auto = auto.Where(m => m.persona != null).ToList();
                    auto = auto.Where(m => m.persona.Departamento != null).ToList();
                    auto = auto.Where(x => departament.Contains((int)x.persona.Departamento)).ToList();
                }

                if (refiere.Count() != 0)
                {
                    auto = auto.Where(m => m.persona != null).ToList();
                    auto = auto.Where(m => m.persona.Refiere != null).ToList();
                    auto = auto.Where(x => refiere.Contains((int)x.persona.Refiere)).ToList();
                }

                if (idprograma.Count() != 0)
                {
                    auto = auto.Where(m => m.persona != null).ToList();
                    auto = auto.Where(m => m.persona.Programa != null).ToList();
                    auto = auto.Where(x => idprograma.Contains((int)x.persona.Programa)).ToList();
                }

                List<SocioImplementador> SocioImple = new List<SocioImplementador>();
                SocioImple = _ctx.SocioImplementadors.ToList();

                List<Sexo> ssexos = new List<Sexo>();
                ssexos = _ctx.Sexos.ToList();

                List<Departamento> departamentos = new List<Departamento>();
                departamentos = _ctx.Departamentos.ToList();

                List<TipoMatricula> ttipomatricula = new List<TipoMatricula>();
                ttipomatricula = _ctx.TipoMatriculas.ToList();

                List<Refiere> refiereCMs = new List<Refiere>();
                refiereCMs = _ctx.Refieres.Where(x => x.Activo == true).ToList();

                vM.AutoempleoTables = auto;

                vM.Socios = SocioImple;
                vM.sexos = ssexos;
                vM.tipomatricula = ttipomatricula;
                vM.departamentos = departamentos;
                vM.dropRefiereCM = refiereCMs;
                return View(vM);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region SeguimientoPracticasPr
        public async Task<IActionResult> SeguimientoPracticasPr()
        {
            try
            {
                SeguimientoPracticasPrVM vM = new SeguimientoPracticasPrVM();
                var practicas = await _ctx.CargaSeguimientoPracticasPrs.Select(x =>
                    new SeguimientoPracticasPrTable
                    {
                        PId = x.PId,
                        PpEmpresa = x.PpEmpresa,
                        PpCargo = x.PpCargo,
                        PpDocenteAsign = x.PpDocenteAsign,
                        PpGestion = x.PpGestion,
                        PpMontoRemuneracion = x.PpMontoRemuneracion,
                        PpPosibilidadContratacion = x.PpPosibilidadContratacion,
                        Año = x.Año,
                        Mes = x.Mes,
                        persona = _ctx.Personas.Include(f => f.programa).Include(f => f.sexo).Include(f => f.socio).Include(f => f.matricula).Include(f => f.carrera).Include(f => f.sede).FirstOrDefault(n => n.PIdOim == x.PId)
                    }
                    ).ToListAsync();

                List<SocioImplementador> SocioImple = new List<SocioImplementador>();
                SocioImple = _ctx.SocioImplementadors.ToList();

                List<Sexo> ssexos = new List<Sexo>();
                ssexos = _ctx.Sexos.ToList();

                List<Departamento> departamentos = new List<Departamento>();
                departamentos = _ctx.Departamentos.ToList();

                List<TipoMatricula> ttipomatricula = new List<TipoMatricula>();
                ttipomatricula = _ctx.TipoMatriculas.ToList();

                List<Refiere> refiereCMs = new List<Refiere>();
                refiereCMs = _ctx.Refieres.Where(x => x.Activo == true).ToList();


                vM.ListaPracticas = practicas;

                vM.Socios = SocioImple;
                vM.sexos = ssexos;
                vM.tipomatricula = ttipomatricula;
                vM.departamentos = departamentos;
                vM.dropRefiereCM = refiereCMs;
                return View(vM);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> SeguimientoPracticasPr(List<int> idprograma, List<int> socios, List<int> sedes, List<int> carreras, List<int> sexos, List<int> tipomatricula, List<int> departament, List<int> refiere)
        {
            try
            {
                SeguimientoPracticasPrVM vM = new SeguimientoPracticasPrVM();
                var practicas = await _ctx.CargaSeguimientoPracticasPrs.Select(x =>
                    new SeguimientoPracticasPrTable
                    {
                        PId = x.PId,
                        PpEmpresa = x.PpEmpresa,
                        PpCargo = x.PpCargo,
                        PpDocenteAsign = x.PpDocenteAsign,
                        PpGestion = x.PpGestion,
                        PpMontoRemuneracion = x.PpMontoRemuneracion,
                        PpPosibilidadContratacion = x.PpPosibilidadContratacion,
                        Año = x.Año,
                        Mes = x.Mes,
                        persona = _ctx.Personas.Include(x => x.sede).Include(x => x.matricula).Include(x => x.programa).Include(x => x.socio).Include(x => x.carrera).Include(x => x.refiere).FirstOrDefault(n => n.PIdOim == x.PId)
                    }
                    ).ToListAsync();

                if (socios.Count() != 0)
                {
                    practicas = practicas.Where(m => m.persona != null).ToList();
                    practicas = practicas.Where(m => m.persona.SocioIm != null).ToList();
                    practicas = practicas.Where(x => socios.Contains((int)x.persona.SocioIm)).ToList();
                }

                if (sedes.Count() != 0)
                {
                    practicas = practicas.Where(m => m.persona != null).ToList();
                    practicas = practicas.Where(m => m.persona.Sede != null).ToList();
                    practicas = practicas.Where(x => sedes.Contains((int)x.persona.Sede)).ToList();
                }

                if (carreras.Count() != 0)
                {
                    practicas = practicas.Where(m => m.persona != null).ToList();
                    practicas = practicas.Where(m => m.persona.CarreraCursoGrado != null).ToList();
                    practicas = practicas.Where(x => carreras.Contains((int)x.persona.CarreraCursoGrado)).ToList();
                }

                if (sexos.Count() != 0)
                {
                    practicas = practicas.Where(m => m.persona != null).ToList();
                    practicas = practicas.Where(m => m.persona.Sexo != null).ToList();
                    practicas = practicas.Where(x => sexos.Contains((int)x.persona.Sexo)).ToList();
                }

                if (departament.Count() != 0)
                {
                    practicas = practicas.Where(m => m.persona != null).ToList();
                    practicas = practicas.Where(m => m.persona.Departamento != null).ToList();
                    practicas = practicas.Where(x => departament.Contains((int)x.persona.Departamento)).ToList();
                }

                if (refiere.Count() != 0)
                {
                    practicas = practicas.Where(m => m.persona != null).ToList();
                    practicas = practicas.Where(m => m.persona.Refiere != null).ToList();
                    practicas = practicas.Where(x => refiere.Contains((int)x.persona.Refiere)).ToList();
                }

                if (idprograma.Count() != 0)
                {
                    practicas = practicas.Where(m => m.persona != null).ToList();
                    practicas = practicas.Where(m => m.persona.Programa != null).ToList();
                    practicas = practicas.Where(x => idprograma.Contains((int)x.persona.Programa)).ToList();
                }

                List<SocioImplementador> SocioImple = new List<SocioImplementador>();
                SocioImple = _ctx.SocioImplementadors.ToList();

                List<Sexo> ssexos = new List<Sexo>();
                ssexos = _ctx.Sexos.ToList();

                List<Departamento> departamentos = new List<Departamento>();
                departamentos = _ctx.Departamentos.ToList();

                List<TipoMatricula> ttipomatricula = new List<TipoMatricula>();
                ttipomatricula = _ctx.TipoMatriculas.ToList();

                List<Refiere> refiereCMs = new List<Refiere>();
                refiereCMs = _ctx.Refieres.Where(x => x.Activo == true).ToList();

                vM.ListaPracticas = practicas;

                vM.Socios = SocioImple;
                vM.sexos = ssexos;
                vM.tipomatricula = ttipomatricula;
                vM.departamentos = departamentos;
                vM.dropRefiereCM = refiereCMs;
                return View(vM);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Educacion
        public async Task<ActionResult> Educacion()
        {
            try
            {
                EducacionVM vM = new EducacionVM();
                var educacion = await _ctx.CargaEducacions.Select(x =>
                    new EducacionTable
                    {
                        IdCargaEducacion = x.IdCargaEducacion,
                        PIdOim = x.PIdOim,
                        RAño = x.RAño,
                        RMes = x.RMes,
                        DFechaReasg = x.DFechaReasg,
                        DEstado = x.DEstado,
                        DFechades = x.DFechades,
                        DMotivodesercion = x.DMotivodesercion,
                        IDiasAsistenciaEstablecidos = x.IDiasAsistenciaEstablecidos,
                        IDiasAsistenciaEfectivos = x.IDiasAsistenciaEfectivos,
                        IMotivoInasistencia = x.IMotivoInasistencia,
                        IModulosInscritos = x.IModulosInscritos,
                        IModulosAprobados = x.IModulosAprobados,
                        IModulosReprobados = x.IModulosReprobados,
                        persona = _ctx.Personas.Include(f => f.programa).Include(f => f.sexo).Include(f => f.socio).Include(f => f.matricula).Include(f => f.carrera).Include(f => f.sede).Include(x => x.zona).FirstOrDefault(n => n.PIdOim == x.PIdOim)

                    }
                    ).ToListAsync();

                List<SocioImplementador> SocioImple = new List<SocioImplementador>();
                SocioImple = _ctx.SocioImplementadors.ToList();

                List<Sexo> ssexos = new List<Sexo>();
                ssexos = _ctx.Sexos.ToList();

                List<Departamento> departamentos = new List<Departamento>();
                departamentos = _ctx.Departamentos.ToList();

                List<TipoMatricula> ttipomatricula = new List<TipoMatricula>();
                ttipomatricula = _ctx.TipoMatriculas.ToList();

                List<Refiere> refiereCMs = new List<Refiere>();
                refiereCMs = _ctx.Refieres.Where(x => x.Activo == true).ToList();

                List<CatAño> year = new List<CatAño>();
                year = _ctx.CatAños.Where(x => x.Activo == true).ToList();

                List<CatMe> mont = new List<CatMe>();
                mont = _ctx.CatMes.Where(x => x.Activo == true).ToList();


                vM.Socios = SocioImple;
                vM.sexos = ssexos;
                vM.tipomatricula = ttipomatricula;
                vM.departamentos = departamentos;
                vM.dropRefiereCM = refiereCMs;
                vM.catAños = year;
                vM.mes = mont;

                vM.EducacionTable = educacion;
                return View(vM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult> Educacion(List<int> idprograma, List<int> socios, List<int> sedes, List<int> carreras, List<int> sexos, List<int> tipomatricula, List<int> departament, List<int> refiere, List<int> dropzona, List<int> year1, List<string> mes, List<int> cohorte1)
        {
            try
            {
                EducacionVM vM = new EducacionVM();
                var educacion = await _ctx.CargaEducacions.Select(x =>
                    new EducacionTable
                    {
                        IdCargaEducacion = x.IdCargaEducacion,
                        PIdOim = x.PIdOim,
                        RAño = x.RAño,
                        RMes = x.RMes,
                        DFechaReasg = x.DFechaReasg,
                        DEstado = x.DEstado,
                        DFechades = x.DFechades,
                        DMotivodesercion = x.DMotivodesercion,
                        IDiasAsistenciaEstablecidos = x.IDiasAsistenciaEstablecidos,
                        IDiasAsistenciaEfectivos = x.IDiasAsistenciaEfectivos,
                        IMotivoInasistencia = x.IMotivoInasistencia,
                        IModulosInscritos = x.IModulosInscritos,
                        IModulosAprobados = x.IModulosAprobados,
                        IModulosReprobados = x.IModulosReprobados,
                        persona = _ctx.Personas.Include(f => f.programa).Include(f => f.sexo).Include(f => f.socio).Include(f => f.matricula).Include(f => f.carrera).Include(f => f.sede).Include(x => x.zona).FirstOrDefault(n => n.PIdOim == x.PIdOim)

                    }
                    ).ToListAsync();

                if (socios.Count() != 0)
                {
                    educacion = educacion.Where(m => m.persona != null).ToList();
                    educacion = educacion.Where(m => m.persona.SocioIm != null).ToList();
                    educacion = educacion.Where(x => socios.Contains((int)x.persona.SocioIm)).ToList();
                }

                if (sedes.Count() != 0)
                {
                    educacion = educacion.Where(m => m.persona != null).ToList();
                    educacion = educacion.Where(m => m.persona.Sede != null).ToList();
                    educacion = educacion.Where(x => sedes.Contains((int)x.persona.Sede)).ToList();
                }

                if (carreras.Count() != 0)
                {
                    educacion = educacion.Where(m => m.persona != null).ToList();
                    educacion = educacion.Where(m => m.persona.CarreraCursoGrado != null).ToList();
                    educacion = educacion.Where(x => carreras.Contains((int)x.persona.CarreraCursoGrado)).ToList();
                }

                if (sexos.Count() != 0)
                {
                    educacion = educacion.Where(m => m.persona != null).ToList();
                    educacion = educacion.Where(m => m.persona.Sexo != null).ToList();
                    educacion = educacion.Where(x => sexos.Contains((int)x.persona.Sexo)).ToList();
                }

                if (departament.Count() != 0)
                {
                    educacion = educacion.Where(m => m.persona != null).ToList();
                    educacion = educacion.Where(m => m.persona.Departamento != null).ToList();
                    educacion = educacion.Where(x => departament.Contains((int)x.persona.Departamento)).ToList();
                }

                if (refiere.Count() != 0)
                {
                    educacion = educacion.Where(m => m.persona != null).ToList();
                    educacion = educacion.Where(m => m.persona.Refiere != null).ToList();
                    educacion = educacion.Where(x => refiere.Contains((int)x.persona.Refiere)).ToList();
                }

                if (idprograma.Count() != 0)
                {
                    educacion = educacion.Where(m => m.persona != null).ToList();
                    educacion = educacion.Where(m => m.persona.Programa != null).ToList();
                    educacion = educacion.Where(x => idprograma.Contains((int)x.persona.Programa)).ToList();
                }

                if (dropzona.Count() != 0)
                {
                    educacion = educacion.Where(m => m.persona != null).ToList();
                    educacion = educacion.Where(m => m.persona.Zona != null).ToList();
                    educacion = educacion.Where(x => dropzona.Contains((int)x.persona.Programa)).ToList();
                }

                if (year1.Count() != 0)
                {
                    educacion = educacion.Where(x => year1.Contains((int)x.RAño)).ToList();
                }

                if (mes.Count() != 0)
                {
                    educacion = educacion.Where(x => mes.Contains(x.RMes)).ToList();
                }

                if (cohorte1.Count() != 0)
                {
                    educacion = educacion.Where(m => m.persona != null).ToList();
                    educacion = educacion.Where(m => m.persona.Cohorte != null).ToList();
                    //educacion = educacion.Where(x => cohorte1.Contains((int)x.persona.Cohorte)).ToList();
                }

                List<SocioImplementador> SocioImple = new List<SocioImplementador>();
                SocioImple = _ctx.SocioImplementadors.ToList();

                List<Sexo> ssexos = new List<Sexo>();
                ssexos = _ctx.Sexos.ToList();

                List<Departamento> departamentos = new List<Departamento>();
                departamentos = _ctx.Departamentos.ToList();

                List<TipoMatricula> ttipomatricula = new List<TipoMatricula>();
                ttipomatricula = _ctx.TipoMatriculas.ToList();

                List<Refiere> refiereCMs = new List<Refiere>();
                refiereCMs = _ctx.Refieres.Where(x => x.Activo == true).ToList();

                List<CatAño> year = new List<CatAño>();
                year = _ctx.CatAños.Where(x => x.Activo == true).ToList();

                List<CatMe> mont = new List<CatMe>();
                mont = _ctx.CatMes.Where(x => x.Activo == true).ToList();

                List<Cohorte> cohortes = new List<Cohorte>();
                cohortes = _ctx.Cohortes.Where(x => x.Activo == true).ToList();


                vM.Socios = SocioImple;
                vM.sexos = ssexos;
                vM.tipomatricula = ttipomatricula;
                vM.departamentos = departamentos;
                vM.dropRefiereCM = refiereCMs;
                vM.catAños = year;
                vM.mes = mont;

                vM.EducacionTable = educacion;
                return View(vM);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Seguimiento pasantias
        public async Task<ActionResult> pasantias()
        {
            try
            {
                CargaSeguimientoPasantiaVM vM = new CargaSeguimientoPasantiaVM();
                var pasantillas = await _ctx.CargaSeguimientoPasantias.Select(x =>
                    new CargaSeguimientoPasantiaTable
                    {
                        IdSeguimientoPasantias = x.IdSeguimientoPasantias,
                        PId = x.PId,
                        Año = x.Año,
                        Mes = x.Mes,
                        PasEmpresa = x.PasEmpresa,
                        PasEntrevista = x.PasEntrevista,
                        PasPruebas = x.PasPruebas,
                        PasContratacion = x.PasContratacion,
                        PasCargo = x.PasCargo,
                        PasFechaContratacion = x.PasFechaContratacion,
                        PasMontoRemuneracion = x.PasMontoRemuneracion,
                        IdCarga = x.IdCarga,
                        persona = _ctx.Personas.Include(x => x.sede).Include(x => x.matricula).Include(x => x.programa).Include(x => x.socio).Include(x => x.carrera).Include(x => x.refiere).FirstOrDefault(n => n.PIdOim == x.PId)

                    }
                    ).ToListAsync();
                List<SocioImplementador> SocioImple = new List<SocioImplementador>();
                SocioImple = _ctx.SocioImplementadors.ToList();

                List<Sexo> ssexos = new List<Sexo>();
                ssexos = _ctx.Sexos.ToList();

                List<Departamento> departamentos = new List<Departamento>();
                departamentos = _ctx.Departamentos.ToList();

                List<TipoMatricula> ttipomatricula = new List<TipoMatricula>();
                ttipomatricula = _ctx.TipoMatriculas.ToList();

                List<Refiere> refiereCMs = new List<Refiere>();
                refiereCMs = _ctx.Refieres.Where(x => x.Activo == true).ToList();


                vM.Socios = SocioImple;
                vM.sexos = ssexos;
                vM.tipomatricula = ttipomatricula;
                vM.departamentos = departamentos;
                vM.dropRefiereCM = refiereCMs;

                vM.seguimientopasantillas = pasantillas;
                return View(vM);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<ActionResult> pasantias(List<int> idprograma, List<int> socios, List<int> sedes, List<int> carreras, List<int> sexos, List<int> tipomatricula, List<int> departament, List<int> refiere)
        {
            try
            {
                CargaSeguimientoPasantiaVM vM = new CargaSeguimientoPasantiaVM();
                var pasantillas = await _ctx.CargaSeguimientoPasantias.Select(x =>
                    new CargaSeguimientoPasantiaTable
                    {
                        IdSeguimientoPasantias = x.IdSeguimientoPasantias,
                        PId = x.PId,
                        Año = x.Año,
                        Mes = x.Mes,
                        PasEmpresa = x.PasEmpresa,
                        PasEntrevista = x.PasEntrevista,
                        PasPruebas = x.PasPruebas,
                        PasContratacion = x.PasContratacion,
                        PasCargo = x.PasCargo,
                        PasFechaContratacion = x.PasFechaContratacion,
                        PasMontoRemuneracion = x.PasMontoRemuneracion,
                        IdCarga = x.IdCarga,
                        persona = _ctx.Personas.Include(x => x.sede).Include(x => x.matricula).Include(x => x.programa).Include(x => x.socio).Include(x => x.carrera).Include(x => x.refiere).FirstOrDefault(n => n.PIdOim == x.PId)

                    }
                    ).ToListAsync();

                if (socios.Count() != 0)
                {
                    pasantillas = pasantillas.Where(m => m.persona != null).ToList();
                    pasantillas = pasantillas.Where(m => m.persona.SocioIm != null).ToList();
                    pasantillas = pasantillas.Where(x => socios.Contains((int)x.persona.SocioIm)).ToList();
                }

                if (sedes.Count() != 0)
                {
                    pasantillas = pasantillas.Where(m => m.persona != null).ToList();
                    pasantillas = pasantillas.Where(m => m.persona.Sede != null).ToList();
                    pasantillas = pasantillas.Where(x => sedes.Contains((int)x.persona.Sede)).ToList();
                }

                if (carreras.Count() != 0)
                {
                    pasantillas = pasantillas.Where(m => m.persona != null).ToList();
                    pasantillas = pasantillas.Where(m => m.persona.CarreraCursoGrado != null).ToList();
                    pasantillas = pasantillas.Where(x => carreras.Contains((int)x.persona.CarreraCursoGrado)).ToList();
                }

                if (sexos.Count() != 0)
                {
                    pasantillas = pasantillas.Where(m => m.persona != null).ToList();
                    pasantillas = pasantillas.Where(m => m.persona.Sexo != null).ToList();
                    pasantillas = pasantillas.Where(x => sexos.Contains((int)x.persona.Sexo)).ToList();
                }

                if (departament.Count() != 0)
                {
                    pasantillas = pasantillas.Where(m => m.persona != null).ToList();
                    pasantillas = pasantillas.Where(m => m.persona.Departamento != null).ToList();
                    pasantillas = pasantillas.Where(x => departament.Contains((int)x.persona.Departamento)).ToList();
                }

                if (refiere.Count() != 0)
                {
                    pasantillas = pasantillas.Where(m => m.persona != null).ToList();
                    pasantillas = pasantillas.Where(m => m.persona.Refiere != null).ToList();
                    pasantillas = pasantillas.Where(x => refiere.Contains((int)x.persona.Refiere)).ToList();
                }

                if (idprograma.Count() != 0)
                {
                    pasantillas = pasantillas.Where(m => m.persona != null).ToList();
                    pasantillas = pasantillas.Where(m => m.persona.Programa != null).ToList();
                    pasantillas = pasantillas.Where(x => idprograma.Contains((int)x.persona.Programa)).ToList();
                }

                List<SocioImplementador> SocioImple = new List<SocioImplementador>();
                SocioImple = _ctx.SocioImplementadors.ToList();

                List<Sexo> ssexos = new List<Sexo>();
                ssexos = _ctx.Sexos.ToList();

                List<Departamento> departamentos = new List<Departamento>();
                departamentos = _ctx.Departamentos.ToList();

                List<TipoMatricula> ttipomatricula = new List<TipoMatricula>();
                ttipomatricula = _ctx.TipoMatriculas.ToList();

                List<Refiere> refiereCMs = new List<Refiere>();
                refiereCMs = _ctx.Refieres.Where(x => x.Activo == true).ToList();


                vM.Socios = SocioImple;
                vM.sexos = ssexos;
                vM.tipomatricula = ttipomatricula;
                vM.departamentos = departamentos;
                vM.dropRefiereCM = refiereCMs;

                vM.seguimientopasantillas = pasantillas;
                return View(vM);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Seguimiento Psicosocial
        public async Task<ActionResult> SeguimientoPsicosocial()
        {
            try
            {
                SeguimientoPsicosocialVM vM = new SeguimientoPsicosocialVM();
                var psicosocial = await _ctx.CargaSeguimientoPsicosocials.Select(x =>
                    new SeguimientoPsicosocialTable
                    {
                        IdSeguimientoPsicosocial = x.IdSeguimientoPsicosocial,
                        PId = x.PId,
                        Año = x.Año,
                        Mes = x.Mes,
                        SegMotivo = x.SegMotivo,
                        SegEstado = x.SegEstado,
                        SegMedida = x.SegMedida,
                        persona = _ctx.Personas.Include(x => x.sede).Include(x => x.matricula).Include(x => x.programa).Include(x => x.socio).Include(x => x.carrera).Include(x => x.refiere).FirstOrDefault(n => n.PIdOim == x.PId)

                    }
                    ).ToListAsync();

                List<SocioImplementador> SocioImple = new List<SocioImplementador>();
                SocioImple = _ctx.SocioImplementadors.ToList();

                List<Sexo> ssexos = new List<Sexo>();
                ssexos = _ctx.Sexos.ToList();

                List<Departamento> departamentos = new List<Departamento>();
                departamentos = _ctx.Departamentos.ToList();

                List<TipoMatricula> ttipomatricula = new List<TipoMatricula>();
                ttipomatricula = _ctx.TipoMatriculas.ToList();

                List<Refiere> refiereCMs = new List<Refiere>();
                refiereCMs = _ctx.Refieres.Where(x => x.Activo == true).ToList();


                vM.Socios = SocioImple;
                vM.sexos = ssexos;
                vM.tipomatricula = ttipomatricula;
                vM.departamentos = departamentos;
                vM.dropRefiereCM = refiereCMs;

                vM.seguimientoPsicosocials = psicosocial;
                return View(vM);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<ActionResult> SeguimientoPsicosocial(List<int> idprograma, List<int> socios, List<int> sedes, List<int> carreras, List<int> sexos, List<int> tipomatricula, List<int> departament, List<int> refiere)
        {
            try
            {
                SeguimientoPsicosocialVM vM = new SeguimientoPsicosocialVM();
                var psicosocial = await _ctx.CargaSeguimientoPsicosocials.Select(x =>
                    new SeguimientoPsicosocialTable
                    {
                        IdSeguimientoPsicosocial = x.IdSeguimientoPsicosocial,
                        PId = x.PId,
                        Año = x.Año,
                        Mes = x.Mes,
                        SegMotivo = x.SegMotivo,
                        SegEstado = x.SegEstado,
                        SegMedida = x.SegMedida,
                        persona = _ctx.Personas.Include(x => x.sede).Include(x => x.matricula).Include(x => x.programa).Include(x => x.socio).Include(x => x.carrera).Include(x => x.refiere).FirstOrDefault(n => n.PIdOim == x.PId)

                    }
                    ).ToListAsync();
                if (socios.Count() != 0)
                {
                    psicosocial = psicosocial.Where(m => m.persona != null).ToList();
                    psicosocial = psicosocial.Where(m => m.persona.SocioIm != null).ToList();
                    psicosocial = psicosocial.Where(x => socios.Contains((int)x.persona.SocioIm)).ToList();
                }

                if (sedes.Count() != 0)
                {
                    psicosocial = psicosocial.Where(m => m.persona != null).ToList();
                    psicosocial = psicosocial.Where(m => m.persona.Sede != null).ToList();
                    psicosocial = psicosocial.Where(x => sedes.Contains((int)x.persona.Sede)).ToList();
                }

                if (carreras.Count() != 0)
                {
                    psicosocial = psicosocial.Where(m => m.persona != null).ToList();
                    psicosocial = psicosocial.Where(m => m.persona.CarreraCursoGrado != null).ToList();
                    psicosocial = psicosocial.Where(x => carreras.Contains((int)x.persona.CarreraCursoGrado)).ToList();
                }

                if (sexos.Count() != 0)
                {
                    psicosocial = psicosocial.Where(m => m.persona != null).ToList();
                    psicosocial = psicosocial.Where(m => m.persona.Sexo != null).ToList();
                    psicosocial = psicosocial.Where(x => sexos.Contains((int)x.persona.Sexo)).ToList();
                }

                if (departament.Count() != 0)
                {
                    psicosocial = psicosocial.Where(m => m.persona != null).ToList();
                    psicosocial = psicosocial.Where(m => m.persona.Departamento != null).ToList();
                    psicosocial = psicosocial.Where(x => departament.Contains((int)x.persona.Departamento)).ToList();
                }

                if (refiere.Count() != 0)
                {
                    psicosocial = psicosocial.Where(m => m.persona != null).ToList();
                    psicosocial = psicosocial.Where(m => m.persona.Refiere != null).ToList();
                    psicosocial = psicosocial.Where(x => refiere.Contains((int)x.persona.Refiere)).ToList();
                }

                if (idprograma.Count() != 0)
                {
                    psicosocial = psicosocial.Where(m => m.persona != null).ToList();
                    psicosocial = psicosocial.Where(m => m.persona.Programa != null).ToList();
                    psicosocial = psicosocial.Where(x => idprograma.Contains((int)x.persona.Programa)).ToList();
                }

                List<SocioImplementador> SocioImple = new List<SocioImplementador>();
                SocioImple = _ctx.SocioImplementadors.ToList();

                List<Sexo> ssexos = new List<Sexo>();
                ssexos = _ctx.Sexos.ToList();

                List<Departamento> departamentos = new List<Departamento>();
                departamentos = _ctx.Departamentos.ToList();

                List<TipoMatricula> ttipomatricula = new List<TipoMatricula>();
                ttipomatricula = _ctx.TipoMatriculas.ToList();

                List<Refiere> refiereCMs = new List<Refiere>();
                refiereCMs = _ctx.Refieres.Where(x => x.Activo == true).ToList();


                vM.Socios = SocioImple;
                vM.sexos = ssexos;
                vM.tipomatricula = ttipomatricula;
                vM.departamentos = departamentos;
                vM.dropRefiereCM = refiereCMs;

                vM.seguimientoPsicosocials = psicosocial;
                return View(vM);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Estipendios
        public async Task<IActionResult> Estipendios()
        {
            try
            {
                EstipendiosVM vM = new EstipendiosVM();
                var estipendios = await _ctx.CargaEstipendios.Select(x =>
                    new EstipendiosTable
                    {
                        IdCargaEstipendios = x.IdCargaEstipendios,
                        PId = x.PId,
                        Año = x.Año,
                        Mes = x.Mes,
                        AlimEfectivo = x.AlimEfectivo,
                        AlimMontoEfectivo = x.AlimMontoEfectivo,
                        AlimDiasPresencialesEfectivo = x.AlimDiasPresencialesEfectivo,
                        AlimSubtotalEfectivo = x.AlimSubtotalEfectivo,
                        AlimTransferencia = x.AlimTransferencia,
                        AlimMontoTransferencia = x.AlimMontoTransferencia,
                        AlimDiasPresencialesTransferencia = x.AlimDiasPresencialesTransferencia,
                        AlimSubtotalTransferencia = x.AlimSubtotalTransferencia,
                        AlimEspecie = x.AlimEspecie,
                        AlimMontoEspecie = x.AlimMontoEspecie,
                        AlimDiasPresencialesEspecie = x.AlimDiasPresencialesEspecie,
                        AlimSubtotalEspecie = x.AlimSubtotalEspecie,
                        AlimMontoTotal = x.AlimMontoTotal,

                        TranspEfectivo = x.TranspEfectivo,
                        TranspMontoEfectivo = x.TranspMontoEfectivo,
                        TranspDiasPresencialesEfectivo = x.TranspDiasPresencialesEfectivo,
                        TranspSubtotalEfectivo = x.TranspSubtotalEfectivo,
                        TranspTransferencia = x.TranspTransferencia,
                        TranspTarifaDiferenciada = x.TranspTarifaDiferenciada,
                        TranspMontoTransferencia = x.TranspMontoTransferencia,
                        TranspDiasPresencialesTransferencia = x.TranspDiasPresencialesTransferencia,
                        TranspSubtotalTransferencia = x.TranspSubtotalTransferencia,
                        TranspMontoTotal = x.TranspMontoTotal,

                        ConecEfectivo = x.ConecEfectivo,
                        ConecMontoEfectivo = x.ConecMontoEfectivo,
                        ConecDiasPresencialesEfectivo = x.ConecDiasPresencialesEfectivo,
                        ConecSubtotalEfectivo = x.ConecSubtotalEfectivo,
                        ConecTransferencia = x.ConecTransferencia,
                        ConecMontoTransferencia = x.ConecMontoTransferencia,
                        ConecDiasPresencialesTransferencia = x.ConecDiasPresencialesTransferencia,
                        ConecSubtotalTransferencia = x.ConecSubtotalTransferencia,
                        ConecMontoTotal = x.ConecMontoTotal,
                        EstipendioTotal = x.EstipendioTotal,
                        persona = _ctx.Personas.Include(f => f.programa).Include(f => f.sexo).Include(f => f.socio).Include(f => f.matricula).Include(f => f.carrera).Include(f => f.sede).FirstOrDefault(n => n.PIdOim == x.PId)

                    }
                    ).ToListAsync();
                vM.EstipendiosTable = estipendios;
                return View(vM);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}
