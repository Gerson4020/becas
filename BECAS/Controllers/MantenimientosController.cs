using BECAS.Models;
using BECAS.Models.VM;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BECASLC;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using BECAS.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using BECAS.Interfaces;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;

namespace BECAS.Controllers
{
    //[ResponseHeader]
    [Authorize(Policy = "ADOnly")]
    //[Authorize(Policy = "ExternalUsers")]
    public class MantenimientosController : Controller
    {
        private readonly MEOBContext _ctx;
        private readonly IPersonas _persona;
        private readonly IEncryptionService _encryptionService;
        public MantenimientosController(MEOBContext ctx, IPersonas personas, IEncryptionService encryptionService)
        {
            _ctx = ctx;
            _persona = personas;
            _encryptionService = encryptionService;
        }

        #region MTTO PERSONAS
        // GET: MantenimientosController
        public async Task<ActionResult> Personas()
        {
            var vM = new PersonasVM
            {
                Sexos = await _ctx.Sexos.Where(x => x.Activo == 1).ToListAsync(),
                // Restablecer los datos comentados si es necesario:
                Socios = await _ctx.SocioImplementadors.Where(x => x.Activo == true).ToListAsync(),
                tipomatricula = await _ctx.TipoMatriculas.Where(x => x.Activo == true).ToListAsync(),
                departamentos = await _ctx.Departamentos.Where(x => x.Activo == true).ToListAsync(),
                dropRefiereCM = await _ctx.Refieres.Where(x => x.Activo == true).ToListAsync(),
                cohorte = await _ctx.Cohortes.Where(x => x.Activo == true).ToListAsync(),
                Sectors = await _ctx.Sectors.Where(x => x.Activo == true).ToListAsync(),
                estadoPersona = await _ctx.EstadoPersonas.Where(x => x.Activo == true).ToListAsync(),
                catAños = await _ctx.CatAños.Where(x => x.Activo == true).ToListAsync(),
                proyectos = await _ctx.Proyectos.Where(x => x.Activo == true).ToListAsync()
            };

            return View(vM);
        }


        [HttpPost]
        public ActionResult Personas(DTParameters param)
        {
            List<PersonTableVM> ListPersonas = _persona.GetPersonas(param);

            string searchValue = param.Search?.Value;

            // Aplicar filtro de búsqueda si se proporcionó un término de búsqueda
            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                ListPersonas = ListPersonas.Where(item =>
                    item.NombreCompleto.Contains(searchValue) ||
                    item.Telefono1.Contains(searchValue)
                // Agrega más condiciones de filtrado si es necesario
                ).ToList();

                // Verifica si hay alguna coincidencia en al menos uno de los campos
                if (ListPersonas.Count == 0)
                {
                    ListPersonas = ListPersonas.Where(item =>
                        item.NombreCompleto.Contains(searchValue) ||
                        item.Telefono1.Contains(searchValue)
                    // Agrega más condiciones de filtrado si es necesario
                    ).ToList();
                }
            }

            // Realiza una consulta para contar el número total de registros
            var totalRegistros = param.count;



            // Devolver los datos en formato JSON con información adicional necesaria para DataTables
            return Json(new DTResult<PersonTableVM>
            {
                draw = param.Draw,
                recordsTotal = (int)totalRegistros, // Total de registros sin paginar
                recordsFiltered = (int)totalRegistros, // Total de registros después de aplicar filtros (si los hay)
                data = ListPersonas // Registros para la página actual
            });
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

