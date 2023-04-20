using Aspose.Cells;
using BECAS.Models;
using BECAS.Models.VM;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using BECASLC;
using BECAS.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BECAS.Controllers
{
    public class CargaDatosController : Controller
    {
        private IHostingEnvironment Environment;
        private readonly MEOBContext _ctx;
        private readonly ICatalogos _catalogos;

        public CargaDatosController(IHostingEnvironment _environment, MEOBContext ctx, ICatalogos catalogos)
        {
            Environment = _environment;
            _ctx = ctx;
            _catalogos = catalogos;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(List<IFormFile> postedFiles)
        {
            try
            {
                //GUARDAR ARCHIVO EN UNA CARPETA DEL SERVIDOR
                string path = Path.Combine(this.Environment.WebRootPath, "Uploads");
                var SFile = SaveFile(postedFiles, path);

                //EJECUTAR LA CARGA DE DATOS A LA BASE DE DATOS
                var PersonasAsync = LoadPersonasAsync(path, SFile.FileName);
                await Task.WhenAll(PersonasAsync);
                ViewBag.Message += string.Format("{0}", PersonasAsync.Result);
            }
            catch (Exception e)
            {
                ViewBag.Message += string.Format("{0}", e.Message);
            }

            return View();
        }

        public static SaveFileVM SaveFile(List<IFormFile> postedFiles, string path)
        {

            List<string> uploadedFiles = new List<string>();
            SaveFileVM vM = new SaveFileVM();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            foreach (IFormFile postedFile in postedFiles)
            {
                vM.FileName = Path.GetFileName(postedFile.FileName);
                try
                {
                    using (FileStream stream = new FileStream(Path.Combine(path, vM.FileName), FileMode.Create))
                    {
                        postedFile.CopyTo(stream);
                        uploadedFiles.Add(vM.FileName);

                    }
                }
                catch (Exception ex)
                {
                    vM.Estado = false;
                    vM.FileName = ex.Message;
                    return vM;
                }
            }

            return vM;
        }

        public async Task<string> LoadPersonasAsync(string path, string fileName)
        {
            try
            {

                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var stream = System.IO.File.Open(Path.Combine(path, fileName), FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var dbTransaction = _ctx.Database.BeginTransaction();
                        Carga carga = new Carga();
                        var result = reader.AsDataSet();
                        //Control de carga
                        try
                        {
                            DataTable tablepersonas = result.Tables[0];
                            DataTable tableeducacion = result.Tables[1];
                            DataTable tablepsicosocial = result.Tables[2];
                            DataTable tableSeguimientoPsicosocial = result.Tables[3];
                            DataTable tableSeguimientosPr = result.Tables[4];
                            DataTable tableSeguimientPasantillas = result.Tables[5];
                            DataTable SeguimientoAutoempleo = result.Tables[6];
                            DataTable tableEstipendios = result.Tables[7];


                            carga.TotalInscripcion = tablepersonas.Rows.Count;
                            carga.TotalEducacion = tableeducacion.Rows.Count;
                            carga.TotalPsicosocial = tablepsicosocial.Rows.Count;
                            carga.TotalSegPsicosocial = tableSeguimientoPsicosocial.Rows.Count;
                            carga.TotalSeguimientoPracticasPr = tableSeguimientosPr.Rows.Count;
                            carga.TotalSeguimientoPasantias = tableSeguimientPasantillas.Rows.Count;
                            carga.TotalSeguimientoAutoempleo = SeguimientoAutoempleo.Rows.Count;
                            carga.TotalEstipendios = tableEstipendios.Rows.Count;

                            carga.FechaCarga = DateTime.Today;
                            _ctx.Add(carga);
                            await _ctx.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            string message = ex.Message;
                            dbTransaction.Rollback();
                            throw;
                        }

                        //Carga persona
                        int loopPer = 0;
                        try
                        {

                            //Tabla de estudiantes incritos
                            DataTable tablepersonas = result.Tables[0];
                            foreach (DataRow item in tablepersonas.Rows)
                            {
                                if (loopPer != 0)
                                {

                                    Persona p = new Persona();
                                    p = _ctx.Personas.FirstOrDefault(x => x.PIdOim.Equals(item[2].ToString()));
                                    if (p != null)
                                    {
                                        if (!string.IsNullOrEmpty(item[3].ToString()))
                                        {
                                            var matricula = _catalogos.GetTipoMatricula(item[3].ToString());
                                            p.TipoMatricula = matricula != null ? matricula.IdTipoMatricula : null;
                                        }
                                        p.Nombre = string.IsNullOrEmpty(item[4].ToString()) ? "" : item[4].ToString();
                                        p.Apellido = string.IsNullOrEmpty(item[5].ToString()) ? "" : item[5].ToString();
                                        p.NombreCompleto = string.IsNullOrEmpty(item[6].ToString()) ? "" : item[6].ToString();
                                        p.UltimoGradoAprobado = string.IsNullOrEmpty(item[7].ToString()) ? "" : item[7].ToString();
                                        p.NivelAcademico = string.IsNullOrEmpty(item[8].ToString()) ? "" : item[8].ToString();
                                        p.Telefono1 = string.IsNullOrEmpty(item[9].ToString()) ? "" : item[9].ToString();
                                        p.Telefono2 = string.IsNullOrEmpty(item[10].ToString()) ? "" : item[10].ToString();
                                        if (!string.IsNullOrEmpty(item[11].ToString()))
                                        {
                                            var sexo = _catalogos.GetSexo(item[11].ToString());
                                            p.Sexo = sexo != null ? sexo.IdSexo : null;
                                        }
                                        p.FechaNacimiento = string.IsNullOrEmpty(item[12].ToString()) ? null : Convert.ToDateTime(item[12].ToString());
                                        p.Edad = string.IsNullOrEmpty(item[13].ToString()) ? 0 : Convert.ToInt32(item[13].ToString());
                                        p.Discapacidad = string.IsNullOrEmpty(item[14].ToString()) ? "" : item[14].ToString();
                                        p.VictimaViolencia = string.IsNullOrEmpty(item[15].ToString()) ? "" : item[15].ToString();
                                        p.MigranteRetornado = string.IsNullOrEmpty(item[16].ToString()) ? "" : item[16].ToString();
                                        p.PiensaMigrar = string.IsNullOrEmpty(item[17].ToString()) ? "" : item[17].ToString();
                                        p.FamiliaresMigrantes = string.IsNullOrEmpty(item[18].ToString()) ? "" : item[18].ToString();
                                        p.FamiliaresRetornados = string.IsNullOrEmpty(item[19].ToString()) ? "" : item[19].ToString();
                                        p.Empleo = string.IsNullOrEmpty(item[20].ToString()) ? "" : item[20].ToString();
                                        p.Dui = string.IsNullOrEmpty(item[21].ToString()) ? "" : item[21].ToString();
                                        p.Nie = string.IsNullOrEmpty(item[22].ToString()) ? "" : item[22].ToString();
                                        p.Correo = string.IsNullOrEmpty(item[23].ToString()) ? "" : item[23].ToString();
                                        if (!string.IsNullOrEmpty(item[24].ToString()))
                                        {
                                            var refiere = _catalogos.GetReferencias(item[24].ToString());
                                            p.Refiere = refiere != null ? refiere.IdRefiere : null;
                                        }
                                        if (!string.IsNullOrEmpty(item[25].ToString()))
                                        {
                                            var departamento = _catalogos.GetDepartamento(item[25].ToString());
                                            p.Departamento = departamento != null ? departamento.IdDepartamento : null;
                                        }
                                        if (!string.IsNullOrEmpty(item[26].ToString()))
                                        {
                                            var municipio = _catalogos.GetMunicipio(item[26].ToString());
                                            p.Municipio = municipio != null ? municipio.IdMunicipio : null;
                                        }
                                        if (!string.IsNullOrEmpty(item[27].ToString()))
                                        {
                                            var programa = _catalogos.GetPrograma(item[27].ToString());
                                            p.Programa = programa != null ? programa.IdPrograma : null;
                                        }
                                        p.CohorteYear = string.IsNullOrEmpty(item[28].ToString()) ? "" : item[28].ToString();
                                        p.Year = string.IsNullOrEmpty(item[29].ToString()) ? 0 : int.Parse(item[29].ToString());
                                        if (!string.IsNullOrEmpty(item[30].ToString()))
                                        {
                                            var cohorte = _catalogos.GetCohorte(item[30].ToString());
                                            p.Cohorte = cohorte != null ? cohorte.IdCohorte : null;
                                        }
                                        if (!string.IsNullOrEmpty(item[31].ToString()))
                                        {
                                            var sede = _catalogos.GetSede(item[31].ToString());
                                            p.Sede = sede != null ? sede.IdCatSede : null;
                                        }
                                        if (!string.IsNullOrEmpty(item[32].ToString()))
                                        {
                                            var socio = _catalogos.GetSocioImplementador(item[32].ToString());
                                            p.SocioIm = socio != null ? socio.IdImplementador : null;
                                        }
                                        if (!string.IsNullOrEmpty(item[33].ToString()))
                                        {
                                            var zona = _catalogos.GetZona(item[33].ToString());
                                            p.Zona = zona != null ? zona.IdZona : null;
                                        }
                                        p.EstadoInscripcion = string.IsNullOrEmpty(item[34].ToString()) ? "" : item[34].ToString();
                                        p.EstadoMf = string.IsNullOrEmpty(item[35].ToString()) ? "" : item[35].ToString();
                                        if (string.IsNullOrEmpty(item[36].ToString()))
                                        {
                                            var carrera = _catalogos.GetCarrera(item[36].ToString());
                                            p.CarreraCursoGrado = carrera != null ? carrera.IdCatCarrera : null;
                                        }
                                        p.Sector = string.IsNullOrEmpty(item[37].ToString()) ? "" : item[37].ToString();
                                        p.CartaCompromiso = string.IsNullOrEmpty(item[38].ToString()) ? "" : item[38].ToString();
                                        p.EstadoPersona = string.IsNullOrEmpty(item[39].ToString()) ? "" : item[39].ToString();

                                        _ctx.Entry(p).State = EntityState.Modified;
                                        //await _ctx.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        Persona persona = new Persona();
                                        persona.FechaEntrevista = string.IsNullOrEmpty(item[0].ToString()) ? null : Convert.ToDateTime(item[0].ToString());
                                        persona.Id = string.IsNullOrEmpty(item[1].ToString()) ? "" : item[1].ToString();
                                        persona.PIdOim = string.IsNullOrEmpty(item[2].ToString()) ? "" : item[2].ToString();
                                        if (!string.IsNullOrEmpty(item[3].ToString()))
                                        {
                                            var matricula = _catalogos.GetTipoMatricula(item[3].ToString());
                                            persona.TipoMatricula = matricula != null ? matricula.IdTipoMatricula : null;
                                        }
                                        persona.Nombre = string.IsNullOrEmpty(item[4].ToString()) ? "" : item[4].ToString();
                                        persona.Apellido = string.IsNullOrEmpty(item[5].ToString()) ? "" : item[5].ToString();
                                        persona.NombreCompleto = string.IsNullOrEmpty(item[6].ToString()) ? "" : item[6].ToString();
                                        persona.UltimoGradoAprobado = string.IsNullOrEmpty(item[7].ToString()) ? "" : item[7].ToString();
                                        persona.NivelAcademico = string.IsNullOrEmpty(item[8].ToString()) ? "" : item[8].ToString();
                                        persona.Telefono1 = string.IsNullOrEmpty(item[9].ToString()) ? "" : item[9].ToString();
                                        persona.Telefono2 = string.IsNullOrEmpty(item[10].ToString()) ? "" : item[10].ToString();
                                        if (!string.IsNullOrEmpty(item[11].ToString()))
                                        {
                                            var sexo = _catalogos.GetSexo(item[11].ToString());
                                            persona.Sexo = sexo != null ? sexo.IdSexo : null;
                                        }
                                        persona.FechaNacimiento = string.IsNullOrEmpty(item[12].ToString()) ? null : Convert.ToDateTime(item[12].ToString());
                                        persona.Edad = string.IsNullOrEmpty(item[13].ToString()) ? 0 : Convert.ToInt32(item[13].ToString());
                                        persona.Discapacidad = string.IsNullOrEmpty(item[14].ToString()) ? "" : item[14].ToString();
                                        persona.VictimaViolencia = string.IsNullOrEmpty(item[15].ToString()) ? "" : item[15].ToString();
                                        persona.MigranteRetornado = string.IsNullOrEmpty(item[16].ToString()) ? "" : item[16].ToString();
                                        persona.PiensaMigrar = string.IsNullOrEmpty(item[17].ToString()) ? "" : item[17].ToString();
                                        persona.FamiliaresMigrantes = string.IsNullOrEmpty(item[18].ToString()) ? "" : item[18].ToString();
                                        persona.FamiliaresRetornados = string.IsNullOrEmpty(item[19].ToString()) ? "" : item[19].ToString();
                                        persona.Empleo = string.IsNullOrEmpty(item[20].ToString()) ? "" : item[20].ToString();
                                        persona.Dui = string.IsNullOrEmpty(item[21].ToString()) ? "" : item[21].ToString();
                                        persona.Nie = string.IsNullOrEmpty(item[22].ToString()) ? "" : item[22].ToString();
                                        persona.Correo = string.IsNullOrEmpty(item[23].ToString()) ? "" : item[23].ToString();
                                        if (!string.IsNullOrEmpty(item[24].ToString()))
                                        {
                                            var refiere = _catalogos.GetReferencias(item[24].ToString());
                                            persona.Refiere = refiere != null ? refiere.IdRefiere : null;
                                        }
                                        if (!string.IsNullOrEmpty(item[25].ToString()))
                                        {
                                            var departamento = _catalogos.GetDepartamento(item[25].ToString());
                                            persona.Departamento = departamento != null ? departamento.IdDepartamento : null;
                                        }
                                        if (!string.IsNullOrEmpty(item[26].ToString()))
                                        {
                                            var municipio = _catalogos.GetMunicipio(item[26].ToString());
                                            persona.Municipio = municipio != null ? municipio.IdMunicipio : null;
                                        }
                                        if (!string.IsNullOrEmpty(item[27].ToString()))
                                        {
                                            var programa = _catalogos.GetPrograma(item[27].ToString());
                                            persona.Programa = programa != null ? programa.IdPrograma : null;
                                        }
                                        persona.CohorteYear = string.IsNullOrEmpty(item[28].ToString()) ? "" : item[28].ToString();
                                        persona.Year = string.IsNullOrEmpty(item[29].ToString()) ? 0 : int.Parse(item[29].ToString());
                                        if (!string.IsNullOrEmpty(item[30].ToString()))
                                        {
                                            var cohorte = _catalogos.GetCohorte(item[30].ToString());
                                            persona.Cohorte = cohorte != null ? cohorte.IdCohorte : null;
                                        }
                                        if (!string.IsNullOrEmpty(item[31].ToString()))
                                        {
                                            var sede = _catalogos.GetSede(item[31].ToString());
                                            persona.Sede = sede != null ? sede.IdCatSede : null;
                                        }
                                        if (!string.IsNullOrEmpty(item[32].ToString()))
                                        {
                                            var socio = _catalogos.GetSocioImplementador(item[32].ToString());
                                            persona.SocioIm = socio != null ? socio.IdImplementador : null;
                                        }
                                        if (!string.IsNullOrEmpty(item[33].ToString()))
                                        {
                                            var zona = _catalogos.GetZona(item[33].ToString());
                                            persona.Zona = zona != null ? zona.IdZona : null;
                                        }
                                        persona.EstadoInscripcion = string.IsNullOrEmpty(item[34].ToString()) ? "" : item[34].ToString();
                                        persona.EstadoMf = string.IsNullOrEmpty(item[35].ToString()) ? "" : item[35].ToString();
                                        if (string.IsNullOrEmpty(item[36].ToString()))
                                        {
                                            var carrera = _catalogos.GetCarrera(item[36].ToString());
                                            persona.CarreraCursoGrado = carrera != null ? carrera.IdCatCarrera : null;
                                        }
                                        persona.Sector = string.IsNullOrEmpty(item[37].ToString()) ? "" : item[37].ToString();
                                        persona.CartaCompromiso = string.IsNullOrEmpty(item[38].ToString()) ? "" : item[38].ToString();
                                        persona.EstadoPersona = string.IsNullOrEmpty(item[39].ToString()) ? "" : item[39].ToString();
                                        persona.IdCarga = carga.IdCarga;

                                        _ctx.Entry(persona).State = EntityState.Added;
                                        //await _ctx.SaveChangesAsync();
                                    }

                                }
                                loopPer++;
                            }
                            await _ctx.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            var linea = loopPer;
                            string msn = ex.Message;
                            dbTransaction.Rollback();
                            throw new Exception(string.Format("Error al cargar datos en la tabla Inscripciones verificar el número de fila {0}", linea));
                        }

                        // Tabla de educacion
                        int loopEd = 0;
                        try
                        {
                            DataTable tableeducacion = result.Tables[1];
                            foreach (DataRow item in tableeducacion.Rows)
                            {
                                if (loopEd != 0)
                                {
                                    //int año = Convert.ToInt32(item[0].ToString());
                                    string mes = item[1].ToString();
                                    string DFechaReasg = item[11].ToString().Trim();
                                    DateTime TimeReasg;
                                    string DFechades = item[13].ToString().Trim();
                                    DateTime timeFechades;
                                    string IModulosInscritos = item[19].ToString().Trim();
                                    string IModulosAprobados = item[20].ToString().Trim();
                                    string IModulosReprobados = item[22].ToString().Trim();
                                    string IDiasAsistenciaEstablecidos = item[15].ToString().Trim();
                                    string IMotivoInasistencia = item[18].ToString().Trim();
                                    string IDiasAsistenciaEfectivos = item[16].ToString().Trim();
                                    string DEstado = string.IsNullOrEmpty(item[12].ToString().Trim()) ? "" : item[12].ToString().Trim();



                                    CargaEducacion ccarga = new CargaEducacion();
                                    ccarga.PIdOim = item[6].ToString();

                                    if (!string.IsNullOrEmpty(DFechaReasg))
                                    {
                                        if (!DFechaReasg.Equals("N/A"))
                                        {
                                            bool success = DateTime.TryParse(DFechaReasg, out TimeReasg);
                                            ccarga.DFechaReasg = success == true ? TimeReasg : null;
                                        }
                                    }
                                    ccarga.DEstado = DEstado;

                                    if (!string.IsNullOrEmpty(DFechades))
                                    {
                                        if (!DFechades.Equals("N/A"))
                                        {
                                            bool success = DateTime.TryParse(DFechades, out timeFechades);
                                            ccarga.DFechades = success == true ? timeFechades : null;
                                        }
                                    }

                                    ccarga.DMotivodesercion = string.IsNullOrEmpty(item[14].ToString()) ? "" : item[14].ToString();
                                    ccarga.IDiasAsistenciaEstablecidos = string.IsNullOrEmpty(IDiasAsistenciaEstablecidos) ? 0 : Convert.ToInt32(IDiasAsistenciaEstablecidos);
                                    ccarga.IDiasAsistenciaEfectivos = string.IsNullOrEmpty(IDiasAsistenciaEfectivos) ? 0 : Convert.ToInt32(IDiasAsistenciaEfectivos);
                                    ccarga.IMotivoInasistencia = string.IsNullOrEmpty(IMotivoInasistencia) ? "" : IMotivoInasistencia;
                                    ccarga.IModulosInscritos = string.IsNullOrEmpty(IModulosInscritos) ? 0 : IModulosInscritos.Equals("N/A") ? 0 : Convert.ToInt32(IModulosInscritos);
                                    ccarga.IModulosAprobados = string.IsNullOrEmpty(IModulosAprobados) ? 0 : IModulosAprobados.Equals("N/A") ? 0 : Convert.ToInt32(IModulosAprobados);
                                    ccarga.IModulosReprobados = string.IsNullOrEmpty(IModulosReprobados) ? 0 : IModulosReprobados.Equals("N/A") ? 0 : Convert.ToInt32(IModulosReprobados);
                                    ccarga.ICausaReprobacion = string.IsNullOrEmpty(item[23].ToString()) ? "" : item[23].ToString();
                                    ccarga.IdCarga = carga.IdCarga;
                                    if (!string.IsNullOrEmpty(item[0].ToString()))
                                    {
                                        var year = _catalogos.GetYear(item[0].ToString());
                                        ccarga.RAño = year != null ? year.IdAño : null;
                                    }
                                    //ccarga.RAño = año;
                                    ccarga.RMes = mes;
                                    _ctx.Add(ccarga);

                                }
                                loopEd++;
                            }
                            await _ctx.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            var fila = loopEd;
                            string smn = ex.Message;
                            dbTransaction.Rollback();
                            throw new Exception(string.Format("Error al cargar datos en la tabla Educación verificar el número de fila {0}", fila));
                        }

                        //Tabla psicosocial
                        int loopPsi = 0;
                        try
                        {
                            DataTable tablepsicosocial = result.Tables[2];
                            foreach (DataRow item in tablepsicosocial.Rows)
                            {
                                if (loopPsi != 0)
                                {

                                    CargaEvaluacionPsicosocial cargaEvaluacion = new CargaEvaluacionPsicosocial();
                                    int año = Convert.ToInt32(item[0].ToString());
                                    string mes = item[1].ToString();
                                    cargaEvaluacion.PId = string.IsNullOrEmpty(item[5].ToString()) ? "" : item[5].ToString();
                                    cargaEvaluacion.OvParticipacion = string.IsNullOrEmpty(item[9].ToString()) ? null : item[9].ToString().Equals("Si") ? true : false;
                                    cargaEvaluacion.OvPuntajePret = string.IsNullOrEmpty(item[10].ToString()) ? "" : item[10].ToString();
                                    cargaEvaluacion.OvPuntajePos = string.IsNullOrEmpty(item[11].ToString()) ? "" : item[11].ToString();
                                    cargaEvaluacion.EpInstrumentoRiesgo = string.IsNullOrEmpty(item[12].ToString()) ? "" : item[12].ToString();
                                    cargaEvaluacion.EpVulnerabilidades = string.IsNullOrEmpty(item[13].ToString()) ? "" : item[13].ToString();
                                    cargaEvaluacion.EpAlertaDesercion = string.IsNullOrEmpty(item[14].ToString()) ? "" : item[14].ToString();
                                    cargaEvaluacion.IdCarga = carga.IdCarga;
                                    cargaEvaluacion.Año = año;
                                    cargaEvaluacion.Mes = mes;
                                    _ctx.Add(cargaEvaluacion);

                                }
                                loopPsi++;
                            }
                            await _ctx.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            var fila = loopPsi;
                            string msn = ex.Message;
                            dbTransaction.Rollback();
                            throw new Exception(string.Format("Error al cargar datos en la tabla Psicosocial verificar el número de fila {0}", fila));
                        }

                        //Tabla seguimiento psicosocial
                        int loopSegPsi = 0;
                        try
                        {
                            DataTable tableSeguimientoPsicosocial = result.Tables[3];
                            foreach (DataRow item in tableSeguimientoPsicosocial.Rows)
                            {
                                if (loopSegPsi != 0)
                                {

                                    CargaSeguimientoPsicosocial ee = new CargaSeguimientoPsicosocial();
                                    int año = Convert.ToInt32(item[0].ToString());
                                    string mes = item[1].ToString();
                                    ee.PId = string.IsNullOrEmpty(item[5].ToString()) ? "" : item[5].ToString();
                                    ee.SegMotivo = string.IsNullOrEmpty(item[9].ToString()) ? "" : item[9].ToString();
                                    ee.SegEstado = string.IsNullOrEmpty(item[10].ToString()) ? "" : item[10].ToString();
                                    ee.SegMedida = string.IsNullOrEmpty(item[11].ToString()) ? "" : item[11].ToString();
                                    ee.SegAlertaDesercion = string.IsNullOrEmpty(item[12].ToString()) ? "" : item[12].ToString();
                                    ee.IdCarga = carga.IdCarga;
                                    ee.Año = año;
                                    ee.Mes = mes;
                                    _ctx.Add(ee);
                                }
                                loopSegPsi++;
                            }
                            await _ctx.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            var fila = loopSegPsi;
                            string msn = ex.ToString();
                            dbTransaction.Rollback();
                            throw new Exception(string.Format("Error al cargar datos en la tabla Seguimiento Psicosocial verificar el número de fila {0}", fila));
                        }

                        //Tabla practicas Pr
                        int loopSegPra = 0;
                        try
                        {
                            DataTable tableSeguimientosPr = result.Tables[4];
                            foreach (DataRow item in tableSeguimientosPr.Rows)
                            {
                                if (loopSegPra != 0)
                                {

                                    CargaSeguimientoPracticasPr ee = new CargaSeguimientoPracticasPr();
                                    int año = Convert.ToInt32(item[0].ToString());
                                    string mes = item[1].ToString();
                                    ee.PId = string.IsNullOrEmpty(item[5].ToString()) ? "" : item[5].ToString();
                                    ee.PpEmpresa = string.IsNullOrEmpty(item[9].ToString()) ? "" : item[9].ToString();
                                    ee.PpCargo = string.IsNullOrEmpty(item[10].ToString()) ? "" : item[10].ToString();
                                    ee.PpDocenteAsign = string.IsNullOrEmpty(item[11].ToString()) ? "" : item[11].ToString();
                                    ee.PpGestion = string.IsNullOrEmpty(item[12].ToString()) ? "" : item[12].ToString();
                                    ee.PpMontoRemuneracion = string.IsNullOrEmpty(item[13].ToString()) ? "" : item[13].ToString();
                                    ee.PpPosibilidadContratacion = string.IsNullOrEmpty(item[14].ToString()) ? "" : item[14].ToString();
                                    ee.IdCarga = carga.IdCarga;
                                    ee.Año = año;
                                    ee.Mes = mes;
                                    _ctx.Add(ee);


                                }
                                loopSegPra++;
                            }
                            await _ctx.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            var fila = loopSegPra;
                            string msg = ex.Message;
                            dbTransaction.Rollback();
                            throw new Exception(string.Format("Error al cargar datos en la tabla Prácticas Profesionales verificar el número de fila {0}", fila));
                        }
                        //Seguimiento Pasantillas
                        int loopsegPasantillas = 0;
                        try
                        {
                            DataTable tableSeguimientPsicosocial = result.Tables[5];
                            foreach (DataRow item in tableSeguimientPsicosocial.Rows)
                            {
                                if (loopsegPasantillas != 0)
                                {
                                    CargaSeguimientoPasantia e = new CargaSeguimientoPasantia();
                                    int año = Convert.ToInt32(item[0].ToString());
                                    string mes = item[1].ToString();
                                    e.PId = item[5].ToString();
                                    e.PasEmpresa = item[9].ToString();
                                    e.PasEntrevista = item[10].ToString();
                                    e.PasPruebas = item[11].ToString();
                                    e.PasContratacion = item[12].ToString();
                                    e.PasCargo = item[13].ToString();
                                    e.PasFechaContratacion = item[14].ToString();
                                    e.PasMontoRemuneracion = item[15].ToString();
                                    e.Año = año;
                                    e.Mes = mes;
                                    _ctx.Add(e);
                                }
                                loopsegPasantillas++;
                            }
                            await _ctx.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            var fila = loopsegPasantillas;
                            string msg = ex.Message;
                            dbTransaction.Rollback();
                            throw new Exception(string.Format("Error al cargar datos en la tabla Seguimiento Pasantillas verificar el número de fila {0}", fila));
                        }

                        //Tabla SeguimientoAutoempleo
                        int loopSegAut = 0;
                        try
                        {
                            DataTable SeguimientoAutoempleo = result.Tables[6];
                            foreach (DataRow item in SeguimientoAutoempleo.Rows)
                            {
                                if (loopSegAut != 0)
                                {

                                    CargaSeguimientoAutoempleo ee = new CargaSeguimientoAutoempleo();
                                    int año = Convert.ToInt32(item[0].ToString());
                                    string mes = item[1].ToString();
                                    ee.PId = string.IsNullOrEmpty(item[5].ToString()) ? "" : item[5].ToString();
                                    ee.AutoempEmpresa = string.IsNullOrEmpty(item[9].ToString()) ? "" : item[9].ToString();
                                    ee.AutoempTipoCapital = string.IsNullOrEmpty(item[10].ToString()) ? "" : item[10].ToString();
                                    ee.AutoempEstado = string.IsNullOrEmpty(item[11].ToString()) ? "" : item[11].ToString();
                                    ee.AutoempTipoFinanciamiento = string.IsNullOrEmpty(item[12].ToString()) ? "" : item[12].ToString();
                                    ee.AutoempTipoEmpresa = string.IsNullOrEmpty(item[13].ToString()) ? "" : item[13].ToString();
                                    ee.AutoempTipoEmpresaOtro = string.IsNullOrEmpty(item[14].ToString()) ? "" : item[14].ToString();
                                    ee.AutoempPlanNegocios = string.IsNullOrEmpty(item[15].ToString()) ? "" : item[15].ToString();
                                    ee.AutoempRegistro = string.IsNullOrEmpty(item[16].ToString()) ? "" : item[16].ToString();
                                    ee.AutoempFechaInicio = string.IsNullOrEmpty(item[17].ToString()) ? null : Convert.ToDateTime(item[17].ToString());
                                    ee.IdCarga = carga.IdCarga;
                                    ee.Año = año;
                                    ee.Mes = mes;
                                    _ctx.Add(ee);

                                }
                                loopSegAut++;

                            }
                            await _ctx.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            var fila = loopSegAut;
                            string msn = ex.Message;
                            dbTransaction.Rollback();
                            throw new Exception(string.Format("Error al cargar datos en la tabla Seguimiento Autoempleo verificar el número de fila {0}", fila));
                        }

                        //Tabla estipendios
                        int loopEst = 0;
                        try
                        {
                            DataTable tableEstipendios = result.Tables[7];
                            foreach (DataRow item in tableEstipendios.Rows)
                            {
                                if (loopEst != 0)
                                {
                                    CargaEstipendio ee = new CargaEstipendio();
                                    int año = Convert.ToInt32(item[0].ToString());
                                    string mes = item[1].ToString();
                                    ee.PId = string.IsNullOrEmpty(item[5].ToString()) ? "" : item[5].ToString();
                                    ee.AlimEfectivo = string.IsNullOrEmpty(item[10].ToString()) ? 0 : Convert.ToDecimal(item[10].ToString());
                                    ee.AlimMontoEfectivo = string.IsNullOrEmpty(item[10].ToString()) ? 0 : Convert.ToDecimal(item[11].ToString());
                                    ee.AlimDiasPresencialesEfectivo = string.IsNullOrEmpty(item[12].ToString()) ? 0 : Convert.ToDecimal(item[12].ToString());
                                    ee.AlimSubtotalEfectivo = string.IsNullOrEmpty(item[13].ToString()) ? 0 : Convert.ToDecimal(item[13].ToString());
                                    ee.AlimTransferencia = string.IsNullOrEmpty(item[14].ToString()) ? 0 : Convert.ToDecimal(item[14].ToString());
                                    ee.AlimMontoTransferencia = string.IsNullOrEmpty(item[15].ToString()) ? 0 : Convert.ToDecimal(item[15].ToString());
                                    ee.AlimDiasPresencialesTransferencia = string.IsNullOrEmpty(item[16].ToString()) ? 0 : Convert.ToDecimal(item[16].ToString());
                                    ee.AlimSubtotalTransferencia = string.IsNullOrEmpty(item[17].ToString()) ? 0 : Convert.ToDecimal(item[17].ToString());
                                    ee.AlimEspecie = string.IsNullOrEmpty(item[18].ToString()) ? 0 : Convert.ToDecimal(item[18].ToString());
                                    ee.AlimMontoEspecie = string.IsNullOrEmpty(item[19].ToString()) ? 0 : Convert.ToDecimal(item[19].ToString());
                                    ee.AlimDiasPresencialesEspecie = string.IsNullOrEmpty(item[20].ToString()) ? 0 : Convert.ToDecimal(item[20].ToString());
                                    ee.AlimSubtotalEspecie = string.IsNullOrEmpty(item[21].ToString()) ? 0 : Convert.ToDecimal(item[21].ToString());
                                    ee.AlimMontoTotal = string.IsNullOrEmpty(item[22].ToString()) ? 0 : Convert.ToDecimal(item[22].ToString());

                                    ee.TranspEfectivo = string.IsNullOrEmpty(item[23].ToString()) ? 0 : Convert.ToDecimal(item[23].ToString());
                                    ee.TranspMontoEfectivo = string.IsNullOrEmpty(item[24].ToString()) ? 0 : Convert.ToDecimal(item[24].ToString());
                                    ee.TranspDiasPresencialesEfectivo = string.IsNullOrEmpty(item[25].ToString()) ? 0 : Convert.ToDecimal(item[25].ToString());
                                    ee.TranspSubtotalEfectivo = string.IsNullOrEmpty(item[26].ToString()) ? 0 : Convert.ToDecimal(item[26].ToString());
                                    ee.TranspTransferencia = string.IsNullOrEmpty(item[27].ToString()) ? 0 : Convert.ToDecimal(item[27].ToString());
                                    ee.TranspTarifaDiferenciada = string.IsNullOrEmpty(item[28].ToString()) ? 0 : Convert.ToDecimal(item[28].ToString());
                                    ee.TranspMontoTransferencia = string.IsNullOrEmpty(item[29].ToString()) ? 0 : Convert.ToDecimal(item[29].ToString());
                                    ee.TranspDiasPresencialesTransferencia = string.IsNullOrEmpty(item[30].ToString()) ? 0 : Convert.ToDecimal(item[30].ToString());
                                    ee.TranspSubtotalTransferencia = string.IsNullOrEmpty(item[31].ToString()) ? 0 : Convert.ToDecimal(item[31].ToString());
                                    ee.TranspMontoTotal = string.IsNullOrEmpty(item[32].ToString()) ? 0 : Convert.ToDecimal(item[32].ToString());

                                    ee.ConecEfectivo = string.IsNullOrEmpty(item[33].ToString()) ? 0 : Convert.ToDecimal(item[33].ToString());
                                    ee.ConecMontoEfectivo = string.IsNullOrEmpty(item[34].ToString()) ? 0 : Convert.ToDecimal(item[34].ToString());
                                    ee.ConecDiasPresencialesEfectivo = string.IsNullOrEmpty(item[35].ToString()) ? 0 : Convert.ToDecimal(item[35].ToString());
                                    ee.ConecSubtotalEfectivo = string.IsNullOrEmpty(item[36].ToString()) ? 0 : Convert.ToDecimal(item[36].ToString());
                                    ee.ConecTransferencia = string.IsNullOrEmpty(item[37].ToString()) ? 0 : Convert.ToDecimal(item[37].ToString());
                                    ee.ConecMontoTransferencia = string.IsNullOrEmpty(item[38].ToString()) ? 0 : Convert.ToDecimal(item[38].ToString());
                                    ee.ConecDiasPresencialesTransferencia = string.IsNullOrEmpty(item[39].ToString()) ? 0 : Convert.ToDecimal(item[39].ToString());
                                    ee.ConecSubtotalTransferencia = string.IsNullOrEmpty(item[40].ToString()) ? 0 : Convert.ToDecimal(item[40].ToString());
                                    ee.ConecMontoTotal = string.IsNullOrEmpty(item[41].ToString()) ? 0 : Convert.ToDecimal(item[41].ToString());
                                    ee.IdCarga = carga.IdCarga;

                                    ee.Año = año;
                                    ee.Mes = mes;
                                    _ctx.Add(ee);
                                }
                                loopEst++;
                            }
                            await _ctx.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            var fila = loopEst + 1;
                            string msg = ex.Message;
                            dbTransaction.Rollback();
                            throw new Exception(string.Format("Error al cargar datos en la tabla Estipendios verificar el número de fila {0}", fila));
                        }

                        dbTransaction.Commit();
                    }
                }
                System.IO.File.Delete(Path.Combine(path, fileName));
            }
            catch (Exception e)
            {
                var msn = e.Message;
                throw;
            }
            return "Datos cargados conexito";

        }


    }

}
