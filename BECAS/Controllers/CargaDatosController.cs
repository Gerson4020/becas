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
            //GUARDAR ARCHIVO EN UNA CARPETA DEL SERVIDOR
            string path = Path.Combine(this.Environment.WebRootPath, "Uploads");

            var SFile = SaveFile(postedFiles, path);

            //EJECUTAR LA CARGA DE DATOS A LA BASE DE DATOS
            var PersonasAsync = LoadPersonasAsync(path, SFile.FileName);
            //var EducacionAsync = LoadEducacionAsync(postedFiles, path, SFile.FileName);
            //var PsicosicialAsync = LoadPsicosicialAsync(postedFiles, path, SFile.FileName);
            //var SegPsicosicialAsync = LoadSegPsicosicialAsync(postedFiles, path, SFile.FileName);
            //var PracticasPrAsync = LoadPracticasPrAsync(postedFiles, path, SFile.FileName);
            //var AutoEmpleoPrAsync = LoadAutoEmpleoPrAsync(postedFiles, path, SFile.FileName);
            //var EstipendiosAsync = LoadEstipendiosAsync(postedFiles, path, SFile.FileName);

            //await Task.WhenAll(PersonasAsync, EducacionAsync, PsicosicialAsync, SegPsicosicialAsync, PracticasPrAsync, AutoEmpleoPrAsync, EstipendiosAsync);
            await Task.WhenAll(PersonasAsync);

            ViewBag.Message += string.Format("<b>{0}</b> uploaded.<br />", PersonasAsync);

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

        public async Task<bool> LoadPersonasAsync(string path, string fileName)
        {
            await Task.Run(async () =>
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
                                //DataTable tableeducacion = result.Tables[1];
                                //DataRow row = tableeducacion.Rows[1];
                                carga.FechaCarga = DateTime.Today;
                                //carga.RAño = Convert.ToInt32(row[0].ToString());
                                //carga.RMes = row[1].ToString();
                                //carga.RFechaini = Convert.ToDateTime(row[2].ToString());
                                //carga.RFechafin = Convert.ToDateTime(row[3].ToString());
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

                                            if (!string.IsNullOrEmpty(item[10].ToString()))
                                            {
                                                var sexo = _catalogos.GetSexo(item[10].ToString());
                                                p.Sexo = sexo != null ? sexo.IdSexo : null;
                                            }

                                            if (!string.IsNullOrEmpty(item[25].ToString()))
                                            {
                                                var programa = _catalogos.GetPrograma(item[25].ToString());
                                                p.Programa = programa != null ? programa.IdPrograma : null;
                                            }

                                            if (!string.IsNullOrEmpty(item[27].ToString()))
                                            {
                                                var sede = _catalogos.GetSede(item[27].ToString());
                                                p.Sede = sede != null ? sede.IdCatSede : null;
                                            }

                                            if (!string.IsNullOrEmpty(item[28].ToString()))
                                            {
                                                var socio = _catalogos.GetSocioImplementador(item[28].ToString());
                                                p.SocioIm = socio != null ? socio.IdImplementador : null;
                                            }

                                            if (!string.IsNullOrEmpty(item[32].ToString()))
                                            {
                                                var carrera = _catalogos.GetCarrera(item[32].ToString());
                                                p.CarreraCursoGrado = carrera != null ? carrera.IdCatCarrera : null;
                                            }

                                            if (!string.IsNullOrEmpty(item[24].ToString()))
                                            {
                                                //p.Departamento = string.IsNullOrEmpty(item[24].ToString()) ? "" : item[24].ToString();
                                                var departamento = _catalogos.GetDepartamento(item[24].ToString());
                                                p.Departamento = departamento != null ? departamento.IdDepartamento : null;
                                            }

                                            if (!string.IsNullOrEmpty(item[23].ToString()))
                                            {
                                                //p.Refiere = string.IsNullOrEmpty(item[23].ToString()) ? "" : item[23].ToString();
                                                var refiere = _catalogos.GetReferencias(item[23].ToString());
                                                p.Refiere = refiere != null ? refiere.IdRefiere : null;
                                            }

                                            if (!string.IsNullOrEmpty(item[29].ToString()))
                                            {
                                                //p.Zona = string.IsNullOrEmpty(item[29].ToString()) ? "" : item[29].ToString();
                                                var zona = _catalogos.GetZona(item[29].ToString());
                                                p.Zona = zona != null ? zona.IdZona : null;
                                            }

                                            if (!string.IsNullOrEmpty(item[26].ToString()))
                                            {
                                                //p.Cohorte = string.IsNullOrEmpty(item[26].ToString()) ? "" : item[26].ToString();
                                                var cohorte = _catalogos.GetCohorte(item[26].ToString());
                                                p.Cohorte = cohorte != null ? cohorte.IdCohorte : null;
                                            }

                                            p.FechaEntrevista = string.IsNullOrEmpty(item[0].ToString()) ? null : Convert.ToDateTime(item[0].ToString());
                                            p.Nombre = string.IsNullOrEmpty(item[4].ToString()) ? "" : item[4].ToString();
                                            p.Apellido = string.IsNullOrEmpty(item[5].ToString()) ? "" : item[5].ToString();
                                            p.NombreCompleto = string.IsNullOrEmpty(item[6].ToString()) ? "" : item[6].ToString();
                                            p.UltimoGradoAprobado = string.IsNullOrEmpty(item[7].ToString()) ? "" : item[7].ToString();
                                            p.Telefono1 = string.IsNullOrEmpty(item[8].ToString()) ? "" : item[8].ToString();
                                            p.Telefono2 = string.IsNullOrEmpty(item[9].ToString()) ? "" : item[9].ToString();
                                            p.FechaNacimiento = string.IsNullOrEmpty(item[11].ToString()) ? null : Convert.ToDateTime(item[11].ToString());
                                            p.Edad = string.IsNullOrEmpty(item[12].ToString()) ? 0 : Convert.ToInt32(item[12].ToString());
                                            p.Discapacidad = string.IsNullOrEmpty(item[13].ToString()) ? "" : item[13].ToString();
                                            p.VictimaViolencia = string.IsNullOrEmpty(item[14].ToString()) ? "" : item[14].ToString();
                                            p.MigranteRetornado = string.IsNullOrEmpty(item[15].ToString()) ? "" : item[15].ToString();
                                            p.PiensaMigrar = string.IsNullOrEmpty(item[16].ToString()) ? "" : item[16].ToString();
                                            p.FamiliaresMigrantes = string.IsNullOrEmpty(item[17].ToString()) ? "" : item[17].ToString();
                                            p.FamiliaresRetornados = string.IsNullOrEmpty(item[18].ToString()) ? "" : item[18].ToString();
                                            p.Empleo = string.IsNullOrEmpty(item[19].ToString()) ? "" : item[19].ToString();
                                            p.Dui = string.IsNullOrEmpty(item[20].ToString()) ? "" : item[20].ToString();
                                            p.Nie = string.IsNullOrEmpty(item[21].ToString()) ? "" : item[21].ToString();
                                            p.Correo = string.IsNullOrEmpty(item[22].ToString()) ? "" : item[22].ToString();
                                            //p.Cohorte = string.IsNullOrEmpty(item[26].ToString()) ? "" : item[26].ToString();
                                            p.EstadoInscripcion = string.IsNullOrEmpty(item[30].ToString()) ? "" : item[30].ToString();
                                            p.EstadoMf = string.IsNullOrEmpty(item[31].ToString()) ? "" : item[31].ToString();
                                            p.CartaCompromiso = string.IsNullOrEmpty(item[33].ToString()) ? "" : item[33].ToString();
                                            _ctx.Entry(p).State = EntityState.Modified;
                                            await _ctx.SaveChangesAsync();
                                        }
                                        else
                                        {

                                            Persona persona = new Persona();

                                            if (!string.IsNullOrEmpty(item[3].ToString()))
                                            {
                                                var matricula = _catalogos.GetTipoMatricula(item[3].ToString());
                                                persona.TipoMatricula = matricula != null ? matricula.IdTipoMatricula : null;
                                            }

                                            if (!string.IsNullOrEmpty(item[10].ToString()))
                                            {
                                                var sexo = _catalogos.GetSexo(item[10].ToString());
                                                persona.Sexo = sexo != null ? sexo.IdSexo : null;
                                            }

                                            if (!string.IsNullOrEmpty(item[25].ToString()))
                                            {
                                                var programa = _catalogos.GetPrograma(item[25].ToString());
                                                persona.Programa = programa != null ? programa.IdPrograma : null;
                                            }

                                            if (!string.IsNullOrEmpty(item[27].ToString()))
                                            {
                                                var sede = _catalogos.GetSede(item[27].ToString());
                                                persona.Sede = sede != null ? sede.IdCatSede : null;
                                            }

                                            if (!string.IsNullOrEmpty(item[28].ToString()))
                                            {
                                                var socio = _catalogos.GetSocioImplementador(item[28].ToString());
                                                persona.SocioIm = socio != null ? socio.IdImplementador : null;
                                            }

                                            if (string.IsNullOrEmpty(item[32].ToString()))
                                            {
                                                var carrera = _catalogos.GetCarrera(item[32].ToString());
                                                persona.CarreraCursoGrado = carrera != null ? carrera.IdCatCarrera : null;
                                            }

                                            if (!string.IsNullOrEmpty(item[24].ToString()))
                                            {
                                                //p.Departamento = string.IsNullOrEmpty(item[24].ToString()) ? "" : item[24].ToString();
                                                var departamento = _catalogos.GetDepartamento(item[24].ToString());
                                                persona.Departamento = departamento != null ? departamento.IdDepartamento : null;
                                            }

                                            if (!string.IsNullOrEmpty(item[23].ToString()))
                                            {
                                                //p.Refiere = string.IsNullOrEmpty(item[23].ToString()) ? "" : item[23].ToString();
                                                var refiere = _catalogos.GetReferencias(item[23].ToString());
                                                persona.Refiere = refiere != null ? refiere.IdRefiere : null;
                                            }

                                            if (!string.IsNullOrEmpty(item[29].ToString()))
                                            {
                                                //p.Zona = string.IsNullOrEmpty(item[29].ToString()) ? "" : item[29].ToString();
                                                var zona = _catalogos.GetZona(item[29].ToString());
                                                persona.Zona = zona != null ? zona.IdZona : null;
                                            }

                                            if (!string.IsNullOrEmpty(item[26].ToString()))
                                            {
                                                //p.Cohorte = string.IsNullOrEmpty(item[26].ToString()) ? "" : item[26].ToString();
                                                var cohorte = _catalogos.GetCohorte(item[26].ToString());
                                                persona.Cohorte = cohorte != null ? cohorte.IdCohorte : null;
                                            }

                                            persona.FechaEntrevista = string.IsNullOrEmpty(item[0].ToString()) ? null : Convert.ToDateTime(item[0].ToString());
                                            persona.Id = string.IsNullOrEmpty(item[1].ToString()) ? "" : item[1].ToString();
                                            persona.PIdOim = string.IsNullOrEmpty(item[2].ToString()) ? "" : item[2].ToString();
                                            persona.Nombre = string.IsNullOrEmpty(item[4].ToString()) ? "" : item[4].ToString();
                                            persona.Apellido = string.IsNullOrEmpty(item[5].ToString()) ? "" : item[5].ToString();
                                            persona.NombreCompleto = string.IsNullOrEmpty(item[6].ToString()) ? "" : item[6].ToString();
                                            persona.UltimoGradoAprobado = string.IsNullOrEmpty(item[7].ToString()) ? "" : item[7].ToString();
                                            persona.Telefono1 = string.IsNullOrEmpty(item[8].ToString()) ? "" : item[8].ToString();
                                            persona.Telefono2 = string.IsNullOrEmpty(item[9].ToString()) ? "" : item[9].ToString();
                                            persona.FechaNacimiento = string.IsNullOrEmpty(item[11].ToString()) ? null : Convert.ToDateTime(item[11].ToString());
                                            persona.Edad = string.IsNullOrEmpty(item[12].ToString()) ? 0 : Convert.ToInt32(item[12].ToString());
                                            persona.Discapacidad = string.IsNullOrEmpty(item[13].ToString()) ? "" : item[13].ToString();
                                            persona.VictimaViolencia = string.IsNullOrEmpty(item[14].ToString()) ? "" : item[14].ToString();
                                            persona.MigranteRetornado = string.IsNullOrEmpty(item[15].ToString()) ? "" : item[15].ToString();
                                            persona.PiensaMigrar = string.IsNullOrEmpty(item[16].ToString()) ? "" : item[16].ToString();
                                            persona.FamiliaresMigrantes = string.IsNullOrEmpty(item[17].ToString()) ? "" : item[17].ToString();
                                            persona.FamiliaresRetornados = string.IsNullOrEmpty(item[18].ToString()) ? "" : item[18].ToString();
                                            persona.Empleo = string.IsNullOrEmpty(item[19].ToString()) ? "" : item[19].ToString();
                                            persona.Dui = string.IsNullOrEmpty(item[20].ToString()) ? "" : item[20].ToString();
                                            persona.Nie = string.IsNullOrEmpty(item[21].ToString()) ? "" : item[21].ToString();
                                            persona.Correo = string.IsNullOrEmpty(item[22].ToString()) ? "" : item[22].ToString();
                                            //persona.Cohorte = string.IsNullOrEmpty(item[26].ToString()) ? "" : item[26].ToString();
                                            persona.EstadoInscripcion = string.IsNullOrEmpty(item[30].ToString()) ? "" : item[30].ToString();
                                            persona.EstadoMf = string.IsNullOrEmpty(item[31].ToString()) ? "" : item[31].ToString();
                                            persona.CartaCompromiso = string.IsNullOrEmpty(item[33].ToString()) ? "" : item[33].ToString();
                                            persona.IdCarga = carga.IdCarga;

                                            _ctx.Entry(persona).State = EntityState.Added;
                                            await _ctx.SaveChangesAsync();
                                        }

                                    }
                                    loopPer++;
                                }
                            }
                            catch (IndexOutOfRangeException ex)
                            {
                                var linea = loopPer;
                                string msn = ex.Message;
                                dbTransaction.Rollback();
                                throw;
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
                                        await _ctx.SaveChangesAsync();
                                    }
                                    loopEd++;
                                }
                            }
                            catch (Exception ex)
                            {
                                var fila = loopEd;
                                string smn = ex.Message;
                                dbTransaction.Rollback();
                                throw;
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
                                        await _ctx.SaveChangesAsync();
                                    }
                                    loopPsi++;
                                }
                            }
                            catch (Exception ex)
                            {
                                var fila = loopPsi;
                                string msn = ex.Message;
                                dbTransaction.Rollback();
                                throw;
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
                                        await _ctx.SaveChangesAsync();


                                    }
                                    loopSegPsi++;

                                }
                            }
                            catch (Exception ex)
                            {
                                var linea = loopSegPsi;
                                string msn = ex.ToString();
                                dbTransaction.Rollback();
                                throw;
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
                                        await _ctx.SaveChangesAsync();

                                    }
                                    loopSegPra++;
                                }
                            }
                            catch (Exception ex)
                            {
                                var linea = loopSegPra;
                                string msg = ex.Message;
                                dbTransaction.Rollback();
                                throw;
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
                                        await _ctx.SaveChangesAsync();
                                    }
                                    loopsegPasantillas++;
                                }
                            }
                            catch (Exception ex)
                            {
                                var linea = loopsegPasantillas;
                                string msg = ex.Message;
                                dbTransaction.Rollback();
                                throw;
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
                                        await _ctx.SaveChangesAsync();


                                    }
                                    loopSegAut++;

                                }
                            }
                            catch (Exception ex)
                            {
                                var linea = loopSegAut;
                                string msn = ex.Message;
                                dbTransaction.Rollback();
                                throw;
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
                                        await _ctx.SaveChangesAsync();
                                    }
                                    loopEst++;
                                }
                            }
                            catch (Exception ex)
                            {
                                var linea = loopEst;
                                string msg = ex.Message;
                                dbTransaction.Rollback();
                                throw;
                            }
                            //_ctx.SaveChanges();
                            dbTransaction.Commit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                }
            });
            return true;
        }

        public static async Task<bool> LoadEducacionAsync(List<IFormFile> postedFiles, string path, string fileName)
        {
            List<string> uploadedFiles = new List<string>();
            await Task.Run(() =>
            {
                try
                {
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    using (var stream = System.IO.File.Open(Path.Combine(path, fileName), FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            CargaEducacion e = new CargaEducacion();

                            var result = reader.AsDataSet();
                            DataTable table = result.Tables[1];
                            foreach (DataRow item in table.Rows)
                            {
                                e.PIdOim = item[5].ToString();
                                e.DFechaReasg = Convert.ToDateTime(item[10].ToString());
                                e.DEstado = item[11].ToString();
                                e.DFechades = Convert.ToDateTime(item[12].ToString());
                                e.DMotivodesercion = item[13].ToString();
                                e.IDiasAsistenciaEstablecidos = Convert.ToInt32(item[14].ToString());
                                e.IDiasAsistenciaEfectivos = Convert.ToInt32(item[15].ToString());
                                e.IMotivoInasistencia = item[16].ToString();
                                e.IModulosInscritos = Convert.ToInt32(item[17].ToString());
                                e.IModulosAprobados = Convert.ToInt32(item[18].ToString());
                                e.IModulosReprobados = Convert.ToInt32(item[19].ToString());
                                e.ICausaReprobacion = item[20].ToString();

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                }
            });
            return true;
        }

        public static async Task<bool> LoadPsicosicialAsync(List<IFormFile> postedFiles, string path, string fileName)
        {
            List<string> uploadedFiles = new List<string>();
            await Task.Run(() =>
            {
                try
                {
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    using (var stream = System.IO.File.Open(Path.Combine(path, fileName), FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            CargaEvaluacionPsicosocial e = new CargaEvaluacionPsicosocial();

                            var result = reader.AsDataSet();
                            DataTable table = result.Tables[2];
                            foreach (DataRow item in table.Rows)
                            {
                                e.PId = item[5].ToString();
                                e.OvParticipacion = item[9].ToString().Equals("Si") ? true : false;
                                e.OvPuntajePret = item[10].ToString();
                                e.OvPuntajePos = item[11].ToString();
                                e.EpInstrumentoRiesgo = item[12].ToString();
                                e.EpVulnerabilidades = item[13].ToString();
                                e.EpAlertaDesercion = item[14].ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                }
            });
            return true;
        }

        public static async Task<bool> LoadSegPsicosicialAsync(List<IFormFile> postedFiles, string path, string fileName)
        {
            List<string> uploadedFiles = new List<string>();
            await Task.Run(() =>
            {
                try
                {
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    using (var stream = System.IO.File.Open(Path.Combine(path, fileName), FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            CargaSeguimientoPsicosocial e = new CargaSeguimientoPsicosocial();

                            var result = reader.AsDataSet();
                            DataTable table = result.Tables[3];
                            foreach (DataRow item in table.Rows)
                            {
                                e.PId = item[5].ToString();
                                e.SegMotivo = item[9].ToString();
                                e.SegEstado = item[10].ToString();
                                e.SegMedida = item[11].ToString();
                                e.SegAlertaDesercion = item[12].ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                }
            });
            return true;
        }

        public static async Task<bool> LoadPracticasPrAsync(List<IFormFile> postedFiles, string path, string fileName)
        {
            List<string> uploadedFiles = new List<string>();
            await Task.Run(() =>
            {
                try
                {
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    using (var stream = System.IO.File.Open(Path.Combine(path, fileName), FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            CargaSeguimientoPracticasPr e = new CargaSeguimientoPracticasPr();

                            var result = reader.AsDataSet();
                            DataTable table = result.Tables[3];
                            foreach (DataRow item in table.Rows)
                            {
                                e.PId = item[5].ToString();
                                e.PpEmpresa = item[9].ToString();
                                e.PpCargo = item[10].ToString();
                                e.PpDocenteAsign = item[11].ToString();
                                e.PpGestion = item[12].ToString();
                                e.PpMontoRemuneracion = item[12].ToString();
                                e.PpPosibilidadContratacion = item[12].ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                }
            });
            return true;
        }

        public static async Task<bool> LoadSegPracticasPrAsync(List<IFormFile> postedFiles, string path, string fileName)
        {
            List<string> uploadedFiles = new List<string>();
            await Task.Run(() =>
            {
                try
                {
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    using (var stream = System.IO.File.Open(Path.Combine(path, fileName), FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            CargaSeguimientoPasantia e = new CargaSeguimientoPasantia();

                            var result = reader.AsDataSet();
                            DataTable table = result.Tables[3];
                            foreach (DataRow item in table.Rows)
                            {
                                e.PId = item[5].ToString();
                                e.PasEmpresa = item[9].ToString();
                                e.PasEntrevista = item[10].ToString();
                                e.PasPruebas = item[11].ToString();
                                e.PasContratacion = item[12].ToString();
                                e.PasCargo = item[12].ToString();
                                e.PasFechaContratacion = item[12].ToString();
                                e.PasMontoRemuneracion = item[13].ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                }
            });
            return true;
        }

        public static async Task<bool> LoadAutoEmpleoPrAsync(List<IFormFile> postedFiles, string path, string fileName)
        {
            List<string> uploadedFiles = new List<string>();
            await Task.Run(() =>
            {
                try
                {
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    using (var stream = System.IO.File.Open(Path.Combine(path, fileName), FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            CargaSeguimientoAutoempleo e = new CargaSeguimientoAutoempleo();

                            var result = reader.AsDataSet();
                            DataTable table = result.Tables[3];
                            foreach (DataRow item in table.Rows)
                            {
                                e.PId = item[5].ToString();
                                e.AutoempEmpresa = item[9].ToString();
                                e.AutoempTipoCapital = item[10].ToString();
                                e.AutoempEstado = item[11].ToString();
                                e.AutoempTipoFinanciamiento = item[12].ToString();
                                e.AutoempTipoEmpresa = item[12].ToString();
                                e.AutoempTipoEmpresaOtro = item[12].ToString();
                                e.AutoempPlanNegocios = item[13].ToString();
                                e.AutoempRegistro = item[13].ToString();
                                e.AutoempFechaInicio = Convert.ToDateTime(item[13].ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                }
            });
            return true;
        }

        public static async Task<bool> LoadEstipendiosAsync(List<IFormFile> postedFiles, string path, string fileName)
        {
            List<string> uploadedFiles = new List<string>();
            await Task.Run(() =>
            {
                try
                {
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    using (var stream = System.IO.File.Open(Path.Combine(path, fileName), FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            CargaEstipendio e = new CargaEstipendio();

                            var result = reader.AsDataSet();
                            DataTable table = result.Tables[3];
                            foreach (DataRow item in table.Rows)
                            {
                                e.PId = item[5].ToString();
                                e.AlimEfectivo = Convert.ToDecimal(item[10].ToString());
                                e.AlimMontoEfectivo = Convert.ToDecimal(item[11].ToString());
                                e.AlimDiasPresencialesEfectivo = Convert.ToDecimal(item[12].ToString());
                                e.AlimSubtotalEfectivo = Convert.ToDecimal(item[13].ToString());
                                e.AlimTransferencia = Convert.ToDecimal(item[14].ToString());
                                e.AlimMontoTransferencia = Convert.ToDecimal(item[15].ToString());
                                e.AlimDiasPresencialesTransferencia = Convert.ToDecimal(item[16].ToString());
                                e.AlimSubtotalTransferencia = Convert.ToDecimal(item[17].ToString());
                                e.AlimEspecie = Convert.ToDecimal(item[18].ToString());
                                e.AlimMontoEspecie = Convert.ToDecimal(item[19].ToString());
                                e.AlimDiasPresencialesEspecie = Convert.ToDecimal(item[20].ToString());
                                e.AlimSubtotalEspecie = Convert.ToDecimal(item[21].ToString());
                                e.AlimMontoTotal = Convert.ToDecimal(item[22].ToString());

                                e.TranspEfectivo = Convert.ToDecimal(item[23].ToString());
                                e.TranspMontoEfectivo = Convert.ToDecimal(item[24].ToString());
                                e.TranspDiasPresencialesEfectivo = Convert.ToDecimal(item[25].ToString());
                                e.TranspSubtotalEfectivo = Convert.ToDecimal(item[26].ToString());
                                e.TranspTransferencia = Convert.ToDecimal(item[27].ToString());
                                e.TranspTarifaDiferenciada = Convert.ToDecimal(item[28].ToString());
                                e.TranspMontoTransferencia = Convert.ToDecimal(item[29].ToString());
                                e.TranspDiasPresencialesTransferencia = Convert.ToDecimal(item[30].ToString());
                                e.TranspSubtotalTransferencia = Convert.ToDecimal(item[31].ToString());
                                e.TranspMontoTotal = Convert.ToDecimal(item[32].ToString());

                                e.ConecEfectivo = Convert.ToDecimal(item[33].ToString());
                                e.ConecMontoEfectivo = Convert.ToDecimal(item[34].ToString());
                                e.ConecDiasPresencialesEfectivo = Convert.ToDecimal(item[35].ToString());
                                e.ConecSubtotalEfectivo = Convert.ToDecimal(item[36].ToString());
                                e.ConecTransferencia = Convert.ToDecimal(item[37].ToString());
                                e.ConecMontoTransferencia = Convert.ToDecimal(item[38].ToString());
                                e.ConecDiasPresencialesTransferencia = Convert.ToDecimal(item[39].ToString());
                                e.ConecSubtotalTransferencia = Convert.ToDecimal(item[40].ToString());
                                e.ConecMontoTotal = Convert.ToDecimal(item[41].ToString());

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                }
            });
            return true;
        }

    }
}