        public async Task<JsonResult> GetZona(string id)
        {
            try
            {
                List<int> ids = string.IsNullOrEmpty(id) ? new List<int>() : id.Split(',').Select(int.Parse).ToList();

                var socio = _ctx.Sedes.Include(x => x.catsede).Include(i => i.zona).Where(x => ids.Contains((int)x.IdSocio)).Select(z => z.zona).Distinct().ToList();
                var sedes = socio.Select(x => new { id = x.IdZona, nombre = x.Nombre });

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
                var cascade = sedes.Select(x => new { id = x.IdSede, nombre = x.catsede.Nombre });
                return Json(cascade);
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
                var programas = sedes.Select(x => new { id = x.IdPrograma, nombre = x.Nombre });
                return Json(programas);
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

        public async Task<JsonResult> GetCarreras(int id, int ids, int idp)
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
                    var Cat = carreras.Select(x => new { id = x.idCarrera, nombre = x.nombre, IsCarrera = x.IsCarrera, idSede = x.idSede, activo = x.activo, cohorte = x.cohorte });
                    return Json(Cat);
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
                    var gra = grados.Select(x => new { id = x.idCarrera, nombre = x.nombre, IsCarrera = x.IsCarrera, idSede = x.idSede, activo = x.activo, cohorte = x.cohorte });
                    return Json(gra);
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        [HttpPost]
        public async Task<IActionResult> DownloadPersonsExcel([FromBody] DTParameters filter)
        {

            List<PersonTableVM> ListPersonas = _persona.GetPersonasD(filter);

            // Crear un nuevo libro de Excel
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Inscritos");

            // Agregar encabezados
            worksheet.Cell(1, 1).Value = "PIdOim";
            worksheet.Cell(1, 2).Value = "NumeroInscripciones";
            worksheet.Cell(1, 3).Value = "NombreCompleto";
            worksheet.Cell(1, 4).Value = "NombreSocio";
            worksheet.Cell(1, 5).Value = "NombreSede";
            worksheet.Cell(1, 6).Value = "Matricula";
            worksheet.Cell(1, 7).Value = "Programa";
            worksheet.Cell(1, 8).Value = "Carrera";
            worksheet.Cell(1, 9).Value = "Telefono1";
            worksheet.Cell(1, 10).Value = "Telefono2";
            worksheet.Cell(1, 11).Value = "Sexo";
            worksheet.Cell(1, 12).Value = "LGBTIQ";
            worksheet.Cell(1, 13).Value = "FechaNacimiento";
            worksheet.Cell(1, 14).Value = "Edad";
            worksheet.Cell(1, 15).Value = "Discapacidad";
            worksheet.Cell(1, 16).Value = "VictimaViolencia";
            worksheet.Cell(1, 17).Value = "MigranteRetornado";
            worksheet.Cell(1, 18).Value = "PiensaMigrar";
            worksheet.Cell(1, 19).Value = "FamiliaresMigrantes";
            worksheet.Cell(1, 20).Value = "FamiliaresRetornados";
            worksheet.Cell(1, 21).Value = "Empleo";
            worksheet.Cell(1, 22).Value = "Dui";
            worksheet.Cell(1, 23).Value = "Nie";
            worksheet.Cell(1, 24).Value = "Correo";
            worksheet.Cell(1, 25).Value = "Refiere";
            worksheet.Cell(1, 26).Value = "Departamento";
            worksheet.Cell(1, 27).Value = "Municipio";
            worksheet.Cell(1, 28).Value = "UltimoGradoAprobado";
            worksheet.Cell(1, 29).Value = "NivelAcademico";
            worksheet.Cell(1, 30).Value = "AñoEstudio";
            worksheet.Cell(1, 31).Value = "NombreEstado";

            // Agregar más encabezados según tus necesidades

            // Llenar datos
            for (int i = 0; i < ListPersonas.Count; i++)
            {
                var person = ListPersonas[i];
                worksheet.Cell(i + 2, 1).Value = person.PIdOim;
                worksheet.Cell(i + 2, 2).Value = person.NumeroInscripciones;
                worksheet.Cell(i + 2, 3).Value = person.NombreCompleto;
                worksheet.Cell(i + 2, 4).Value = person.p_socioNombre;
                worksheet.Cell(i + 2, 5).Value = person.p_sedeNombre;
                worksheet.Cell(i + 2, 6).Value = person.tipomatricula;
                worksheet.Cell(i + 2, 7).Value = person.programa;
                worksheet.Cell(i + 2, 8).Value = person.carrera;
                worksheet.Cell(i + 2, 9).Value = person.Telefono1;
                worksheet.Cell(i + 2, 10).Value = person.Telefono2;
                worksheet.Cell(i + 2, 11).Value = person.SexoNombre;
                worksheet.Cell(i + 2, 12).Value = person.LGBTIQ;
                worksheet.Cell(i + 2, 13).Value = person.FechaNacimiento;
                worksheet.Cell(i + 2, 14).Value = person.Edad;
                worksheet.Cell(i + 2, 15).Value = person.Discapacidad;
                worksheet.Cell(i + 2, 16).Value = person.VictimaViolencia;
                worksheet.Cell(i + 2, 17).Value = person.MigranteRetornadoNombre;
                worksheet.Cell(i + 2, 18).Value = person.PiensaMigrar;
                worksheet.Cell(i + 2, 19).Value = person.FamiliaresMigrantes;
                worksheet.Cell(i + 2, 20).Value = person.FamiliaresRetornados;
                worksheet.Cell(i + 2, 21).Value = person.Empleo;
                worksheet.Cell(i + 2, 22).Value = person.Dui;
                worksheet.Cell(i + 2, 23).Value = person.Nie;
                worksheet.Cell(i + 2, 24).Value = person.Correo;
                worksheet.Cell(i + 2, 25).Value = person.Refiere;
                worksheet.Cell(i + 2, 26).Value = person.DepartamentoNombre;
                worksheet.Cell(i + 2, 27).Value = person.MunicipioNombre;
                worksheet.Cell(i + 2, 28).Value = person.UltimoGradoAprobado;
                worksheet.Cell(i + 2, 29).Value = person.NivelAcademico;
                worksheet.Cell(i + 2, 30).Value = person.añoestudio;
                worksheet.Cell(i + 2, 31).Value = person.Estado;
                // Agregar más datos según tus necesidades
            }

            // Guardar el libro de Excel en un flujo de memoria
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                // Devolver el archivo Excel como una descarga al navegador
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Inscritos.xlsx");
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

        #region SeguimientoAutoempleo(Emprendimiento)
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
                List<EducacionTable> consulta = new List<EducacionTable>();
                // Consulta base de datos para cargar los datos relevantes
                string query = @"WITH EducacionConDetalles AS (
                                    SELECT 
                                        e.p_id_oim,
                                        p.NombreCompleto AS nombre,
                                		e.r_fechaini,
                                        e.d_fechades,
                                        e.d_fecha_reasg,
                                        es.Nombre AS DEstado,
                                        e.i_dias_asistencia_efectivos,
                                        e.i_dias_asistencia_establecidos,
                                        e.i_motivo_inasistencia,
                                        e.i_modulos_inscritos,
                                        e.i_modulos_aprobados,
                                        e.i_modulos_reprobados,
                                        e.i_causa_reprobacion,
                                        e.p_socio,
                                        s.Nombre AS socio,
                                        e.p_sede,
                                        se.Nombre AS sede,
                                        e.CarreraCursoGrado,
                                        CASE 
                                            WHEN e.p_tipobeca IN (3,4,6) THEN (SELECT g.Nombre FROM [dbo].[Grados] g WHERE g.IdGrado = e.CarreraCursoGrado)
                                            ELSE c.Nombre
                                        END AS carrera,
                                        p.Sexo AS sexoID,
                                        sx.Nombre AS sexoNombre,
                                        p.Departamento AS departamento,
                                        p.Refiere AS refiere,
                                        r.Nombre AS refiereNombre,
                                        e.p_tipobeca AS programa,
                                        pr.Nombre AS programaNombre,
                                        e.r_año,
                                        e.r_mes,
                                        e.Cohorte,
                                        co.Nombre AS CohorteNombre,
                                        e.Year AS year,
                                        tm.Nombre AS tipoMatricula,
                                        e.d_motivodesercion AS motivodesercion,
                                        e.i_proc_asistencia AS pocentajeasistencia,
                                        e.r_fechaini AS fechainicio,
                                        e.IdZona AS zona,
                                        e.d_estado AS EstadoPersona,
                                        ROW_NUMBER() OVER(PARTITION BY e.p_id_oim ORDER BY e.r_fechaini DESC) AS row_num
                                    FROM
                                        [dbo].[CargaEducacion] e
                                    JOIN
                                        [dbo].[Persona] p ON e.p_id_oim = p.PIdOim
                                    LEFT JOIN
                                        [dbo].[SocioImplementador] s ON e.p_socio = s.IdImplementador
                                    LEFT JOIN
                                        [dbo].[CatSede] se ON e.p_sede = se.IdCatSede
                                    LEFT JOIN
                                        [dbo].[CatCarrera] c ON e.CarreraCursoGrado = c.IdCatCarrera
                                    LEFT JOIN
                                        [dbo].[Programa] pr ON e.p_tipobeca = pr.IdPrograma
                                    LEFT JOIN
                                        [dbo].[Cohorte] co ON e.Cohorte = co.IdCohorte
                                    LEFT JOIN
                                        [dbo].[EstadoPersona] es ON e.d_estado = es.IdEstadoPersona
                                    LEFT JOIN
                                        [dbo].[TipoMatricula] tm ON e.p_matricula = tm.IdTipoMatricula
                                    LEFT JOIN
                                        [dbo].[Grados] g ON e.p_tipobeca = g.IdGrado
                                    LEFT JOIN
                                        [dbo].[Sexo] sx ON p.Sexo = sx.IdSexo
                                    LEFT JOIN
                                        [dbo].[Refiere] r ON p.refiere = r.IdRefiere
                                )
                                SELECT 
                                    p_id_oim,
                                    nombre,
                                	r_fechaini,
                                    d_fechades,
                                    d_fecha_reasg,
                                    DEstado,
                                    i_dias_asistencia_efectivos,
                                    i_dias_asistencia_establecidos,
                                    i_motivo_inasistencia,
                                    i_modulos_inscritos,
                                    i_modulos_aprobados,
                                    i_modulos_reprobados,
                                    i_causa_reprobacion,
                                    p_socio,
                                    socio,
                                    p_sede,
                                    sede,
                                    CarreraCursoGrado,
                                    carrera,
                                    sexoID,
                                    sexoNombre,
                                    departamento,
                                    refiere,
                                    refiereNombre,
                                    programa,
                                    programaNombre,
                                    r_año,
                                    r_mes,
                                    Cohorte,
                                    CohorteNombre,
                                    year,
                                    tipoMatricula,
                                    motivodesercion,
                                    pocentajeasistencia,
                                    fechainicio,
                                    zona,
                                    EstadoPersona
                                FROM 
                                    EducacionConDetalles
                                WHERE 
                                    row_num = 1
                                ORDER BY 
                                    fechainicio DESC;";
                // Crea la conexión y ejecuta la consulta
                using (SqlConnection connection = new SqlConnection(_ctx.Database.GetConnectionString()))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            EducacionTable model = new EducacionTable
                            {
                                PIdOim = reader["p_id_oim"] != DBNull.Value ? Convert.ToString(reader["p_id_oim"]) : string.Empty,
                                nombre = reader["nombre"] != DBNull.Value ? Convert.ToString(reader["nombre"]) : string.Empty,
                                fechainicio = reader["r_fechaini"] != DBNull.Value ? Convert.ToDateTime(reader["r_fechaini"]) : DateTime.MinValue,
                                DFechades = reader["d_fechades"] != DBNull.Value ? Convert.ToDateTime(reader["d_fechades"]) : DateTime.MinValue,
                                DFechaReasg = reader["d_fecha_reasg"] != DBNull.Value ? Convert.ToDateTime(reader["d_fecha_reasg"]) : DateTime.MinValue,
                                IDiasAsistenciaEfectivos = reader["i_dias_asistencia_efectivos"] != DBNull.Value ? Convert.ToInt32(reader["i_dias_asistencia_efectivos"]) : 0,
                                IDiasAsistenciaEstablecidos = reader["i_dias_asistencia_establecidos"] != DBNull.Value ? Convert.ToInt32(reader["i_dias_asistencia_establecidos"]) : 0,
                                IMotivoInasistencia = reader["i_motivo_inasistencia"] != DBNull.Value ? Convert.ToString(reader["i_motivo_inasistencia"]) : string.Empty,
                                IModulosInscritos = reader["i_modulos_inscritos"] != DBNull.Value ? Convert.ToInt32(reader["i_modulos_inscritos"]) : 0,
                                IModulosAprobados = reader["i_modulos_aprobados"] != DBNull.Value ? Convert.ToInt32(reader["i_modulos_aprobados"]) : 0,
                                IModulosReprobados = reader["i_modulos_reprobados"] != DBNull.Value ? Convert.ToInt32(reader["i_modulos_reprobados"]) : 0,
                                ICausaReprobacion = reader["i_causa_reprobacion"] != DBNull.Value ? Convert.ToString(reader["i_causa_reprobacion"]) : string.Empty,
                                p_socio = reader["p_socio"] != DBNull.Value ? Convert.ToInt32(reader["p_socio"]) : 0,
                                socio = reader["socio"] != DBNull.Value ? Convert.ToString(reader["socio"]) : string.Empty,
                                p_sede = reader["p_sede"] != DBNull.Value ? Convert.ToInt32(reader["p_sede"]) : 0,
                                sede = reader["sede"] != DBNull.Value ? Convert.ToString(reader["sede"]) : string.Empty,
                                CarreraCursoGrado = reader["CarreraCursoGrado"] != DBNull.Value ? Convert.ToInt32(reader["CarreraCursoGrado"]) : 0,
                                carrera = reader["carrera"] != DBNull.Value ? Convert.ToString(reader["carrera"]) : string.Empty,
                                sexoID = reader["sexoID"] != DBNull.Value ? Convert.ToInt32(reader["sexoID"]) : 0,
                                sexoNombre = reader["sexoNombre"] != DBNull.Value ? Convert.ToString(reader["sexoNombre"]) : string.Empty,
                                departamento = reader["departamento"] != DBNull.Value ? Convert.ToInt32(reader["departamento"]) : 0,
                                refiere = reader["refiere"] != DBNull.Value ? Convert.ToInt32(reader["refiere"]) : 0,
                                refiereNombre = reader["refiereNombre"] != DBNull.Value ? Convert.ToString(reader["refiereNombre"]) : string.Empty,
                                programa = reader["programa"] != DBNull.Value ? Convert.ToInt32(reader["programa"]) : 0,
                                programaNombre = reader["programaNombre"] != DBNull.Value ? Convert.ToString(reader["programaNombre"]) : string.Empty,
                                RAño = reader["r_año"] != DBNull.Value ? Convert.ToInt32(reader["r_año"]) : 0,
                                RMes = reader["r_mes"] != DBNull.Value ? Convert.ToString(reader["r_mes"]) : string.Empty,
                                CohorteNombre = reader["CohorteNombre"] != DBNull.Value ? Convert.ToString(reader["CohorteNombre"]) : string.Empty,
                                year = reader["year"] != DBNull.Value ? Convert.ToInt32(reader["year"]) : 0,
                                tipoMatricula = reader["tipoMatricula"] != DBNull.Value ? Convert.ToString(reader["tipoMatricula"]) : string.Empty,
                                motivodesercion = reader["motivodesercion"] != DBNull.Value ? Convert.ToString(reader["motivodesercion"]) : string.Empty,
                                pocentajeasistencia = reader["pocentajeasistencia"] != DBNull.Value ? Convert.ToString(reader["pocentajeasistencia"]) : string.Empty,
                                zona = reader["zona"] != DBNull.Value ? Convert.ToInt32(reader["zona"]) : 0,
                                EstadoPersona = reader["EstadoPersona"] != DBNull.Value ? Convert.ToInt32(reader["EstadoPersona"]) : 0,
                                DEstado = reader["DEstado"] != DBNull.Value ? Convert.ToString(reader["DEstado"]) : string.Empty

                            };
                            consulta.Add(model);
                        }
                    }
                }

