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
        private readonly IWebHostEnvironment _env;

        public CargaDatosController(IHostingEnvironment _environment, MEOBContext ctx, ICatalogos catalogos, IWebHostEnvironment env)
        {
            Environment = _environment;
            _ctx = ctx;
            _catalogos = catalogos;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(List<IFormFile> postedFiles, int Anexo)
        {
            try
            {

                //GUARDAR ARCHIVO EN UNA CARPETA DEL SERVIDOR
                string path = Path.Combine(this.Environment.WebRootPath, "Uploads");
                var SFile = SaveFile(postedFiles, path);

                if (Anexo == 1)
                {
                    var PersonasAsync = LoadPersonasAsync(path, SFile.FileName);
                    await Task.WhenAll(PersonasAsync);
                    ViewBag.Message += string.Format("{0}", PersonasAsync.Result);
                }
                if (Anexo == 2)
                {
                    var EducacionAsync = LoadEducacionAsync(path, SFile.FileName);
                    await Task.WhenAll(EducacionAsync);
                    ViewBag.Message += string.Format("{0}", EducacionAsync.Result);
                }
                if (Anexo == 3)
                {
                    var PsocosocialAsync = LoadPsocosocialAsync(path, SFile.FileName);
                    await Task.WhenAll(PsocosocialAsync);
                    ViewBag.Message += string.Format("{0}", PsocosocialAsync.Result);
                }
                if (Anexo == 4)
                {
                    var SeguimientoPsicosocialAsync = LoadSeguimientoPsicosocialAsync(path, SFile.FileName);
                    await Task.WhenAll(SeguimientoPsicosocialAsync);
                    ViewBag.Message += string.Format("{0}", SeguimientoPsicosocialAsync.Result);
                }
                if (Anexo == 5)
                {
                    var PracticasAsync = LoadPracticasAsync(path, SFile.FileName);
                    await Task.WhenAll(PracticasAsync);
                    ViewBag.Message += string.Format("{0}", PracticasAsync.Result);
                }
                if (Anexo == 6)
                {
                    var PasantillasAsync = LoadPasantillasAsync(path, SFile.FileName);
                    await Task.WhenAll(PasantillasAsync);
                    ViewBag.Message += string.Format("{0}", PasantillasAsync.Result);
                }
                if (Anexo == 7)
                {
                    var AutoempleoAsync = LoadAutoempleoAsync(path, SFile.FileName);
                    await Task.WhenAll(AutoempleoAsync);
                    ViewBag.Message += string.Format("{0}", AutoempleoAsync.Result);
                }
                if (Anexo == 8)
                {
                    var EstipendiosAsync = LoadEstipendiosAsync(path, SFile.FileName);
                    await Task.WhenAll(EstipendiosAsync);
                    ViewBag.Message += string.Format("{0}", EstipendiosAsync.Result);
                }
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

        public FileResult Plantillas(int anexo)
        {
            if (anexo == 1)
            {
                return File("/Plantillas/Inscripciones.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            }
            if (anexo == 2)
            {
                //var filePath = Path.Combine(_env.ContentRootPath, "Plantillas", "Evaluacion_Psicosocial.xlsx");
                return File("/Plantillas/Evaluacion_Psicosocial.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            if (anexo == 3)
            {
                //var filePath = Path.Combine(_env.ContentRootPath, "Plantillas", "Seguimiento_Psicosocial.xlsx");
                return File("/Plantillas/Seguimiento_Psicosocial.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            if (anexo == 4)
            {
                //var filePath = Path.Combine(_env.ContentRootPath, "Plantillas", "Educacion.xlsx");
                return File("/Plantillas/Educacion.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            if (anexo == 5)
            {
                //var filePath = Path.Combine(_env.ContentRootPath, "Plantillas", "Seguimiento_en_Pasantías.xlsx");
                return File("/Plantillas/Seguimiento_en_Pasantías.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            if (anexo == 6)
            {
                //var filePath = Path.Combine(_env.ContentRootPath, "Plantillas", "Seguimiento_en_Prácticas_Pr.xlsx");
                return File("/Plantillas/Seguimiento_en_Prácticas_Pr.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            if (anexo == 7)
            {
                //var filePath = Path.Combine(_env.ContentRootPath, "Plantillas", "Seguimiento_en_Autoempleo.xlsx");
                return File("/Plantillas/Seguimiento_en_Autoempleo.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            if (anexo == 8)
            {
                //var filePath = Path.Combine(_env.ContentRootPath, "Plantillas", "Estipendios.xlsx");
                return File("/Plantillas/Estipendios.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            var fileDefaul = Path.Combine(_env.ContentRootPath, "Plantillas", "Inscripciones.xlsx");
            return PhysicalFile(fileDefaul, "application/vnd.ms-excel");
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

                            carga.TotalInscripcion = tablepersonas.Rows.Count;

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
                        int columnaPer = 1;
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
                                        p.Nombre = string.IsNullOrEmpty(item[4].ToString()) ? string.Empty : item[4].ToString();
                                        columnaPer++;
                                        p.Apellido = string.IsNullOrEmpty(item[5].ToString()) ? string.Empty : item[5].ToString();
                                        columnaPer++;
                                        p.NombreCompleto = string.IsNullOrEmpty(item[6].ToString()) ? string.Empty : item[6].ToString();
                                        columnaPer++;
                                        p.UltimoGradoAprobado = string.IsNullOrEmpty(item[7].ToString()) ? string.Empty : item[7].ToString();
                                        columnaPer++;
                                        p.NivelAcademico = string.IsNullOrEmpty(item[8].ToString()) ? string.Empty : item[8].ToString();
                                        columnaPer++;
                                        p.Telefono1 = string.IsNullOrEmpty(item[9].ToString()) ? string.Empty : item[9].ToString();
                                        columnaPer++;
                                        p.Telefono2 = string.IsNullOrEmpty(item[10].ToString()) ? string.Empty : item[10].ToString();
                                        columnaPer++;
                                        if (!string.IsNullOrEmpty(item[11].ToString()))
                                        {
                                            var sexo = _catalogos.GetSexo(item[11].ToString());
                                            p.Sexo = sexo != null ? sexo.IdSexo : null;
                                        }
                                        columnaPer++;
                                        if (!string.IsNullOrEmpty(item[12].ToString()))
                                        {
                                            p.LGBTIQ = item[12].ToString() == "Sí" ? true : false;
                                        }
                                        columnaPer++;
                                        p.FechaNacimiento = string.IsNullOrEmpty(item[13].ToString()) ? null : Convert.ToDateTime(item[13].ToString());
                                        columnaPer++;
                                        p.Edad = string.IsNullOrEmpty(item[14].ToString()) ? 0 : Convert.ToInt32(item[14].ToString().Trim());
                                        columnaPer++;
                                        p.Discapacidad = string.IsNullOrEmpty(item[15].ToString()) ? string.Empty : item[15].ToString();
                                        columnaPer++;
                                        p.VictimaViolencia = string.IsNullOrEmpty(item[16].ToString()) ? string.Empty : item[16].ToString();
                                        columnaPer++;
                                        //p.MigranteRetornado = string.IsNullOrEmpty(item[17].ToString()) ? string.Empty : item[17].ToString();
                                        if (!string.IsNullOrEmpty(item[17].ToString()))
                                        {
                                            p.MigranteRetornado = item[17].ToString().Equals("Sí") ? 1 : 2;
                                        }
                                        columnaPer++;
                                        p.PiensaMigrar = string.IsNullOrEmpty(item[18].ToString()) ? string.Empty : item[18].ToString();
                                        columnaPer++;
                                        p.FamiliaresMigrantes = string.IsNullOrEmpty(item[19].ToString()) ? string.Empty : item[19].ToString();
                                        columnaPer++;
                                        p.FamiliaresRetornados = string.IsNullOrEmpty(item[20].ToString()) ? string.Empty : item[20].ToString();
                                        columnaPer++;
                                        p.Empleo = string.IsNullOrEmpty(item[21].ToString()) ? string.Empty : item[21].ToString();
                                        columnaPer++;
                                        p.Dui = string.IsNullOrEmpty(item[22].ToString()) ? string.Empty : item[22].ToString();
                                        columnaPer++;
                                        p.Nie = string.IsNullOrEmpty(item[23].ToString()) ? string.Empty : item[23].ToString();
                                        columnaPer++;
                                        p.Correo = string.IsNullOrEmpty(item[24].ToString()) ? string.Empty : item[24].ToString();
                                        columnaPer++;
                                        if (!string.IsNullOrEmpty(item[25].ToString()))
                                        {
                                            var refiere = _catalogos.GetReferencias(item[25].ToString());
                                            p.Refiere = refiere != null ? refiere.IdRefiere : null;
                                        }
                                        columnaPer++;
                                        if (!string.IsNullOrEmpty(item[26].ToString()))
                                        {
                                            var departamento = _catalogos.GetDepartamento(item[26].ToString());
                                            p.Departamento = departamento != null ? departamento.IdDepartamento : null;
                                        }
                                        columnaPer++;
                                        if (!string.IsNullOrEmpty(item[27].ToString()))
                                        {
                                            var municipio = _catalogos.GetMunicipio(item[27].ToString());
                                            p.Municipio = municipio != null ? municipio.IdMunicipio : null;
                                        }
                                        columnaPer++;
                                        //programa = item 28
                                        if (!string.IsNullOrEmpty(item[28].ToString()))
                                        {
                                            var tipomatricula = _catalogos.GetTipoMatricula(item[28].ToString());
                                            p.IdTipoMatricula = tipomatricula != null ? tipomatricula.IdTipoMatricula : 0;

                                        }
                                        columnaPer++;
                                        if (!string.IsNullOrEmpty(item[29].ToString()))
                                        {
                                            var programa = _catalogos.GetPrograma(item[29].ToString());
                                            p.IdPrograma = programa != null ? programa.IdPrograma : 0;

                                        }
                                        columnaPer++;
                                        //AÑO = item 29
                                        p.Year = string.IsNullOrEmpty(item[30].ToString()) ? 0 : int.Parse(item[30].ToString());
                                        //Cohorte 30
                                        if (!string.IsNullOrEmpty(item[31].ToString()))
                                        {
                                            var cohorte = _catalogos.GetCohorte(item[31].ToString());
                                            p.Cohorte = cohorte != null ? cohorte.IdCohorte : 0;

                                        }
                                        columnaPer++;
                                        //Sede 31
                                        if (!string.IsNullOrEmpty(item[32].ToString()))
                                        {
                                            var Catsede = _catalogos.GetSede(item[32].ToString());
                                            if (Catsede != null)
                                            {
                                                var sede = _ctx.Sedes.FirstOrDefault(x => x.IdCatSede.Equals(Catsede.IdCatSede));
                                                p.p_socio = sede.IdSocio;
                                                p.IdZona = sede.IdZona;
                                            }
                                            p.p_sede = Catsede != null ? Catsede.IdCatSede : 0;

                                        }
                                        columnaPer++;

                                        if (!string.IsNullOrEmpty(item[33].ToString()))
                                        {
                                            var carrera = _catalogos.GetCarrera(item[33].ToString(), (int)p.IdPrograma);
                                            p.CarreraCursoGrado = carrera != null ? carrera.IdCatCarrera : 0;
                                        }
                                        columnaPer++;

                                        if (!string.IsNullOrEmpty(item[34].ToString()))
                                        {
                                            var sector = _catalogos.GetSector(item[34].ToString());
                                            p.Sector = sector != null ? sector.IdSector : 0;
                                        }
                                        columnaPer++;
                                        p.MedioVerificacion = string.IsNullOrEmpty(item[35].ToString()) ? "" : item[35].ToString();
                                        columnaPer++;
                                        if (!string.IsNullOrEmpty(item[36].ToString()))
                                        {
                                            var proyecto = _catalogos.GetProyectos(item[36].ToString());
                                            p.IdProyecto = proyecto != null ? proyecto.IdProyecto : 0;
                                        }
                                        columnaPer++;
                                        _ctx.Entry(p).State = EntityState.Modified;
                                        columnaPer = 1;
                                    }
                                    else
                                    {
                                        Persona persona = new Persona();
                                        persona.FechaEntrevista = string.IsNullOrEmpty(item[0].ToString()) ? null : DateTime.Parse(item[0].ToString());
                                        columnaPer++;
                                        persona.Id = string.IsNullOrEmpty(item[1].ToString()) ? string.Empty : item[1].ToString();
                                        columnaPer++;
                                        persona.NumeroInscripciones = string.IsNullOrEmpty(item[3].ToString()) ? 0 : int.Parse(item[3].ToString());
                                        columnaPer++;
                                        persona.PIdOim = string.IsNullOrEmpty(item[2].ToString()) ? string.Empty : item[2].ToString();
                                        columnaPer++;
                                        persona.Nombre = string.IsNullOrEmpty(item[4].ToString()) ? string.Empty : item[4].ToString();
                                        columnaPer++;
                                        persona.Apellido = string.IsNullOrEmpty(item[5].ToString()) ? string.Empty : item[5].ToString();
                                        columnaPer++;
                                        persona.NombreCompleto = string.IsNullOrEmpty(item[6].ToString()) ? string.Empty : item[6].ToString();
                                        columnaPer++;
                                        persona.UltimoGradoAprobado = string.IsNullOrEmpty(item[7].ToString()) ? string.Empty : item[7].ToString();
                                        columnaPer++;
                                        persona.NivelAcademico = string.IsNullOrEmpty(item[8].ToString()) ? string.Empty : item[8].ToString();
                                        columnaPer++;
                                        persona.Telefono1 = string.IsNullOrEmpty(item[9].ToString()) ? string.Empty : item[9].ToString();
                                        columnaPer++;
                                        persona.Telefono2 = string.IsNullOrEmpty(item[10].ToString()) ? string.Empty : item[10].ToString();
                                        columnaPer++;
                                        if (!string.IsNullOrEmpty(item[11].ToString()))
                                        {
                                            var sexo = _catalogos.GetSexo(item[11].ToString());
                                            persona.Sexo = sexo != null ? sexo.IdSexo : null;
                                        }
                                        columnaPer++;
                                        if (!string.IsNullOrEmpty(item[12].ToString()))
                                        {
                                            persona.LGBTIQ = item[12].ToString() == "Sí" ? true : false;
                                        }
                                        columnaPer++;
                                        persona.FechaNacimiento = string.IsNullOrEmpty(item[13].ToString()) ? null : Convert.ToDateTime(item[13].ToString());
                                        columnaPer++;
                                        persona.Edad = string.IsNullOrEmpty(item[14].ToString()) ? 0 : Convert.ToInt32(item[14].ToString().Trim());
                                        columnaPer++;
                                        persona.Discapacidad = string.IsNullOrEmpty(item[15].ToString()) ? string.Empty : item[15].ToString();
                                        columnaPer++;
                                        persona.VictimaViolencia = string.IsNullOrEmpty(item[16].ToString()) ? string.Empty : item[16].ToString();
                                        columnaPer++;
                                        //p.MigranteRetornado = string.IsNullOrEmpty(item[17].ToString()) ? string.Empty : item[17].ToString();
                                        if (!string.IsNullOrEmpty(item[17].ToString()))
                                        {
                                            persona.MigranteRetornado = item[17].ToString().Equals("Sí") ? 1 : 2;
                                        }
                                        columnaPer++;
                                        persona.PiensaMigrar = string.IsNullOrEmpty(item[18].ToString()) ? string.Empty : item[18].ToString();
                                        columnaPer++;
                                        persona.FamiliaresMigrantes = string.IsNullOrEmpty(item[19].ToString()) ? string.Empty : item[19].ToString();
                                        columnaPer++;
                                        persona.FamiliaresRetornados = string.IsNullOrEmpty(item[20].ToString()) ? string.Empty : item[20].ToString();
                                        columnaPer++;
                                        persona.Empleo = string.IsNullOrEmpty(item[21].ToString()) ? string.Empty : item[21].ToString();
                                        columnaPer++;
                                        persona.Dui = string.IsNullOrEmpty(item[22].ToString()) ? string.Empty : item[22].ToString();
                                        columnaPer++;
                                        persona.Nie = string.IsNullOrEmpty(item[23].ToString()) ? string.Empty : item[23].ToString();
                                        columnaPer++;
                                        persona.Correo = string.IsNullOrEmpty(item[24].ToString()) ? string.Empty : item[24].ToString();
                                        columnaPer++;
                                        if (!string.IsNullOrEmpty(item[25].ToString()))
                                        {
                                            var refiere = _catalogos.GetReferencias(item[25].ToString());
                                            persona.Refiere = refiere != null ? refiere.IdRefiere : null;
                                        }
                                        columnaPer++;
                                        if (!string.IsNullOrEmpty(item[26].ToString()))
                                        {
                                            var departamento = _catalogos.GetDepartamento(item[26].ToString());
                                            persona.Departamento = departamento != null ? departamento.IdDepartamento : null;
                                        }
                                        columnaPer++;
                                        if (!string.IsNullOrEmpty(item[27].ToString()))
                                        {
                                            var municipio = _catalogos.GetMunicipio(item[27].ToString());
                                            persona.Municipio = municipio != null ? municipio.IdMunicipio : null;
                                        }
                                        columnaPer++;

                                        if (!string.IsNullOrEmpty(item[28].ToString()))
                                        {
                                            var tipomatricula = _catalogos.GetTipoMatricula(item[28].ToString());
                                            persona.IdTipoMatricula = tipomatricula != null ? tipomatricula.IdTipoMatricula : 0;

                                        }
                                        columnaPer++;
                                        if (!string.IsNullOrEmpty(item[29].ToString()))
                                        {
                                            var programa = _catalogos.GetPrograma(item[29].ToString());
                                            persona.IdPrograma = programa != null ? programa.IdPrograma : 0;

                                        }
                                        columnaPer++;

                                        persona.Year = string.IsNullOrEmpty(item[30].ToString()) ? 0 : int.Parse(item[30].ToString());

                                        if (!string.IsNullOrEmpty(item[31].ToString()))
                                        {
                                            var cohorte = _catalogos.GetCohorte(item[31].ToString());
                                            persona.Cohorte = cohorte != null ? cohorte.IdCohorte : 0;

                                        }
                                        columnaPer++;

                                        if (!string.IsNullOrEmpty(item[32].ToString()))
                                        {
                                            var Catsede = _catalogos.GetSede(item[32].ToString());
                                            if (Catsede != null)
                                            {
                                                var sede = _ctx.Sedes.FirstOrDefault(x => x.IdCatSede.Equals(Catsede.IdCatSede));
                                                persona.p_socio = sede.IdSocio;
                                                persona.IdZona = sede.IdZona;
                                            }
                                            persona.p_sede = Catsede != null ? Catsede.IdCatSede : 0;

                                        }
                                        columnaPer++;
                                        if (!string.IsNullOrEmpty(item[33].ToString()))
                                        {
                                            var carrera = _catalogos.GetCarrera(item[33].ToString(), (int)persona.IdPrograma);
                                            persona.CarreraCursoGrado = carrera != null ? carrera.IdCatCarrera : 0;

                                        }
                                        columnaPer++;
                                        //sector34
                                        if (!string.IsNullOrEmpty(item[34].ToString()))
                                        {
                                            var sector = _catalogos.GetSector(item[34].ToString());
                                            persona.Sector = sector != null ? sector.IdSector : 0;
                                        }
                                        columnaPer++;
                                        persona.MedioVerificacion = string.IsNullOrEmpty(item[35].ToString()) ? "" : item[35].ToString();
                                        columnaPer++;
                                        if (!string.IsNullOrEmpty(item[36].ToString()))
                                        {
                                            var proyecto = _catalogos.GetProyectos(item[36].ToString());
                                            persona.IdProyecto = proyecto != null ? proyecto.IdProyecto : 0;
                                        }
                                        columnaPer++;
                                        persona.IdCarga = carga.IdCarga;

                                        _ctx.Entry(persona).State = EntityState.Added;
                                        columnaPer = 1;
                                    }
                                }
                                loopPer++;
                            }
                            await _ctx.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            var fila = loopPer + 1;
                            var colum = columnaPer;
                            string smn = ex.Message;
                            dbTransaction.Rollback();
                            throw new Exception(string.Format("Error al cargar datos en la tabla Inscripción verificar el número de fila {0} columna {1}", fila, colum));
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
        public async Task<string> LoadEducacionAsync(string path, string fileName)
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
                            DataTable tableeducacion = result.Tables[0];
                            carga.TotalEducacion = tableeducacion.Rows.Count;

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

                        // Tabla de educacion
                        int loopEd = 0;
                        int columnaEd = 1;
                        try
                        {
                            DataTable tableeducacion = result.Tables[0];
                            foreach (DataRow item in tableeducacion.Rows)
                            {
                                if (loopEd != 0)
                                {
                                    var ed = _ctx.CargaEducacions.FirstOrDefault(x => x.PIdOim.Equals(item[5].ToString()) && x.RAño.Equals(item[0].ToString()) && x.RMes.Equals(item[0].ToString()));
                                    if (ed == null)
                                    {
                                        CargaEducacion ccarga = new CargaEducacion();
                                        ccarga.RAño = string.IsNullOrEmpty(item[0].ToString()) ? 0 : Convert.ToInt32(item[0].ToString());
                                        columnaEd++;
                                        ccarga.RMes = string.IsNullOrEmpty(item[1].ToString()) ? string.Empty : item[1].ToString();
                                        columnaEd++;
                                        ccarga.r_fechaini = string.IsNullOrEmpty(item[2].ToString()) ? null : DateTime.TryParse(item[2].ToString(), out DateTime resultf1) == true ? DateTime.Parse(item[2].ToString()) : null;
                                        columnaEd++;
                                        ccarga.r_fechafin = string.IsNullOrEmpty(item[3].ToString()) ? null : DateTime.TryParse(item[3].ToString(), out DateTime resultf2) == true ? DateTime.Parse(item[3].ToString()) : null;
                                        columnaEd++;
                                        if (!string.IsNullOrEmpty(item[4].ToString()))
                                        {
                                            var Catsede = _catalogos.GetSede(item[4].ToString());
                                            if (Catsede != null)
                                            {
                                                var sede = _ctx.Sedes.FirstOrDefault(x => x.IdCatSede.Equals(Catsede.IdCatSede));
                                                ccarga.p_socio = sede.IdSocio;
                                                ccarga.IdZona = sede.IdZona;
                                            }
                                            ccarga.p_sede = Catsede != null ? Catsede.IdCatSede : 0;

                                        }
                                        columnaEd++;
                                        ccarga.PIdOim = item[5].ToString();
                                        columnaEd++;
                                        if (!string.IsNullOrEmpty(item[6].ToString()))
                                        {
                                            var tipobeca = _catalogos.GetPrograma(item[6].ToString());
                                            ccarga.p_tipobeca = tipobeca != null ? tipobeca.IdPrograma : 0;

                                        }
                                        columnaEd++;
                                        if (!string.IsNullOrEmpty(item[7].ToString()))
                                        {
                                            var carrera = _catalogos.GetCarrera(item[7].ToString(), (int)ccarga.p_tipobeca);
                                            ccarga.CarreraCursoGrado = carrera != null ? carrera.IdCatCarrera : 0;

                                        }
                                        columnaEd++;
                                        if (!string.IsNullOrEmpty(item[8].ToString()))
                                        {
                                            var matricula = _catalogos.GetTipoMatricula(item[8].ToString());
                                            ccarga.p_matricula = matricula != null ? matricula.IdTipoMatricula : 0;

                                        }
                                        columnaEd++;
                                        ccarga.Year = string.IsNullOrEmpty(item[9].ToString()) ? 0 : int.Parse(item[9].ToString());
                                        columnaEd++;
                                        if (!string.IsNullOrEmpty(item[10].ToString()))
                                        {
                                            var cohorte = _catalogos.GetCohorte(item[10].ToString());
                                            ccarga.Cohorte = cohorte != null ? cohorte.IdCohorte : 0;

                                        }
                                        columnaEd++;
                                        if (!string.IsNullOrEmpty(item[11].ToString()))
                                        {
                                            var sector = _catalogos.GetSector(item[11].ToString());
                                            ccarga.Sector = sector != null ? sector.IdSector : 0;
                                        }
                                        columnaEd++;
                                        ccarga.DFechaReasg = string.IsNullOrEmpty(item[12].ToString()) ? null : DateTime.Parse(item[12].ToString());
                                        columnaEd++;

                                        if (!string.IsNullOrEmpty(item[13].ToString()))
                                        {
                                            var estado = _catalogos.GetEstadoPersona(item[13].ToString());
                                            ccarga.DEstado = estado != null ? estado.IdEstadoPersona : 0;
                                        }
                                        columnaEd++;
                                        ccarga.DFechades = string.IsNullOrEmpty(item[14].ToString()) ? null : DateTime.Parse(item[14].ToString());
                                        columnaEd++;
                                        ccarga.DMotivodesercion = string.IsNullOrEmpty(item[15].ToString()) ? string.Empty : item[15].ToString();
                                        columnaEd++;
                                        ccarga.IDiasAsistenciaEstablecidos = string.IsNullOrEmpty(item[16].ToString()) ? 0 : int.Parse(item[16].ToString());
                                        columnaEd++;
                                        ccarga.IDiasAsistenciaEfectivos = string.IsNullOrEmpty(item[17].ToString()) ? 0 : int.Parse(item[17].ToString());
                                        columnaEd++;
                                        ccarga.i_proc_asistencia = string.IsNullOrEmpty(item[18].ToString()) ? string.Empty : item[18].ToString();
                                        columnaEd++;
                                        ccarga.IMotivoInasistencia = string.IsNullOrEmpty(item[19].ToString()) ? string.Empty : item[19].ToString();
                                        columnaEd++;
                                        ccarga.IModulosInscritos = string.IsNullOrEmpty(item[20].ToString()) ? 0 : int.Parse(item[20].ToString());
                                        columnaEd++;
                                        ccarga.IModulosAprobados = string.IsNullOrEmpty(item[21].ToString()) ? 0 : int.Parse(item[21].ToString());
                                        columnaEd++;
                                        ccarga.prueba_realizada = string.IsNullOrEmpty(item[22].ToString()) ? string.Empty : item[22].ToString();
                                        columnaEd++;
                                        ccarga.IModulosReprobados = string.IsNullOrEmpty(item[23].ToString()) ? 0 : int.Parse(item[23].ToString());
                                        columnaEd++;
                                        ccarga.ICausaReprobacion = string.IsNullOrEmpty(item[24].ToString()) ? string.Empty : item[24].ToString();
                                        columnaEd++;
                                        ccarga.IdCarga = carga.IdCarga;
                                        _ctx.Add(ccarga);
                                        columnaEd = 1;
                                    }
                                    else
                                    {
                                        ed.RAño = string.IsNullOrEmpty(item[0].ToString()) ? 0 : Convert.ToInt32(item[0].ToString());
                                        columnaEd++;
                                        ed.RMes = string.IsNullOrEmpty(item[1].ToString()) ? string.Empty : item[1].ToString();
                                        columnaEd++;
                                        ed.r_fechaini = string.IsNullOrEmpty(item[2].ToString()) ? null : DateTime.TryParse(item[2].ToString(), out DateTime resultf1) == true ? DateTime.Parse(item[2].ToString()) : null;
                                        columnaEd++;
                                        ed.r_fechafin = string.IsNullOrEmpty(item[3].ToString()) ? null : DateTime.TryParse(item[3].ToString(), out DateTime resultf2) == true ? DateTime.Parse(item[3].ToString()) : null;
                                        columnaEd++;
                                        if (!string.IsNullOrEmpty(item[4].ToString()))
                                        {
                                            var Catsede = _catalogos.GetSede(item[4].ToString());
                                            if (Catsede != null)
                                            {
                                                var sede = _ctx.Sedes.FirstOrDefault(x => x.IdCatSede.Equals(Catsede.IdCatSede));
                                                ed.p_socio = sede.IdSocio;
                                                ed.IdZona = sede.IdZona;
                                            }
                                            ed.p_sede = Catsede != null ? Catsede.IdCatSede : 0;

                                        }
                                        columnaEd++;
                                        ed.PIdOim = item[5].ToString();
                                        columnaEd++;
                                        if (!string.IsNullOrEmpty(item[6].ToString()))
                                        {
                                            var tipobeca = _catalogos.GetPrograma(item[6].ToString());
                                            ed.p_tipobeca = tipobeca != null ? tipobeca.IdPrograma : 0;

                                        }
                                        columnaEd++;
                                        if (!string.IsNullOrEmpty(item[7].ToString()))
                                        {
                                            var carrera = _catalogos.GetCarrera(item[7].ToString(), (int)ed.p_tipobeca);
                                            ed.CarreraCursoGrado = carrera != null ? carrera.IdCatCarrera : 0;

                                        }
                                        columnaEd++;
                                        if (!string.IsNullOrEmpty(item[8].ToString()))
                                        {
                                            var matricula = _catalogos.GetTipoMatricula(item[8].ToString());
                                            ed.p_matricula = matricula != null ? matricula.IdTipoMatricula : 0;

                                        }
                                        columnaEd++;
                                        ed.Year = string.IsNullOrEmpty(item[9].ToString()) ? 0 : int.Parse(item[9].ToString());
                                        columnaEd++;
                                        if (!string.IsNullOrEmpty(item[10].ToString()))
                                        {
                                            var cohorte = _catalogos.GetCohorte(item[10].ToString());
                                            ed.Cohorte = cohorte != null ? cohorte.IdCohorte : 0;

                                        }
                                        columnaEd++;
                                        if (!string.IsNullOrEmpty(item[11].ToString()))
                                        {
                                            var sector = _catalogos.GetSector(item[11].ToString());
                                            ed.Sector = sector != null ? sector.IdSector : 0;
                                        }
                                        columnaEd++;
                                        ed.DFechaReasg = string.IsNullOrEmpty(item[12].ToString()) ? null : DateTime.Parse(item[12].ToString());
                                        columnaEd++;

                                        if (!string.IsNullOrEmpty(item[13].ToString()))
                                        {
                                            var estado = _catalogos.GetEstadoPersona(item[13].ToString());
                                            ed.DEstado = estado != null ? estado.IdEstadoPersona : 0;
                                        }
                                        columnaEd++;
                                        ed.DFechades = string.IsNullOrEmpty(item[14].ToString()) ? null : DateTime.Parse(item[14].ToString());
                                        columnaEd++;
                                        ed.DMotivodesercion = string.IsNullOrEmpty(item[15].ToString()) ? string.Empty : item[15].ToString();
                                        columnaEd++;
                                        ed.IDiasAsistenciaEstablecidos = string.IsNullOrEmpty(item[16].ToString()) ? 0 : int.Parse(item[16].ToString());
                                        columnaEd++;
                                        ed.IDiasAsistenciaEfectivos = string.IsNullOrEmpty(item[17].ToString()) ? 0 : int.Parse(item[17].ToString());
                                        columnaEd++;
                                        ed.i_proc_asistencia = string.IsNullOrEmpty(item[18].ToString()) ? string.Empty : item[18].ToString();
                                        columnaEd++;
                                        ed.IMotivoInasistencia = string.IsNullOrEmpty(item[19].ToString()) ? string.Empty : item[19].ToString();
                                        columnaEd++;
                                        ed.IModulosInscritos = string.IsNullOrEmpty(item[20].ToString()) ? 0 : int.Parse(item[20].ToString());
                                        columnaEd++;
                                        ed.IModulosAprobados = string.IsNullOrEmpty(item[21].ToString()) ? 0 : int.Parse(item[21].ToString());
                                        columnaEd++;
                                        ed.prueba_realizada = string.IsNullOrEmpty(item[22].ToString()) ? string.Empty : item[22].ToString();
                                        columnaEd++;
                                        ed.IModulosReprobados = string.IsNullOrEmpty(item[23].ToString()) ? 0 : int.Parse(item[23].ToString());
                                        columnaEd++;
                                        ed.ICausaReprobacion = string.IsNullOrEmpty(item[24].ToString()) ? string.Empty : item[24].ToString();
                                        columnaEd++;
                                        ed.IdCarga = carga.IdCarga;
                                        _ctx.Update(ed);
                                        columnaEd = 1;
                                    }

                                }
                                loopEd++;
                            }
                            await _ctx.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            var fila = loopEd + 1;
                            var colum = columnaEd;
                            string smn = ex.Message;
                            dbTransaction.Rollback();
                            throw new Exception(string.Format("Error al cargar datos en la tabla Educación verificar el número de fila {0} columna {1}", fila, colum));
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
        public async Task<string> LoadPsocosocialAsync(string path, string fileName)
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
                            DataTable tablepsicosocial = result.Tables[0];

                            carga.TotalPsicosocial = tablepsicosocial.Rows.Count;

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

                        //Tabla psicosocial
                        int loopPsi = 0;
                        try
                        {
                            DataTable tablepsicosocial = result.Tables[0];
                            foreach (DataRow item in tablepsicosocial.Rows)
                            {
                                if (loopPsi != 0)
                                {

                                    CargaEvaluacionPsicosocial cargaEvaluacion = new CargaEvaluacionPsicosocial();
                                    int año = Convert.ToInt32(item[0].ToString());
                                    string mes = item[1].ToString();
                                    cargaEvaluacion.r_fechaini = string.IsNullOrEmpty(item[2].ToString()) ? null : DateTime.TryParse(item[2].ToString(), out DateTime resultf1) == true ? DateTime.Parse(item[2].ToString()) : null;
                                    cargaEvaluacion.r_fechafin = string.IsNullOrEmpty(item[3].ToString()) ? null : DateTime.TryParse(item[3].ToString(), out DateTime resultf2) == true ? DateTime.Parse(item[3].ToString()) : null;
                                    cargaEvaluacion.PId = string.IsNullOrEmpty(item[5].ToString()) ? "" : item[5].ToString();
                                    cargaEvaluacion.OvParticipacion = string.IsNullOrEmpty(item[8].ToString()) ? null : item[8].ToString().Equals("Si") ? true : false;
                                    cargaEvaluacion.OvPuntajePret = string.IsNullOrEmpty(item[9].ToString()) ? "" : item[9].ToString();
                                    cargaEvaluacion.OvPuntajePos = string.IsNullOrEmpty(item[10].ToString()) ? "" : item[10].ToString();
                                    cargaEvaluacion.EpInstrumentoRiesgo = string.IsNullOrEmpty(item[11].ToString()) ? "" : item[11].ToString();
                                    cargaEvaluacion.EpVulnerabilidades = string.IsNullOrEmpty(item[12].ToString()) ? "" : item[12].ToString();
                                    cargaEvaluacion.EpAlertaDesercion = string.IsNullOrEmpty(item[13].ToString()) ? "" : item[13].ToString();
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
        public async Task<string> LoadSeguimientoPsicosocialAsync(string path, string fileName)
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
                            DataTable tableSeguimientoPsicosocial = result.Tables[0];


                            carga.TotalSegPsicosocial = tableSeguimientoPsicosocial.Rows.Count;

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


                        //Tabla seguimiento psicosocial
                        int loopSegPsi = 0;
                        try
                        {
                            DataTable tableSeguimientoPsicosocial = result.Tables[0];
                            foreach (DataRow item in tableSeguimientoPsicosocial.Rows)
                            {
                                if (loopSegPsi != 0)
                                {
                                    CargaSeguimientoPsicosocial ee = new CargaSeguimientoPsicosocial();
                                    int año = Convert.ToInt32(item[0].ToString());
                                    string mes = item[1].ToString();
                                    ee.r_fechaini = string.IsNullOrEmpty(item[2].ToString()) ? null : DateTime.TryParse(item[2].ToString(), out DateTime resultf1) == true ? DateTime.Parse(item[2].ToString()) : null;
                                    ee.r_fechafin = string.IsNullOrEmpty(item[3].ToString()) ? null : DateTime.TryParse(item[3].ToString(), out DateTime resultf2) == true ? DateTime.Parse(item[3].ToString()) : null;
                                    ee.PId = string.IsNullOrEmpty(item[5].ToString()) ? "" : item[5].ToString();
                                    ee.SegMotivo = string.IsNullOrEmpty(item[8].ToString()) ? "" : item[8].ToString();
                                    ee.SegEstado = string.IsNullOrEmpty(item[9].ToString()) ? "" : item[9].ToString();
                                    ee.SegMedida = string.IsNullOrEmpty(item[10].ToString()) ? "" : item[10].ToString();
                                    ee.fecha_atencion = string.IsNullOrEmpty(item[11].ToString()) ? null : DateTime.TryParse(item[11].ToString(), out DateTime resultf3) == true ? DateTime.Parse(item[11].ToString()) : null;
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
        public async Task<string> LoadPracticasAsync(string path, string fileName)
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
                            DataTable tableSeguimientosPr = result.Tables[0];

                            carga.TotalSeguimientoPracticasPr = tableSeguimientosPr.Rows.Count;

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

                        //Tabla practicas Pr
                        int loopSegPra = 0;
                        try
                        {
                            DataTable tableSeguimientosPr = result.Tables[0];
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
        public async Task<string> LoadPasantillasAsync(string path, string fileName)
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
                            DataTable tableSeguimientPasantillas = result.Tables[0];
                            carga.TotalSeguimientoPasantias = tableSeguimientPasantillas.Rows.Count;

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

                        //Seguimiento Pasantillas
                        int loopsegPasantillas = 0;
                        try
                        {
                            DataTable tableSeguimientPsicosocial = result.Tables[0];
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
        public async Task<string> LoadAutoempleoAsync(string path, string fileName)
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
                            DataTable SeguimientoAutoempleo = result.Tables[0];


                            carga.TotalSeguimientoAutoempleo = SeguimientoAutoempleo.Rows.Count;


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

                        //Tabla SeguimientoAutoempleo
                        int loopSegAut = 0;
                        try
                        {
                            DataTable SeguimientoAutoempleo = result.Tables[0];
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
        public async Task<string> LoadEstipendiosAsync(string path, string fileName)
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
                            DataTable tableEstipendios = result.Tables[0];

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

                        //Tabla estipendios
                        int loopEst = 0;
                        try
                        {
                            DataTable tableEstipendios = result.Tables[0];
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
