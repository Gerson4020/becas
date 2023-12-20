using BECAS.Models;
using BECAS.Models.VM;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BECASLC;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using BECAS.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            List<PersonTableVM> ListPersonas = _ctx.Personas.Include(x => x.refiere).Include(x => x.sexo).Include(x => x.departamento).Include(x => x.municipio).Select(x =>
                        new PersonTableVM
                        {
                            IdPersona = x.IdPersona,
                            FechaEntrevista = x.FechaEntrevista,
                            Id = x.Id,
                            PIdOim = x.PIdOim,
                            NumeroInscripciones = x.NumeroInscripciones,
                            Nombre = x.Nombre,
                            Apellido = x.Apellido,
                            NombreCompleto = x.NombreCompleto,
                            UltimoGradoAprobado = x.UltimoGradoAprobado,
                            NivelAcademico = x.NivelAcademico,
                            Telefono1 = x.Telefono1,
                            Telefono2 = x.Telefono2,
                            Sexo = x.Sexo,
                            LGBTIQ = x.LGBTIQ,
                            FechaNacimiento = x.FechaNacimiento,
                            Edad = x.Edad,
                            Discapacidad = x.Discapacidad,
                            VictimaViolencia = x.VictimaViolencia,
                            MigranteRetornado = x.MigranteRetornado,
                            PiensaMigrar = x.PiensaMigrar,
                            FamiliaresMigrantes = x.FamiliaresMigrantes,
                            FamiliaresRetornados = x.FamiliaresRetornados,
                            Empleo = x.Empleo,
                            Dui = x.Dui,
                            Nie = x.Nie,
                            Correo = x.Correo,
                            Refiere = x.Refiere,
                            RefiereNombre = x.refiere.Nombre,
                            Departamento = x.Departamento,
                            DepartamentoNombre = x.departamento.Nombre,
                            Municipio = x.Municipio,
                            MunicipioNombre = x.municipio.Nombre,
                            EstadoInscripcion = x.EstadoInscripcion,
                            MedioVerificacion = x.MedioVerificacion,
                            IdCarga = x.IdCarga,
                            educacion = _ctx.CargaEducacions.Include(x => x.cohorte).Include(b => b.socio).Include(x => x.sede).Include(x => x.programa).Include(x => x.carrera).Include(x => x.tipoMatricula).Include(x => x.estado).OrderByDescending(o => o.r_fechafin).FirstOrDefault(e => e.PIdOim.Equals(x.PIdOim)),
                        }
                        ).ToList();
            foreach (var item in ListPersonas)
            {
                if (item.IdPrograma == 3)
                {
                    item.grado = _ctx.Grados.FirstOrDefault(g => g.IdGrado.Equals(item.educacion.CarreraCursoGrado)).Nombre;
                }
            }
            List<SocioImplementador> SocioImple = new List<SocioImplementador>();
            SocioImple = _ctx.SocioImplementadors.Where(x => x.Activo.Equals(true)).ToList();

            List<Sexo> sexos = new List<Sexo>();
            sexos = _ctx.Sexos.ToList();

            List<Departamento> departamentos = new List<Departamento>();
            departamentos = _ctx.Departamentos.Where(x => x.Activo == true).ToList();

            List<TipoMatricula> ttipomatricula = new List<TipoMatricula>();
            ttipomatricula = _ctx.TipoMatriculas.Where(x => x.Activo.Equals(true)).ToList();

            List<Refiere> refiereCMs = new List<Refiere>();
            refiereCMs = _ctx.Refieres.Where(x => x.Activo == true).ToList();

            List<Cohorte> cohortes = new List<Cohorte>();
            cohortes = _ctx.Cohortes.Where(x => x.Activo.Equals(true)).ToList();

            List<Sector> sectors = new List<Sector>();
            sectors = _ctx.Sectors.Where(x => x.Activo == true).ToList();

            List<EstadoPersona> estadoPersonas = new List<EstadoPersona>();
            estadoPersonas = _ctx.EstadoPersonas.Where(x => x.Activo == true).ToList();

            List<CatAño> years = _ctx.CatAños.Where(x => x.Activo == true).ToList();

            vM.Personas = ListPersonas;
            vM.Socios = SocioImple;
            vM.sexos = sexos;
            vM.tipomatricula = ttipomatricula;
            vM.departamentos = departamentos;
            vM.dropRefiereCM = refiereCMs;
            vM.cohorte = cohortes;
            vM.Sectors = sectors;
            vM.estadoPersona = estadoPersonas;
            vM.catAños = years;
            return View(vM);
        }
        [HttpPost]
        public ActionResult Personas(List<int> idprograma, List<int> socios, List<int> sedes, List<int> carreras, List<int> sexos, List<int> tipomatricula, List<int> departament, List<int> refiere, List<int> dropzona, List<int> cohorte, List<int> retornado, List<int> sectores, List<int> estado, List<int> aEstudio)
        {
            List<PersonTableVM> ListPersonas = (from x in _ctx.Personas.Include(x => x.sexo).Include(x => x.refiere)
                                                join educacion in _ctx.CargaEducacions.Include(x => x.socio).Include(x => x.sede).Include(x => x.carrera).Include(x => x.programa).Include(x => x.cohorte).Include(x => x.estado) on x.PIdOim equals educacion.PIdOim
                                                orderby educacion.r_fechaini descending
                                                select new PersonTableVM
                                                {
                                                    IdPersona = x.IdPersona,
                                                    FechaEntrevista = x.FechaEntrevista,
                                                    Id = x.Id,
                                                    PIdOim = x.PIdOim,
                                                    NumeroInscripciones = x.NumeroInscripciones,
                                                    NombreCompleto = x.NombreCompleto,
                                                    programa = educacion.programa.Nombre,
                                                    carrera = educacion.carrera.Nombre,
                                                    NivelAcademico = x.NivelAcademico,
                                                    Telefono1 = x.Telefono1,
                                                    Telefono2 = x.Telefono2,
                                                    Sexo = x.sexo.IdSexo,
                                                    SexoNombre = x.sexo.Nombre,
                                                    LGBTIQ = x.LGBTIQ,
                                                    FechaNacimiento = x.FechaNacimiento,
                                                    Edad = x.Edad,
                                                    Discapacidad = x.Discapacidad,
                                                    VictimaViolencia = x.VictimaViolencia,
                                                    MigranteRetornado = x.MigranteRetornado,
                                                    PiensaMigrar = x.PiensaMigrar,
                                                    FamiliaresMigrantes = x.FamiliaresMigrantes,
                                                    FamiliaresRetornados = x.FamiliaresRetornados,
                                                    Empleo = x.Empleo,
                                                    Dui = x.Dui,
                                                    Nie = x.Nie,
                                                    Correo = x.Correo,
                                                    Refiere = x.Refiere,
                                                    Departamento = x.Departamento,
                                                    DepartamentoNombre = x.departamento.Nombre,
                                                    Municipio = x.Municipio,
                                                    MunicipioNombre = x.municipio.Nombre,
                                                    tipomatricula = educacion.tipoMatricula.Nombre,
                                                    añoestudio = educacion.Year,
                                                    Cohorte = educacion.cohorte.IdCohorte,
                                                    CohorteNombre = educacion.cohorte.Nombre,
                                                    p_socio = educacion.p_socio,
                                                    p_socioNombre = educacion.socio.Nombre,
                                                    p_sede = educacion.p_sede,
                                                    p_sedeNombre = educacion.sede.Nombre,
                                                    UltimoGradoAprobado = x.UltimoGradoAprobado,
                                                    EstadoInscripcion = x.EstadoInscripcion,
                                                    MedioVerificacion = x.MedioVerificacion,
                                                    IdCarga = x.IdCarga,
                                                    IdPrograma = x.IdPrograma,
                                                    Year = x.Year,
                                                    IdZona = x.IdZona,
                                                    EstadoMF = x.EstadoMF,
                                                    CarreraCursoGrado = educacion.CarreraCursoGrado,
                                                    Sector = x.Sector,
                                                    Estado = educacion.estado.Nombre
                                                }).ToList();
            foreach (var item in ListPersonas)
            {
                if ((item.IdPrograma == 3 || item.IdPrograma == 2) && item.CarreraCursoGrado != null)
                {
                    item.carrera = _ctx.Grados.FirstOrDefault(g => g.IdGrado.Equals(item.CarreraCursoGrado)).Nombre;
                }
            }
            ListPersonas = ListPersonas.DistinctBy(x => x.PIdOim).ToList();
            if (retornado.Count() != 0)
            {
                ListPersonas = ListPersonas.Where(m => m.MigranteRetornado != null).ToList();
                ListPersonas = ListPersonas.Where(m => retornado.Contains((int)m.MigranteRetornado)).ToList();
            }
            if (idprograma.Count() != 0)
            {
                ListPersonas = ListPersonas.Where(m => m.IdPrograma != null).ToList();
                ListPersonas = ListPersonas.Where(x => idprograma.Contains((int)x.IdPrograma)).ToList();
            }
            if (departament.Count() != 0)
            {
                ListPersonas = ListPersonas.Where(m => m.Departamento != null).ToList();
                ListPersonas = ListPersonas.Where(m => departament.Contains((int)m.Departamento)).ToList();
            }
            if (socios.Count() != 0)
            {
                ListPersonas = ListPersonas.Where(m => m.p_socio != null).ToList();
                ListPersonas = ListPersonas.Where(x => socios.Contains((int)x.p_socio)).ToList();
            }
            if (sedes.Count() != 0)
            {
                for (int i = 0; i < sedes.Count; i++)
                {
                    var IdCatSede = (int)_ctx.Sedes.FirstOrDefault(x => x.IdSede.Equals(sedes.First())).IdCatSede;
                    sedes[i] = IdCatSede;
                }

                if (sedes != null)
                {
                    ListPersonas = ListPersonas.Where(m => m.p_sede != null).ToList();
                    ListPersonas = ListPersonas.Where(x => sedes.Contains((int)x.p_sede)).ToList();
                }

            }
            if (carreras.Count() != 0)
            {
                ListPersonas = ListPersonas.Where(m => m.CarreraCursoGrado != null).ToList();
                ListPersonas = ListPersonas.Where(x => carreras.Contains((int)x.CarreraCursoGrado)).ToList();
            }
            if (sexos.Count() != 0)
            {
                ListPersonas = ListPersonas.Where(m => m.Sexo != null).ToList();
                ListPersonas = ListPersonas.Where(m => sexos.Contains((int)m.Sexo)).ToList();
            }
            if (tipomatricula.Count() != 0)
            {
                ListPersonas = ListPersonas.Where(m => m.educacion != null).ToList();
                ListPersonas = ListPersonas.Where(m => m.educacion.tipoMatricula != null).ToList();
                ListPersonas = ListPersonas.Where(x => tipomatricula.Contains((int)x.educacion.tipoMatricula.IdTipoMatricula)).ToList();
            }

            if (estado.Count() != 0)
            {
                ListPersonas = ListPersonas.Where(m => m.educacion != null).ToList();
                ListPersonas = ListPersonas.Where(m => m.educacion.DEstado != null).ToList();
                ListPersonas = ListPersonas.Where(x => estado.Contains((int)x.educacion.DEstado)).ToList();
            }

            if (refiere.Count() != 0)
            {
                ListPersonas = ListPersonas.Where(m => m.Refiere != null).ToList();
                ListPersonas = ListPersonas.Where(m => refiere.Contains((int)m.Refiere)).ToList();
            }

            if (dropzona.Count() != 0)
            {
                ListPersonas = ListPersonas.Where(m => m.IdZona != null).ToList();
                ListPersonas = ListPersonas.Where(m => dropzona.Contains((int)m.IdZona)).ToList();
            }

            if (cohorte.Count() != 0)
            {
                ListPersonas = ListPersonas.Where(m => m.Cohorte != null).ToList();
                ListPersonas = ListPersonas.Where(x => cohorte.Contains((int)x.Cohorte)).ToList();
            }

            if (sectores.Count() != 0)
            {
                ListPersonas = ListPersonas.Where(m => m.Sector != null).ToList();
                ListPersonas = ListPersonas.Where(x => sectores.Contains((int)x.Sector)).ToList();
            }

            if (sectores.Count() != 0)
            {
                ListPersonas = ListPersonas.Where(m => m.Sector != null).ToList();
                ListPersonas = ListPersonas.Where(x => sectores.Contains((int)x.Sector)).ToList();
            }

            if (aEstudio.Count() != 0)
            {
                ListPersonas = ListPersonas.Where(m => m.educacion != null).ToList();
                ListPersonas = ListPersonas.Where(m => m.educacion.Year != null).ToList();
                ListPersonas = ListPersonas.Where(x => aEstudio.Contains((int)x.Year)).ToList();
            }



            return Json(new { data = ListPersonas });
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
                vM.mes = mont;
                vM.catAños = year;
                vM.cohorte = cohortes;
                return View(vM);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public JsonResult GetEvPsicosocial(List<int> socio, List<int> zonas, List<int> programa, List<int> sedes, List<int> carreras, List<int> sexos, List<int> tipomatricula, List<int> departament, List<int> refiere, List<int> year1, List<string> mes, List<int> aEstudio, List<int> cohorte1)
        {
            try
            {
                EvalucionPsicosocialVM vM = new EvalucionPsicosocialVM();
                var evpsi = (from x in _ctx.CargaEvaluacionPsicosocials
                             join educacion in _ctx.CargaEducacions.Include(x => x.socio).Include(x => x.sede).Include(x => x.carrera).Include(x => x.programa).Include(x => x.cohorte).Include(x => x.estado) on x.PId equals educacion.PIdOim
                             join persona in _ctx.Personas.Include(x => x.sexo).Include(x => x.refiere) on educacion.PIdOim equals persona.PIdOim
                             orderby educacion.r_fechaini descending
                             select new EvaluscionPsicosicialTable
                             {
                                 Id = x.PId,
                                 Nombre = persona.NombreCompleto,
                                 OvParticipacion = x.OvParticipacion,
                                 OvPuntajePret = x.OvPuntajePret,
                                 OvPuntajePos = x.OvPuntajePos,
                                 EpInstrumentoRiesgo = x.EpInstrumentoRiesgo,
                                 EpVulnerabilidades = x.EpVulnerabilidades,
                                 EpAlertaDesercion = x.EpAlertaDesercion,
                                 Mes = x.Mes,
                                 Año = x.Año,
                                 fechainicio = x.r_fechafin,
                                 idsocio = educacion.p_socio,
                                 idzona = educacion.IdZona,
                                 idprograma = educacion.programa.IdPrograma,
                                 idsede = educacion.p_sede,
                                 idsexo = persona.Sexo,
                                 idTipomatricula = educacion.p_tipobeca,
                                 idDepartament = persona.Departamento,
                                 refiere = persona.Refiere,
                                 year1 = educacion.Year,
                                 nombreSocio = educacion.socio.Nombre,
                                 nombreSede = educacion.sede.Nombre,
                                 tipoMatricula = educacion.tipoMatricula.Nombre,
                                 nombreCarrera = educacion.carrera.Nombre,
                                 idCarreta = educacion.CarreraCursoGrado,
                                 cohorte = educacion.Cohorte,
                                 nombreCohorte = educacion.cohorte.Nombre
                             }).ToList();
                DateTime date = DateTime.Now;
                DateTime oPrimerDiaDelMes = new DateTime(date.Year, date.Month, 1);
                DateTime MesAnt = oPrimerDiaDelMes.AddMonths(-3);
                var f = evpsi.Where(x => x.fechainicio > MesAnt).ToList();
                var ED = f.OrderByDescending(x => x.fechainicio).DistinctBy(x => x.Id).ToList();

                if (socio.Count() != 0)
                {
                    ED = ED.Where(m => m.idsocio != null).ToList();
                    ED = ED.Where(x => socio.Contains((int)x.idsocio)).ToList();
                }
                if (zonas.Count() != 0)
                {
                    ED = ED.Where(m => m.idzona != null).ToList();
                    ED = ED.Where(x => zonas.Contains((int)x.idzona)).ToList();
                }
                if (programa.Count() != 0)
                {
                    ED = ED.Where(m => m.idprograma != null).ToList();
                    ED = ED.Where(x => programa.Contains((int)x.idprograma)).ToList();
                }
                if (sedes.Count() != 0)
                {
                    ED = ED.Where(m => m.idsede != null).ToList();
                    ED = ED.Where(x => sedes.Contains((int)x.idsede)).ToList();
                }
                if (carreras.Count() != 0)
                {
                    ED = ED.Where(m => m.idCarreta != null).ToList();
                    ED = ED.Where(x => carreras.Contains((int)x.idCarreta)).ToList();
                }
                if (sexos.Count() != 0)
                {
                    ED = ED.Where(m => m.idsexo != null).ToList();
                    ED = ED.Where(x => sexos.Contains((int)x.idsexo)).ToList();
                }
                if (tipomatricula.Count() != 0)
                {
                    ED = ED.Where(m => m.idTipomatricula != null).ToList();
                    ED = ED.Where(x => tipomatricula.Contains((int)x.idTipomatricula)).ToList();
                }
                if (departament.Count() != 0)
                {
                    ED = ED.Where(m => m.idDepartament != null).ToList();
                    ED = ED.Where(x => departament.Contains((int)x.idDepartament)).ToList();
                }
                if (refiere.Count() != 0)
                {
                    ED = ED.Where(m => m.refiere != null).ToList();
                    ED = ED.Where(x => refiere.Contains((int)x.refiere)).ToList();
                }
                if (year1.Count() != 0)
                {
                    ED = ED.Where(m => m.Año != null).ToList();
                    ED = ED.Where(x => year1.Contains((int)x.Año)).ToList();
                }
                if (mes.Count() != 0)
                {
                    ED = ED.Where(m => m.Mes != null).ToList();
                    ED = ED.Where(x => mes.Contains(x.Mes)).ToList();
                }
                if (aEstudio.Count() != 0)
                {
                    ED = ED.Where(m => m.year1 != null).ToList();
                    ED = ED.Where(x => aEstudio.Contains((int)x.year1)).ToList();
                }
                if (cohorte1.Count() != 0)
                {
                    ED = ED.Where(m => m.cohorte != null).ToList();
                    ED = ED.Where(x => cohorte1.Contains((int)x.cohorte)).ToList();
                }

                return Json(new { data = ED });
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
                        persona = _ctx.Personas.Include(x => x.refiere).FirstOrDefault(n => n.PIdOim == x.PId)
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

                List<Cohorte> cohortes = new List<Cohorte>();
                cohortes = _ctx.Cohortes.Where(x => x.Activo == true).ToList();

                vM.AutoempleoTables = auto;

                vM.Socios = SocioImple;
                vM.sexos = ssexos;
                vM.tipomatricula = ttipomatricula;
                vM.departamentos = departamentos;
                vM.dropRefiereCM = refiereCMs;
                vM.cohorte = cohortes;
                vM.mes = mont;
                vM.catA = year;
                return View(vM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> SeguimientoAutoempleo(List<int> idprograma, List<int> socios, List<int> sedes, List<int> carreras, List<int> sexos, List<int> tipomatricula, List<int> departament, List<int> refiere, List<int> year1, List<string> mes, List<int> cohorte1)
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
                        persona = _ctx.Personas.Include(f => f.sexo).FirstOrDefault(n => n.PIdOim == x.PId)
                    }
                    ).ToListAsync();
                if (socios.Count() != 0)
                {
                    auto = auto.Where(m => m.persona != null).ToList();
                    //auto = auto.Where(m => m.persona.SocioIm != null).ToList();
                    //auto = auto.Where(x => socios.Contains((int)x.persona.SocioIm)).ToList();
                }

                if (sedes.Count() != 0)
                {
                    auto = auto.Where(m => m.persona != null).ToList();
                    //auto = auto.Where(m => m.persona.Sede != null).ToList();
                    //auto = auto.Where(x => sedes.Contains((int)x.persona.Sede)).ToList();
                }

                if (carreras.Count() != 0)
                {
                    auto = auto.Where(m => m.persona != null).ToList();
                    //auto = auto.Where(m => m.persona.CarreraCursoGrado != null).ToList();
                    //auto = auto.Where(x => carreras.Contains((int)x.persona.CarreraCursoGrado)).ToList();
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
                    //auto = auto.Where(m => m.persona.Programa != null).ToList();
                    //auto = auto.Where(x => idprograma.Contains((int)x.persona.Programa)).ToList();
                }

                if (year1.Count() != 0)
                {
                    auto = auto.Where(x => year1.Contains((int)x.Año)).ToList();
                }

                if (mes.Count() != 0)
                {
                    auto = auto.Where(x => mes.Contains(x.Mes)).ToList();
                }

                if (cohorte1.Count() != 0)
                {
                    auto = auto.Where(m => m.persona != null).ToList();
                    //auto = auto.Where(m => m.persona.Cohorte != null).ToList();
                    //auto = auto.Where(x => cohorte1.Contains((int)x.persona.Cohorte)).ToList();
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

                vM.AutoempleoTables = auto;

                vM.Socios = SocioImple;
                vM.sexos = ssexos;
                vM.tipomatricula = ttipomatricula;
                vM.departamentos = departamentos;
                vM.dropRefiereCM = refiereCMs;
                vM.cohorte = cohortes;
                vM.mes = mont;
                vM.catA = year;

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
                        persona = _ctx.Personas.Include(f => f.sexo).FirstOrDefault(n => n.PIdOim == x.PId)
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

                List<Cohorte> cohortes = new List<Cohorte>();
                cohortes = _ctx.Cohortes.Where(x => x.Activo == true).ToList();

                vM.ListaPracticas = practicas;

                vM.Socios = SocioImple;
                vM.sexos = ssexos;
                vM.tipomatricula = ttipomatricula;
                vM.departamentos = departamentos;
                vM.dropRefiereCM = refiereCMs;
                vM.cohorte = cohortes;
                vM.catA = year;
                vM.mes = mont;
                return View(vM);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> SeguimientoPracticasPr(List<int> idprograma, List<int> socios, List<int> sedes, List<int> carreras, List<int> sexos, List<int> tipomatricula, List<int> departament, List<int> refiere, List<int> year1, List<string> mes, List<int> cohorte1)
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
                        persona = _ctx.Personas.Include(x => x.refiere).FirstOrDefault(n => n.PIdOim == x.PId)
                    }
                    ).ToListAsync();

                if (socios.Count() != 0)
                {
                    practicas = practicas.Where(m => m.persona != null).ToList();
                    //practicas = practicas.Where(m => m.persona.SocioIm != null).ToList();
                    //practicas = practicas.Where(x => socios.Contains((int)x.persona.SocioIm)).ToList();
                }

                if (sedes.Count() != 0)
                {
                    practicas = practicas.Where(m => m.persona != null).ToList();
                    //practicas = practicas.Where(m => m.persona.Sede != null).ToList();
                    //practicas = practicas.Where(x => sedes.Contains((int)x.persona.Sede)).ToList();
                }

                if (carreras.Count() != 0)
                {
                    practicas = practicas.Where(m => m.persona != null).ToList();
                    //practicas = practicas.Where(m => m.persona.CarreraCursoGrado != null).ToList();
                    //practicas = practicas.Where(x => carreras.Contains((int)x.persona.CarreraCursoGrado)).ToList();
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
                    //practicas = practicas.Where(m => m.persona.Programa != null).ToList();
                    //practicas = practicas.Where(x => idprograma.Contains((int)x.persona.Programa)).ToList();
                }

                if (year1.Count() != 0)
                {
                    practicas = practicas.Where(x => year1.Contains((int)x.Año)).ToList();
                }

                if (mes.Count() != 0)
                {
                    practicas = practicas.Where(x => mes.Contains(x.Mes)).ToList();
                }

                if (cohorte1.Count() != 0)
                {
                    practicas = practicas.Where(m => m.persona != null).ToList();
                    //practicas = practicas.Where(m => m.persona.Cohorte != null).ToList();
                    //practicas = practicas.Where(x => cohorte1.Contains((int)x.persona.Cohorte)).ToList();
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

                vM.ListaPracticas = practicas;

                vM.Socios = SocioImple;
                vM.sexos = ssexos;
                vM.tipomatricula = ttipomatricula;
                vM.departamentos = departamentos;
                vM.dropRefiereCM = refiereCMs;
                vM.cohorte = cohortes;
                vM.catA = year;
                vM.mes = mont;
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
                var query = (from educacion in _ctx.CargaEducacions.Include(x => x.socio).Include(x => x.sede).Include(x => x.carrera).Include(x => x.programa).Include(x => x.cohorte).Include(x => x.estado)
                             join persona in _ctx.Personas.Include(x => x.sexo).Include(x => x.refiere) on educacion.PIdOim equals persona.PIdOim
                             orderby educacion.r_fechaini descending
                             select new EducacionTable
                             {
                                 PIdOim = educacion.PIdOim,
                                 nombre = persona.NombreCompleto,
                                 DFechades = educacion.DFechades,
                                 DFechaReasg = educacion.DFechaReasg,
                                 DEstado = educacion.estado.Nombre,
                                 IDiasAsistenciaEfectivos = educacion.IDiasAsistenciaEfectivos,
                                 IDiasAsistenciaEstablecidos = educacion.IDiasAsistenciaEstablecidos,
                                 IMotivoInasistencia = educacion.IMotivoInasistencia,
                                 IModulosInscritos = educacion.IModulosInscritos,
                                 IModulosAprobados = educacion.IModulosAprobados,
                                 IModulosReprobados = educacion.IModulosReprobados,
                                 ICausaReprobacion = educacion.ICausaReprobacion,
                                 p_socio = educacion.p_socio,
                                 socio = educacion.socio.Nombre,
                                 p_sede = educacion.p_sede,
                                 sede = educacion.sede.Nombre,
                                 CarreraCursoGrado = educacion.CarreraCursoGrado,
                                 carrera = educacion.carrera.Nombre,
                                 sexoID = persona.sexo.IdSexo,
                                 sexoNombre = persona.sexo.Nombre,
                                 departamento = persona.Departamento,
                                 refiere = persona.Refiere,
                                 refiereNombre = persona.refiere.Nombre,
                                 programa = persona.IdPrograma,
                                 programaNombre = educacion.programa.Nombre,
                                 RAño = educacion.RAño,
                                 RMes = educacion.RMes,
                                 Cohorte = educacion.Cohorte,
                                 CohorteNombre = educacion.cohorte.Nombre,
                                 year = educacion.Year,
                                 tipoMatricula = educacion.tipoMatricula.Nombre,
                                 motivodesercion = educacion.DMotivodesercion,
                                 pocentajeasistencia = educacion.i_proc_asistencia,
                                 fechainicio = educacion.r_fechaini,
                                 zona = educacion.IdZona,
                                 EstadoPersona = educacion.DEstado
                             }).ToList();
                DateTime date = DateTime.Now;
                DateTime oPrimerDiaDelMes = new DateTime(date.Year, date.Month, 1);
                DateTime MesAnt = oPrimerDiaDelMes.AddMonths(-24);
                var f = query.Where(x => x.fechainicio > MesAnt).ToList();
                var ED = f.DistinctBy(x => x.PIdOim).ToList();

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

                List<Cohorte> cohortes = _ctx.Cohortes.Where(x => x.Activo == true).ToList();

                ViewData["EstadoPersona"] = new SelectList(_ctx.EstadoPersonas, "IdEstadoPersona", "Nombre");


                vM.Socios = SocioImple;
                vM.sexos = ssexos;
                vM.tipomatricula = ttipomatricula;
                vM.departamentos = departamentos;
                vM.dropRefiereCM = refiereCMs;
                vM.catAños = year;
                vM.mes = mont;
                vM.cohorte = cohortes;
                //vM.EstadoPersonas = EstadoPersona;

                vM.EducacionTable = ED;
                return View(vM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult> Educacion(List<int> idprograma, List<int> socios, List<int> sedes, List<int> carreras, List<int> sexos, List<int> tipomatricula, List<int> departament, List<int> refiere, List<int> dropzona, List<int> year1, List<string> mes, List<int> cohorte1, List<int> aEstudio, List<int> ePersona)
        {
            try
            {
                EducacionVM vM = new EducacionVM();
                // Consulta base de datos para cargar los datos relevantes
                var consulta = (from educacion in _ctx.CargaEducacions.Include(x => x.socio).Include(x => x.sede).Include(x => x.carrera).Include(x => x.programa).Include(x => x.cohorte).Include(x => x.estado)
                                join persona in _ctx.Personas.Include(x => x.sexo).Include(x => x.refiere) on educacion.PIdOim equals persona.PIdOim
                                orderby educacion.r_fechaini descending
                                select new EducacionTable
                                {
                                    PIdOim = educacion.PIdOim,
                                    nombre = persona.NombreCompleto,
                                    DFechades = educacion.DFechades,
                                    DFechaReasg = educacion.DFechaReasg,
                                    DEstado = educacion.estado.Nombre,
                                    IDiasAsistenciaEfectivos = educacion.IDiasAsistenciaEfectivos,
                                    IDiasAsistenciaEstablecidos = educacion.IDiasAsistenciaEstablecidos,
                                    IMotivoInasistencia = educacion.IMotivoInasistencia,
                                    IModulosInscritos = educacion.IModulosInscritos,
                                    IModulosAprobados = educacion.IModulosAprobados,
                                    IModulosReprobados = educacion.IModulosReprobados,
                                    ICausaReprobacion = educacion.ICausaReprobacion,
                                    p_socio = educacion.p_socio,
                                    socio = educacion.socio.Nombre,
                                    p_sede = educacion.p_sede,
                                    sede = educacion.sede.Nombre,
                                    CarreraCursoGrado = (educacion.p_tipobeca == 3 || educacion.p_tipobeca == 4) ? _ctx.Grados.FirstOrDefault(x => x.IdGrado.Equals(educacion.p_tipobeca)).IdGrado : educacion.CarreraCursoGrado,
                                    carrera = (educacion.p_tipobeca == 3 || educacion.p_tipobeca == 4) ? _ctx.Grados.FirstOrDefault(x => x.IdGrado.Equals(educacion.p_tipobeca)).Nombre : educacion.carrera.Nombre,
                                    sexoID = persona.sexo.IdSexo,
                                    sexoNombre = persona.sexo.Nombre,
                                    departamento = persona.Departamento,
                                    refiere = persona.Refiere,
                                    refiereNombre = persona.refiere.Nombre,
                                    programa = educacion.p_tipobeca,
                                    programaNombre = educacion.programa.Nombre,
                                    RAño = educacion.RAño,
                                    RMes = educacion.RMes,
                                    Cohorte = educacion.Cohorte,
                                    CohorteNombre = educacion.cohorte.Nombre,
                                    year = educacion.Year,
                                    tipoMatricula = educacion.tipoMatricula.Nombre,
                                    motivodesercion = educacion.DMotivodesercion,
                                    pocentajeasistencia = educacion.i_proc_asistencia,
                                    fechainicio = educacion.r_fechaini,
                                    zona = educacion.IdZona,
                                    EstadoPersona = educacion.DEstado
                                }).ToList();

                consulta = consulta.GroupBy(x => x.PIdOim)
                                   .Select(x => x.First())
                                   .ToList();

                // Filtrado
                if (socios.Count > 0)
                {
                    consulta = consulta.Where(m => m.p_socio != null).ToList();
                    consulta = consulta.Where(x => socios.Contains((int)x.p_socio)).ToList();
                }
                if (sedes.Count > 0)
                {
                    consulta = consulta.Where(m => m.p_sede != null).ToList();
                    consulta = consulta.Where(x => sedes.Contains((int)x.p_sede)).ToList();
                }

                if (idprograma.Count > 0)
                {
                    consulta = consulta.Where(m => m.programa != null).ToList();
                    consulta = consulta.Where(x => idprograma.Contains((int)x.programa)).ToList();
                }

                if (carreras.Count > 0)
                {
                    consulta = consulta.Where(m => m.CarreraCursoGrado != null).ToList();
                    consulta = consulta.Where(x => carreras.Contains((int)x.CarreraCursoGrado)).ToList();
                }

                if (sexos.Count > 0)
                {
                    consulta = consulta.Where(m => m.sexoID != null).ToList();
                    consulta = consulta.Where(x => sexos.Contains((int)x.sexoID)).ToList();
                }

                if (tipomatricula.Count > 0)
                {
                    consulta = consulta.Where(m => m.p_matricula != null).ToList();
                    consulta = consulta.Where(x => tipomatricula.Contains((int)x.p_matricula)).ToList();
                }

                if (departament.Count > 0)
                {
                    consulta = consulta.Where(m => m.departamento != null).ToList();
                    consulta = consulta.Where(x => departament.Contains((int)x.departamento)).ToList();
                }

                if (refiere.Count > 0)
                {
                    consulta = consulta.Where(m => m.refiere != null).ToList();
                    consulta = consulta.Where(x => refiere.Contains((int)x.refiere)).ToList();
                }

                if (dropzona.Count > 0)
                {
                    consulta = consulta.Where(m => m.zona != null).ToList();
                    consulta = consulta.Where(x => dropzona.Contains((int)x.zona)).ToList();
                }

                if (year1.Count > 0)
                {
                    consulta = consulta.Where(m => m.RAño != null).ToList();
                    consulta = consulta.Where(x => year1.Contains((int)x.RAño)).ToList();
                }

                if (mes.Count > 0)
                {
                    consulta = consulta.Where(m => m.RMes != null).ToList();
                    consulta = consulta.Where(x => mes.Contains(x.RMes)).ToList();
                }

                if (cohorte1.Count > 0)
                {
                    consulta = consulta.Where(m => m.Cohorte != null).ToList();
                    consulta = consulta.Where(x => cohorte1.Contains((int)x.Cohorte)).ToList();
                }

                if (aEstudio.Count > 0)
                {
                    consulta = consulta.Where(m => m.year != null).ToList();
                    consulta = consulta.Where(x => aEstudio.Contains((int)x.year)).ToList();
                }

                if (ePersona.Count > 0)
                {
                    consulta = consulta.Where(m => m.EstadoPersona != null).ToList();
                    consulta = consulta.Where(x => ePersona.Contains((int)x.EstadoPersona)).ToList();
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

                ViewData["EstadoPersona"] = new SelectList(_ctx.EstadoPersonas, "IdEstadoPersona", "Nombre");

                vM.Socios = SocioImple;
                vM.sexos = ssexos;
                vM.tipomatricula = ttipomatricula;
                vM.departamentos = departamentos;
                vM.dropRefiereCM = refiereCMs;
                vM.catAños = year;
                vM.mes = mont;
                vM.cohorte = cohortes;

                vM.EducacionTable = consulta;
                return View(vM);
            }
            catch (Exception ex)
            {
                var sms = ex.Message;
                throw;
            }
        }

        [HttpGet]
        public ActionResult GetEducacion(List<int> idprograma, List<int> socios, List<int> sedes, List<int> carreras, List<int> sexos, List<int> tipomatricula, List<int> departament, List<int> refiere, List<int> dropzona, List<int> year1, List<string> mes, List<int> cohorte1, List<int> aEstudio, List<int> ePersona)
        {
            try
            {
                var consulta = (
                    from ce in _ctx.CargaEducacions
                    orderby ce.r_fechaini descending
                    group ce by ce.PIdOim into grupos
                    select new
                    {
                        p_id_oim = grupos.Key,
                        d_estado = grupos.FirstOrDefault().DEstado,
                        r_fechaini = grupos.FirstOrDefault().r_fechaini,
                        r_año = grupos.FirstOrDefault().RAño
                    }
                ).ToList();

                consulta = consulta.GroupBy(x => x.p_id_oim)
                                   .Select(x => x.First())
                                   .ToList();

                return Ok(new { data = consulta });

            }
            catch (Exception ex)
            {
                var sms = ex.Message;
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
                        persona = _ctx.Personas.Include(x => x.refiere).FirstOrDefault(n => n.PIdOim == x.PId)

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
                        persona = _ctx.Personas.Include(f => f.sexo).FirstOrDefault(n => n.PIdOim == x.PId)

                    }
                    ).ToListAsync();

                if (socios.Count() != 0)
                {
                    pasantillas = pasantillas.Where(m => m.persona != null).ToList();
                    //pasantillas = pasantillas.Where(m => m.persona.SocioIm != null).ToList();
                    //pasantillas = pasantillas.Where(x => socios.Contains((int)x.persona.SocioIm)).ToList();
                }

                if (sedes.Count() != 0)
                {
                    pasantillas = pasantillas.Where(m => m.persona != null).ToList();
                    //pasantillas = pasantillas.Where(m => m.persona.Sede != null).ToList();
                    //pasantillas = pasantillas.Where(x => sedes.Contains((int)x.persona.Sede)).ToList();
                }

                if (carreras.Count() != 0)
                {
                    pasantillas = pasantillas.Where(m => m.persona != null).ToList();
                    //pasantillas = pasantillas.Where(m => m.persona.CarreraCursoGrado != null).ToList();
                    //pasantillas = pasantillas.Where(x => carreras.Contains((int)x.persona.CarreraCursoGrado)).ToList();
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
                    //pasantillas = pasantillas.Where(m => m.persona.Programa != null).ToList();
                    //pasantillas = pasantillas.Where(x => idprograma.Contains((int)x.persona.Programa)).ToList();
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
                vM.mes = mont;
                vM.catAños = year;
                vM.cohorte = cohortes;
                return View(vM);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public JsonResult GetSegPsicosocial(List<int> socio, List<int> zonas, List<int> programa, List<int> sedes, List<int> carreras, List<int> sexos, List<int> tipomatricula, List<int> departament, List<int> refiere, List<int> year1, List<string> mes, List<int> aEstudio, List<int> cohorte1)
        {
            try
            {
                EvalucionPsicosocialVM vM = new EvalucionPsicosocialVM();
                var evpsi = (from x in _ctx.CargaSeguimientoPsicosocials
                             join educacion in _ctx.CargaEducacions.Include(x => x.socio).Include(x => x.sede).Include(x => x.carrera).Include(x => x.programa).Include(x => x.cohorte).Include(x => x.estado) on x.PId equals educacion.PIdOim
                             join persona in _ctx.Personas.Include(x => x.sexo).Include(x => x.refiere) on educacion.PIdOim equals persona.PIdOim
                             orderby educacion.r_fechaini descending
                             select new SeguimientoPsicosocialTable
                             {
                                 Id = x.PId,
                                 Nombre = persona.NombreCompleto,
                                 SegMotivo = x.SegMotivo,
                                 SegEstado = x.SegEstado,
                                 SegMedida = x.SegMedida,
                                 fecha_atencion = x.fecha_atencion,
                                 SegAlertaDesercion = x.SegAlertaDesercion,
                                 Mes = x.Mes,
                                 Año = x.Año,
                                 fechainicio = x.r_fechafin,
                                 idsocio = educacion.p_socio,
                                 idzona = educacion.IdZona,
                                 idprograma = educacion.programa.IdPrograma,
                                 idsede = educacion.p_sede,
                                 idsexo = persona.Sexo,
                                 idTipomatricula = educacion.p_tipobeca,
                                 idDepartament = persona.Departamento,
                                 refiere = persona.Refiere,
                                 year1 = educacion.Year,
                                 nombreSocio = educacion.socio.Nombre,
                                 nombreSede = educacion.sede.Nombre,
                                 tipoMatricula = educacion.tipoMatricula.Nombre,
                                 nombreCarrera = educacion.carrera.Nombre,
                                 idCarreta = educacion.CarreraCursoGrado,
                                 cohorte = educacion.Cohorte,
                                 nombreCohorte = educacion.cohorte.Nombre
                             }).ToList();
                DateTime date = DateTime.Now;
                DateTime oPrimerDiaDelMes = new DateTime(date.Year, date.Month, 1);
                DateTime MesAnt = oPrimerDiaDelMes.AddMonths(-3);
                var f = evpsi.Where(x => x.fechainicio > MesAnt).ToList();
                var ED = f.OrderByDescending(x => x.fechainicio).DistinctBy(x => x.Id).ToList();

                if (socio.Count() != 0)
                {
                    ED = ED.Where(m => m.idsocio != null).ToList();
                    ED = ED.Where(x => socio.Contains((int)x.idsocio)).ToList();
                }
                if (zonas.Count() != 0)
                {
                    ED = ED.Where(m => m.idzona != null).ToList();
                    ED = ED.Where(x => zonas.Contains((int)x.idzona)).ToList();
                }
                if (programa.Count() != 0)
                {
                    ED = ED.Where(m => m.idprograma != null).ToList();
                    ED = ED.Where(x => programa.Contains((int)x.idprograma)).ToList();
                }
                if (sedes.Count() != 0)
                {
                    ED = ED.Where(m => m.idsede != null).ToList();
                    ED = ED.Where(x => sedes.Contains((int)x.idsede)).ToList();
                }
                if (carreras.Count() != 0)
                {
                    ED = ED.Where(m => m.idCarreta != null).ToList();
                    ED = ED.Where(x => carreras.Contains((int)x.idCarreta)).ToList();
                }
                if (sexos.Count() != 0)
                {
                    ED = ED.Where(m => m.idsexo != null).ToList();
                    ED = ED.Where(x => sexos.Contains((int)x.idsexo)).ToList();
                }
                if (tipomatricula.Count() != 0)
                {
                    ED = ED.Where(m => m.idTipomatricula != null).ToList();
                    ED = ED.Where(x => tipomatricula.Contains((int)x.idTipomatricula)).ToList();
                }
                if (departament.Count() != 0)
                {
                    ED = ED.Where(m => m.idDepartament != null).ToList();
                    ED = ED.Where(x => departament.Contains((int)x.idDepartament)).ToList();
                }
                if (refiere.Count() != 0)
                {
                    ED = ED.Where(m => m.refiere != null).ToList();
                    ED = ED.Where(x => refiere.Contains((int)x.refiere)).ToList();
                }
                if (year1.Count() != 0)
                {
                    ED = ED.Where(m => m.Año != null).ToList();
                    ED = ED.Where(x => year1.Contains((int)x.Año)).ToList();
                }
                if (mes.Count() != 0)
                {
                    ED = ED.Where(m => m.Mes != null).ToList();
                    ED = ED.Where(x => mes.Contains(x.Mes)).ToList();
                }
                if (aEstudio.Count() != 0)
                {
                    ED = ED.Where(m => m.year1 != null).ToList();
                    ED = ED.Where(x => aEstudio.Contains((int)x.year1)).ToList();
                }
                if (cohorte1.Count() != 0)
                {
                    ED = ED.Where(m => m.cohorte != null).ToList();
                    ED = ED.Where(x => cohorte1.Contains((int)x.cohorte)).ToList();
                }

                return Json(new { data = ED });
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
                        persona = _ctx.Personas.Include(f => f.sexo).FirstOrDefault(n => n.PIdOim == x.PId)
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