                // Filtrado
                if (socios.Count > 0)
                {
                    consulta = consulta.Where(m => m.p_socio != null).ToList();
                    consulta = consulta.Where(x => socios.Contains((int)x.p_socio)).ToList();
                }
                if (sedes.Count > 0)
                {
                    // Obtener las sedes de la base de datos que coinciden con los IDs filtrados
                    var catSedes = _ctx.Sedes
                        .Where(sede => sedes.Contains(sede.IdSede))
                        .ToList();

                    // Filtrar personas que tienen una sede válida y que están en la lista de sedes filtradas
                    consulta = consulta
                        .Where(x => x.p_sede.HasValue && catSedes.Any(sede => sede.IdCatSede == x.p_sede.Value))
                        .ToList();
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

                return Json(new { data = consulta });
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

        #region Seguimiento pasantias(Contrataciones)
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
                        pas_periodo = x.pas_periodo,
                        pas_tipo_empleo = x.pas_tipo_empleo,
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
                //DateTime date = DateTime.Now;
                //DateTime oPrimerDiaDelMes = new DateTime(date.Year, date.Month, 1);
                //DateTime MesAnt = oPrimerDiaDelMes.AddMonths(-3);
                //var f = evpsi.Where(x => x.fechainicio > MesAnt).ToList();
                var ED = evpsi.OrderByDescending(x => x.fechainicio).DistinctBy(x => x.Id).ToList();

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

        #region Reportes
        public ActionResult Promovidos()
        {
            try
            {
                List<PersonTableVM> ListPersonas = _persona.RepPromovidos();
                return View(ListPersonas);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region BuscarBeneficiarios
        public ActionResult SearchBeneficiary()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SearchBeneficiary(DTSearchBeneficiary param)
        {
            // Inicializa la lista de personas
            List<SearchBeneficiaryTable> ListPersonas = new List<SearchBeneficiaryTable>();

            // Verifica si se proporcionaron parámetros de búsqueda


            if (param.dui != null || param.nombre != null || param.aperllido != null || param.telefono1 != null || param.telefono2 != null || param.correo != null)
            {
                // Obtiene la lista de personas basada en los parámetros proporcionados
                ListPersonas = _persona.GetSearchBeneficiaries(param);

                // Obtiene el valor de búsqueda
                string searchValue = param.Search?.Value;

                // Aplica filtro de búsqueda si se proporciona un término de búsqueda
                if (!string.IsNullOrWhiteSpace(searchValue))
                {
                    ListPersonas = ListPersonas.Where(item =>
                        item.NombreCompleto.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                        item.Nombre.Contains(searchValue, StringComparison.OrdinalIgnoreCase)
                    // Agrega más condiciones de filtrado si es necesario
                    ).ToList();
                }
            }

            // Realiza una consulta para contar el número total de registros
            var totalRegistros = param.count != null ? (int)param.count : 0;

            // Devolver los datos en formato JSON con información adicional necesaria para DataTables
            return Json(new DTResult<SearchBeneficiaryTable>
            {
                draw = param.Draw,
                recordsTotal = totalRegistros, // Total de registros sin paginar
                recordsFiltered = ListPersonas.Count, // Total de registros después de aplicar filtros
                data = ListPersonas // Registros para la página actual
            });
        }
        #endregion

        
    }
}
