using BECASLC;
using BECAS.Models.VM;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using BECAS.Models;

namespace BECAS.Interfaces
{
    public interface IPersonas
    {
        List<PersonTableVM> GetPersonas(DTParameters param);
        List<PersonTableVM> GetPersonasD(DTParameters param);
        List<SearchBeneficiaryTable> GetSearchBeneficiaries(DTSearchBeneficiary param);
        List<PersonTableVM> RepPromovidos();
    }

    public class PersonasRepository : IPersonas
    {
        private readonly MEOBContext _ctx;
        public PersonasRepository(MEOBContext context)
        {
            _ctx = context;
        }

        public List<PersonTableVM> GetPersonas(DTParameters param)
        {
            try
            {
                // Aquí se realiza la lógica para calcular el índice de inicio y la cantidad de registros por página
                int start = param.Start; // Índice de inicio de la página
                int length = param.Length; // Cantidad de registros por página
                int skip = start; // El índice de inicio ya es el número de registros a omitir


                List<PersonTableVM> personas = new List<PersonTableVM>();

                // Construye la consulta SQL
                string query = @"SELECT
                                    x.PIdOim,
                                    x.NumeroInscripciones,
                                    x.NombreCompleto,
                                    x.IdPrograma,
                                    p.Nombre AS NombrePrograma,
                                    CASE 
                                        WHEN x.IdPrograma IN (3,4,6) THEN (SELECT g.Nombre FROM [dbo].[Grados] g WHERE g.IdGrado = x.CarreraCursoGrado)
                                        ELSE c.Nombre
                                    END AS NombreCarrera,
                                    x.Telefono1,
                                    x.Telefono2,
                                    x.Sexo,
                                    s.Nombre AS NombreSexo,
                                    CASE WHEN x.LGBTIQ = 1 THEN 'sí' ELSE 'no' END AS LGBTIQ,
                                    x.FechaNacimiento,
                                    x.Edad,
                                    x.Discapacidad,
                                    x.VictimaViolencia,
                                    x.MigranteRetornado AS MigranteRetornadoID,
                                    CASE WHEN x.MigranteRetornado = 1 THEN 'sí' ELSE 'no' END AS MigranteRetornado,
                                    x.PiensaMigrar,
                                    x.FamiliaresMigrantes,
                                    x.FamiliaresRetornados,
                                    x.Empleo,
                                    x.Dui,
                                    x.Nie,
                                    x.Correo,
                                    x.Refiere,
                                    r.Nombre AS NombreRefiere,
                                    x.Departamento,
                                    d.Nombre AS NombreDepartamento,
                                    x.Municipio,
                                    m.Nombre AS NombreMunicipio,
                                    x.IdTipoMatricula,
                                    tm.Nombre AS NombreMatricula,
                                    x.Year,
                                    x.Cohorte,
                                    co.Nombre AS NombreCohorte,
                                    x.p_socio,
                                    si.Nombre AS NombreSocio,
                                    x.p_sede,
                                    se.Nombre AS NombreSede,
                                    x.UltimoGradoAprobado,
                                    x.NivelAcademico,
                                    educacion.d_estado,
                                    e.Nombre AS NombreEstado,
                                    x.IdZona,
                                    x.CarreraCursoGrado,
                                    educacion.year,
                                    x.IdProyecto
                                FROM 
                                    Persona AS x
                                LEFT JOIN 
                                    Programa AS p ON x.IdPrograma = p.IdPrograma
                                LEFT JOIN 
                                    CatCarrera AS c ON x.CarreraCursoGrado = c.IdCatCarrera 
                                LEFT JOIN 
                                    Sexo AS s ON x.Sexo = s.IdSexo 
                                LEFT JOIN 
                                    Refiere AS r ON x.Refiere = r.IdRefiere
                                LEFT JOIN 
                                    Departamento AS d ON x.Departamento = d.IdDepartamento
                                LEFT JOIN 
                                    Municipio AS m ON x.Municipio = m.IdMunicipio
                                LEFT JOIN 
                                    TipoMatricula AS tm ON x.IdTipoMatricula = tm.IdTipoMatricula
                                LEFT JOIN 
                                    Cohorte AS co ON x.Cohorte = co.IdCohorte
                                LEFT JOIN 
                                    SocioImplementador AS si ON x.p_socio = si.IdImplementador
                                LEFT JOIN 
                                    CatSede AS se ON x.p_sede = se.IdCatSede
                                LEFT JOIN 
                                    (
                                        SELECT 
                                            *,
                                            ROW_NUMBER() OVER (PARTITION BY p_id_oim ORDER BY r_fechaini DESC) AS rn
                                        FROM 
                                            CargaEducacion
                                    ) AS educacion ON x.PIdOim = educacion.p_id_oim AND educacion.rn = 1
                                LEFT JOIN 
                                    EstadoPersona AS e ON e.IdEstadoPersona = educacion.d_estado
                                ORDER BY 
                                    educacion.r_fechaini DESC";

                // Crea la conexión y ejecuta la consulta
                using (SqlConnection connection = new SqlConnection(_ctx.Database.GetConnectionString()))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PersonTableVM model = new PersonTableVM
                            {
                                PIdOim = reader["PIdOim"] != DBNull.Value ? Convert.ToString(reader["PIdOim"]) : null,
                                NumeroInscripciones = reader["NumeroInscripciones"] != DBNull.Value ? Convert.ToInt32(reader["NumeroInscripciones"]) : (int?)null,
                                NombreCompleto = reader["NombreCompleto"] != DBNull.Value ? Convert.ToString(reader["NombreCompleto"]) : null,
                                programa = reader["NombrePrograma"] != DBNull.Value ? Convert.ToString(reader["NombrePrograma"]) : null,
                                carrera = reader["NombreCarrera"] != DBNull.Value ? Convert.ToString(reader["NombreCarrera"]) : null,
                                Telefono1 = reader["Telefono1"] != DBNull.Value ? Convert.ToString(reader["Telefono1"]) : null,
                                Telefono2 = reader["Telefono2"] != DBNull.Value ? Convert.ToString(reader["Telefono2"]) : null,
                                SexoNombre = reader["NombreSexo"] != DBNull.Value ? Convert.ToString(reader["NombreSexo"]) : null,
                                LGBTIQ = reader["LGBTIQ"] != DBNull.Value ? Convert.ToString(reader["LGBTIQ"]) : null,
                                FechaNacimiento = reader["FechaNacimiento"] != DBNull.Value ? Convert.ToString(reader["FechaNacimiento"]) : null,
                                Edad = reader["Edad"] != DBNull.Value ? Convert.ToInt32(reader["Edad"]) : (int?)null,
                                Discapacidad = reader["Discapacidad"] != DBNull.Value ? Convert.ToString(reader["Discapacidad"]) : null,
                                VictimaViolencia = reader["VictimaViolencia"] != DBNull.Value ? Convert.ToString(reader["VictimaViolencia"]) : null,
                                MigranteRetornadoNombre = reader["MigranteRetornado"] != DBNull.Value ? Convert.ToString(reader["MigranteRetornado"]) : null,
                                PiensaMigrar = reader["PiensaMigrar"] != DBNull.Value ? Convert.ToString(reader["PiensaMigrar"]) : null,
                                FamiliaresMigrantes = reader["FamiliaresMigrantes"] != DBNull.Value ? Convert.ToString(reader["FamiliaresMigrantes"]) : null,
                                FamiliaresRetornados = reader["FamiliaresRetornados"] != DBNull.Value ? Convert.ToString(reader["FamiliaresRetornados"]) : null,
                                Empleo = reader["Empleo"] != DBNull.Value ? Convert.ToString(reader["Empleo"]) : null,
                                Dui = reader["Dui"] != DBNull.Value ? Convert.ToString(reader["Dui"]) : null,
                                Nie = reader["Nie"] != DBNull.Value ? Convert.ToString(reader["Nie"]) : null,
                                Correo = reader["Correo"] != DBNull.Value ? Convert.ToString(reader["Correo"]) : null,
                                RefiereNombre = reader["NombreRefiere"] != DBNull.Value ? Convert.ToString(reader["NombreRefiere"]) : null,
                                DepartamentoNombre = reader["NombreDepartamento"] != DBNull.Value ? Convert.ToString(reader["NombreDepartamento"]) : null,
                                MunicipioNombre = reader["NombreMunicipio"] != DBNull.Value ? Convert.ToString(reader["NombreMunicipio"]) : null,
                                tipomatricula = reader["NombreMatricula"] != DBNull.Value ? Convert.ToString(reader["NombreMatricula"]) : null,
                                Year = reader["Year"] != DBNull.Value ? Convert.ToInt32(reader["Year"]) : (int?)null,
                                CohorteNombre = reader["NombreCohorte"] != DBNull.Value ? Convert.ToString(reader["NombreCohorte"]) : null,
                                p_socioNombre = reader["NombreSocio"] != DBNull.Value ? Convert.ToString(reader["NombreSocio"]) : null,
                                p_sedeNombre = reader["NombreSede"] != DBNull.Value ? Convert.ToString(reader["NombreSede"]) : null,
                                UltimoGradoAprobado = reader["UltimoGradoAprobado"] != DBNull.Value ? Convert.ToString(reader["UltimoGradoAprobado"]) : null,
                                NivelAcademico = reader["NivelAcademico"] != DBNull.Value ? Convert.ToString(reader["NivelAcademico"]) : null,
                                Estado = reader["NombreEstado"] != DBNull.Value ? Convert.ToString(reader["NombreEstado"]) : null,
                                p_socio = reader["p_socio"] != DBNull.Value ? Convert.ToInt32(reader["p_socio"]) : (int?)null,
                                IdZona = reader["IdZona"] != DBNull.Value ? Convert.ToInt32(reader["IdZona"]) : (int?)null,
                                IdPrograma = reader["IdPrograma"] != DBNull.Value ? Convert.ToInt32(reader["IdPrograma"]) : (int?)null,
                                p_sede = reader["p_sede"] != DBNull.Value ? Convert.ToInt32(reader["p_sede"]) : (int?)null,
                                CarreraCursoGrado = reader["CarreraCursoGrado"] != DBNull.Value ? Convert.ToInt32(reader["CarreraCursoGrado"]) : (int?)null,
                                Sexo = reader["Sexo"] != DBNull.Value ? Convert.ToInt32(reader["Sexo"]) : (int?)null,
                                IdTipomatricula = reader["IdTipomatricula"] != DBNull.Value ? Convert.ToInt32(reader["IdTipomatricula"]) : (int?)null,
                                Departamento = reader["Departamento"] != DBNull.Value ? Convert.ToInt32(reader["Departamento"]) : (int?)null,
                                Refiere = reader["Refiere"] != DBNull.Value ? Convert.ToInt32(reader["Refiere"]) : (int?)null,
                                añoestudio = reader["year"] != DBNull.Value ? Convert.ToInt32(reader["year"]) : (int?)null,
                                Cohorte = reader["Cohorte"] != DBNull.Value ? Convert.ToInt32(reader["Cohorte"]) : (int?)null,
                                proyecto = reader["IdProyecto"] != DBNull.Value ? Convert.ToInt32(reader["IdProyecto"]) : (int?)null,
                                IdEstado = reader["d_estado"] != DBNull.Value ? Convert.ToInt32(reader["d_estado"]) : (int?)null,
                                MigranteRetornado = reader["MigranteRetornadoID"] != DBNull.Value ? Convert.ToInt32(reader["MigranteRetornadoID"]) : (int?)null,
                            };
                            personas.Add(model);
                        }
                    }
                }

                // Aplica filtros a la lista de personas si es necesario
                if (param.socios != null && param.socios.Count > 0 && param.socios[0] != null)
                {
                    var sociosIds = param.socios
                        .Where(s => int.TryParse(s, out _)) // Filtra solo los elementos que se pueden convertir a int
                        .Select(s => int.Parse(s)) // Convierte los elementos a int
                        .ToList(); // Convierte el resultado en una lista

                    personas = personas.Where(x => x.p_socio.HasValue && sociosIds.Contains(x.p_socio.Value)).ToList();
                }

                if (param.DropZona != null && param.DropZona.Count > 0 && param.DropZona[0] != null)
                {
                    //var DropZona = param.DropZona.Select(s => Convert.ToInt32(s));
                    var DropZona = param.DropZona
                        .Where(s => int.TryParse(s, out _)) // Filtra solo los elementos que se pueden convertir a int
                        .Select(s => int.Parse(s)) // Convierte los elementos a int
                        .ToList(); // Convierte el resultado en una lista
                    personas = personas.Where(x => x.IdZona.HasValue && DropZona.Contains(x.IdZona.Value)).ToList();
                }

                if (param.IdPrograma != null && param.IdPrograma.Count > 0 && param.IdPrograma[0] != null)
                {
                    //var IdPrograma = param.IdPrograma.Select(s => Convert.ToInt32(s));
                    var IdPrograma = param.IdPrograma
                        .Where(s => int.TryParse(s, out _)) // Filtra solo los elementos que se pueden convertir a int
                        .Select(s => int.Parse(s)) // Convierte los elementos a int
                        .ToList(); // Convierte el resultado en una lista
                    personas = personas.Where(x => x.IdPrograma.HasValue && IdPrograma.Contains(x.IdPrograma.Value)).ToList();
                }

                if (param.Sedes != null && param.Sedes.Count > 0 && param.Sedes[0] != null)
                {
                    // Filtrar y convertir elementos que pueden ser convertidos a int
                    var sedeIds = param.Sedes
                        .Select(s => int.TryParse(s, out var id) ? (int?)id : null)
                        .Where(id => id.HasValue)
                        .Select(id => id.Value)
                        .ToList();

                    // Obtener las sedes de la base de datos que coinciden con los IDs filtrados
                    var catSedes = _ctx.Sedes
                        .Where(sede => sedeIds.Contains(sede.IdSede))
                        .ToList();

                    // Filtrar personas que tienen una sede válida y que están en la lista de sedes filtradas
                    personas = personas
                        .Where(persona => persona.p_sede.HasValue && catSedes.Any(sede => sede.IdCatSede == persona.p_sede.Value))
                        .ToList();

                }

                if (param.Carreras != null && param.Carreras.Count > 0 && param.Carreras[0] != null)
                {
                    //var Carreras = param.Carreras.Select(s => Convert.ToInt32(s));
                    var Carreras = param.Carreras
                        .Where(s => int.TryParse(s, out _)) // Filtra solo los elementos que se pueden convertir a int
                        .Select(s => int.Parse(s)) // Convierte los elementos a int
                        .ToList(); // Convierte el resultado en una lista
                    personas = personas.Where(x => x.CarreraCursoGrado.HasValue && Carreras.Contains(x.CarreraCursoGrado.Value)).ToList();
                }

                if (param.Sexos != null && param.Sexos.Count > 0 && param.Sexos[0] != null)
                {
                    //var Sexos = param.Sexos.Select(s => Convert.ToInt32(s));
                    var Sexos = param.Sexos
                        .Where(s => int.TryParse(s, out _)) // Filtra solo los elementos que se pueden convertir a int
                        .Select(s => int.Parse(s)) // Convierte los elementos a int
                        .ToList(); // Convierte el resultado en una lista
                    personas = personas.Where(x => x.CarreraCursoGrado.HasValue && Sexos.Contains(x.Sexo.Value)).ToList();
                }

                if (param.TipoMatricula != null && param.TipoMatricula.Count > 0 && param.TipoMatricula[0] != null)
                {
                    //var TipoMatricula = param.TipoMatricula.Select(s => Convert.ToInt32(s));
                    var TipoMatricula = param.TipoMatricula
                        .Where(s => int.TryParse(s, out _)) // Filtra solo los elementos que se pueden convertir a int
                        .Select(s => int.Parse(s)) // Convierte los elementos a int
                        .ToList(); // Convierte el resultado en una lista
                    personas = personas.Where(x => x.IdTipomatricula.HasValue && TipoMatricula.Contains(x.IdTipomatricula.Value)).ToList();
                }

                if (param.Departamento != null && param.Departamento.Count > 0 && param.Departamento[0] != null)
                {
                    //var Departamento = param.Departamento.Select(s => Convert.ToInt32(s));
                    var Departamento = param.Departamento
                        .Where(s => int.TryParse(s, out _)) // Filtra solo los elementos que se pueden convertir a int
                        .Select(s => int.Parse(s)) // Convierte los elementos a int
                        .ToList(); // Convierte el resultado en una lista
                    personas = personas.Where(x => x.Departamento.HasValue && Departamento.Contains(x.Departamento.Value)).ToList();
                }

                if (param.Refiere != null && param.Refiere.Count > 0 && param.Refiere[0] != null)
                {
                    //var Refiere = param.Refiere.Select(s => Convert.ToInt32(s));
                    var Refiere = param.Refiere
                        .Where(s => int.TryParse(s, out _)) // Filtra solo los elementos que se pueden convertir a int
                        .Select(s => int.Parse(s)) // Convierte los elementos a int
                        .ToList(); // Convierte el resultado en una lista
                    personas = personas.Where(x => x.Refiere.HasValue && Refiere.Contains(x.Refiere.Value)).ToList();
                }

                //if (param.Year1 != null && param.Year1.Count > 0)
                //{
                //    var Year1 = param.Year1.Select(s => Convert.ToInt32(s));
                //    personas = personas.Where(x => Year1.Contains((int)x.Year)).ToList();
                //}

                if (param.AEstudio != null && param.AEstudio.Count > 0 && param.AEstudio[0] != null)
                {
                    //var AEstudio = param.AEstudio.Select(s => Convert.ToInt32(s));
                    var AEstudio = param.AEstudio
                        .Where(s => int.TryParse(s, out _)) // Filtra solo los elementos que se pueden convertir a int
                        .Select(s => int.Parse(s)) // Convierte los elementos a int
                        .ToList(); // Convierte el resultado en una lista
                    personas = personas.Where(x => x.añoestudio.HasValue && AEstudio.Contains(x.añoestudio.Value)).ToList();
                }

                if (param.Cohorte != null && param.Cohorte.Count > 0 && param.Cohorte[0] != null)
                {
                    //var Cohorte = param.Cohorte.Select(s => Convert.ToInt32(s));
                    var Cohorte = param.Cohorte
                        .Where(s => int.TryParse(s, out _)) // Filtra solo los elementos que se pueden convertir a int
                        .Select(s => int.Parse(s)) // Convierte los elementos a int
                        .ToList(); // Convierte el resultado en una lista
                    personas = personas.Where(x => x.Cohorte.HasValue && Cohorte.Contains(x.Cohorte.Value)).ToList();
                }

                if (param.Proyecto != null && param.Proyecto.Count > 0 && param.Proyecto[0] != null)
                {
                    //var Proyecto = param.Proyecto.Select(s => Convert.ToInt32(s));
                    var Proyecto = param.Proyecto
                        .Where(s => int.TryParse(s, out _)) // Filtra solo los elementos que se pueden convertir a int
                        .Select(s => int.Parse(s)) // Convierte los elementos a int
                        .ToList(); // Convierte el resultado en una lista
                    personas = personas.Where(x => x.proyecto.HasValue && Proyecto.Contains(x.proyecto.Value)).ToList();
                }

                if (param.Estado != null && param.Estado.Count > 0 && param.Estado[0] != null)
                {
                    //var Estado = param.Estado.Select(s => Convert.ToInt32(s));
                    var Estado = param.Estado
                        .Where(s => int.TryParse(s, out _)) // Filtra solo los elementos que se pueden convertir a int
                        .Select(s => int.Parse(s)) // Convierte los elementos a int
                        .ToList(); // Convierte el resultado en una lista
                    personas = personas.Where(x => x.IdEstado.HasValue && Estado.Contains(x.IdEstado.Value)).ToList();
                }

                if (param.Retornado != null && param.Retornado.Count > 0 && param.Retornado[0] != null)
                {
                    //var Retornado = param.Retornado.Select(s => Convert.ToInt32(s));
                    var Retornado = param.Retornado
                        .Where(s => int.TryParse(s, out _)) // Filtra solo los elementos que se pueden convertir a int
                        .Select(s => int.Parse(s)) // Convierte los elementos a int
                        .ToList(); // Convierte el resultado en una lista
                    personas = personas.Where(x => x.MigranteRetornado.HasValue && Retornado.Contains(x.MigranteRetornado.Value)).ToList();
                }

                param.count = personas.Count();

                return personas.Skip(skip)
                                    .Take(length)
                                    .ToList();

            }
            catch (Exception ex)
            {
                // Maneja la excepción adecuadamente
                Console.WriteLine("Se produjo una excepción: " + ex.Message);
                throw; // Lanza la excepción nuevamente para que sea manejada en un nivel superior
            }
        }

        public List<PersonTableVM> GetPersonasD(DTParameters param)
        {
            try
            {

                List<PersonTableVM> personas = new List<PersonTableVM>();

                // Construye la consulta SQL
                string query = @"SELECT
                                x.PIdOim,
                                x.NumeroInscripciones,
                                x.NombreCompleto,
                                x.IdPrograma,
                                p.Nombre AS NombrePrograma,
                                CASE 
        WHEN x.IdPrograma IN (3, 6) THEN (SELECT g.Nombre FROM [dbo].[Grados] g WHERE g.IdGrado = x.CarreraCursoGrado)
        ELSE c.Nombre
    END AS NombreCarrera,
                                x.Telefono1,
                                x.Telefono2,
                                x.Sexo,
                                s.Nombre AS NombreSexo,
                                CASE WHEN x.LGBTIQ = 1 THEN 'sí' ELSE 'no' END AS LGBTIQ,
                                x.FechaNacimiento,
                                x.Edad,
                                x.Discapacidad,
                                x.VictimaViolencia,
                                x.MigranteRetornado AS MigranteRetornadoID,
                                CASE WHEN x.MigranteRetornado = 1 THEN 'sí' ELSE 'no' END AS MigranteRetornado,
                                x.PiensaMigrar,
                                x.FamiliaresMigrantes,
                                x.FamiliaresRetornados,
                                x.Empleo,
                                x.Dui,
                                x.Nie,
                                x.Correo,
                                x.Refiere,
                                r.Nombre AS NombreRefiere,
                                x.Departamento,
                                d.Nombre AS NombreDepartamento,
                                x.Municipio,
                                m.Nombre AS NombreMunicipio,
                                x.IdTipoMatricula,
                                tm.Nombre AS NombreMatricula,
                                x.Year,
                                x.Cohorte,
                                co.Nombre AS NombreCohorte,
                                x.p_socio,
                                si.Nombre AS NombreSocio,
                                x.p_sede,
                                se.Nombre AS NombreSede,
                                x.UltimoGradoAprobado,
                                x.NivelAcademico,
                                educacion.d_estado,
                                e.Nombre AS NombreEstado,
                                x.IdZona,
                                x.CarreraCursoGrado,
                                educacion.year AS AñoEstudio,
                                x.IdProyecto
                            FROM 
                                Persona AS x
                            LEFT JOIN 
                                Programa AS p ON x.IdPrograma = p.IdPrograma
                            LEFT JOIN 
                                CatCarrera AS c ON x.CarreraCursoGrado = c.IdCatCarrera 
                            LEFT JOIN 
                                Sexo AS s ON x.Sexo = s.IdSexo 
                            LEFT JOIN 
                                Refiere AS r ON x.Refiere = r.IdRefiere
                            LEFT JOIN 
                                Departamento AS d ON x.Departamento = d.IdDepartamento
                            LEFT JOIN 
                                Municipio AS m ON x.Municipio = m.IdMunicipio
                            LEFT JOIN 
                                TipoMatricula AS tm ON x.IdTipoMatricula = tm.IdTipoMatricula
                            LEFT JOIN 
                                Cohorte AS co ON x.Cohorte = co.IdCohorte
                            LEFT JOIN 
                                SocioImplementador AS si ON x.p_socio = si.IdImplementador
                            LEFT JOIN 
                                CatSede AS se ON x.p_sede = se.IdCatSede
                            LEFT JOIN 
                                (
                                    SELECT 
                                        *,
                                        ROW_NUMBER() OVER (PARTITION BY p_id_oim ORDER BY r_fechaini DESC) AS rn
                                    FROM 
                                        CargaEducacion
                                ) AS educacion ON x.PIdOim = educacion.p_id_oim AND educacion.rn = 1
                            LEFT JOIN 
                                EstadoPersona AS e ON e.IdEstadoPersona = educacion.d_estado
                            ORDER BY 
                                educacion.r_fechaini DESC";

                // Crea la conexión y ejecuta la consulta
                using (SqlConnection connection = new SqlConnection(_ctx.Database.GetConnectionString()))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PersonTableVM model = new PersonTableVM
                            {
                                PIdOim = Convert.ToString(reader["PIdOim"]),
                                NumeroInscripciones = Convert.ToInt32(reader["NumeroInscripciones"]),
                                NombreCompleto = Convert.ToString(reader["NombreCompleto"]),
                                programa = Convert.ToString(reader["NombrePrograma"]),
                                carrera = Convert.ToString(reader["NombreCarrera"]),
                                Telefono1 = Convert.ToString(reader["Telefono1"]),
                                Telefono2 = Convert.ToString(reader["Telefono2"]),
                                SexoNombre = Convert.ToString(reader["NombreSexo"]),
                                LGBTIQ = Convert.ToString(reader["LGBTIQ"]),
                                FechaNacimiento = Convert.ToString(reader["FechaNacimiento"]),
                                Edad = Convert.ToInt32(reader["Edad"]),
                                Discapacidad = Convert.ToString(reader["Discapacidad"]),
                                VictimaViolencia = Convert.ToString(reader["VictimaViolencia"]),
                                MigranteRetornadoNombre = Convert.ToString(reader["MigranteRetornado"]),
                                PiensaMigrar = Convert.ToString(reader["PiensaMigrar"]),
                                FamiliaresMigrantes = Convert.ToString(reader["FamiliaresMigrantes"]),
                                FamiliaresRetornados = Convert.ToString(reader["FamiliaresRetornados"]),
                                Empleo = Convert.ToString(reader["Empleo"]),
                                Dui = Convert.ToString(reader["Dui"]),
                                Nie = Convert.ToString(reader["Nie"]),
                                Correo = Convert.ToString(reader["Correo"]),
                                RefiereNombre = Convert.ToString(reader["NombreRefiere"]),
                                DepartamentoNombre = Convert.ToString(reader["NombreDepartamento"]),
                                MunicipioNombre = Convert.ToString(reader["NombreMunicipio"]),
                                tipomatricula = Convert.ToString(reader["NombreMatricula"]),
                                CohorteNombre = Convert.ToString(reader["NombreCohorte"]),
                                p_socioNombre = Convert.ToString(reader["NombreSocio"]),
                                p_sedeNombre = Convert.ToString(reader["NombreSede"]),
                                UltimoGradoAprobado = Convert.ToString(reader["UltimoGradoAprobado"]),
                                NivelAcademico = Convert.ToString(reader["NivelAcademico"]),
                                Estado = Convert.ToString(reader["NombreEstado"]),
                                añoestudio = Convert.ToInt32(reader["AñoEstudio"]),
                                p_socio = reader["p_socio"] != DBNull.Value ? Convert.ToInt32(reader["p_socio"]) : (int?)null,
                                IdZona = reader["IdZona"] != DBNull.Value ? Convert.ToInt32(reader["IdZona"]) : (int?)null,
                                IdPrograma = reader["IdPrograma"] != DBNull.Value ? Convert.ToInt32(reader["IdPrograma"]) : (int?)null,
                                p_sede = reader["p_sede"] != DBNull.Value ? Convert.ToInt32(reader["p_sede"]) : (int?)null,
                                CarreraCursoGrado = reader["CarreraCursoGrado"] != DBNull.Value ? Convert.ToInt32(reader["CarreraCursoGrado"]) : (int?)null,
                                Sexo = reader["Sexo"] != DBNull.Value ? Convert.ToInt32(reader["Sexo"]) : (int?)null,
                                IdTipomatricula = reader["IdTipomatricula"] != DBNull.Value ? Convert.ToInt32(reader["IdTipomatricula"]) : (int?)null,
                                Departamento = reader["Departamento"] != DBNull.Value ? Convert.ToInt32(reader["Departamento"]) : (int?)null,
                                Refiere = reader["Refiere"] != DBNull.Value ? Convert.ToInt32(reader["Refiere"]) : (int?)null,
                                Cohorte = reader["Cohorte"] != DBNull.Value ? Convert.ToInt32(reader["Cohorte"]) : (int?)null,
                                proyecto = reader["IdProyecto"] != DBNull.Value ? Convert.ToInt32(reader["IdProyecto"]) : (int?)null,
                                IdEstado = reader["d_estado"] != DBNull.Value ? Convert.ToInt32(reader["d_estado"]) : (int?)null,
                                MigranteRetornado = reader["MigranteRetornadoID"] != DBNull.Value ? Convert.ToInt32(reader["MigranteRetornadoID"]) : (int?)null,

                            };

                            personas.Add(model);
                        }
                    }
                }
                // Aplica filtros a la lista de personas si es necesario
                if (param.socios != null && param.socios.Count > 0 && param.socios[0] != null)
                {
                    var sociosIds = param.socios
                        .Where(s => int.TryParse(s, out _)) // Filtra solo los elementos que se pueden convertir a int
                        .Select(s => int.Parse(s)) // Convierte los elementos a int
                        .ToList(); // Convierte el resultado en una lista

                    personas = personas.Where(x => x.p_socio.HasValue && sociosIds.Contains(x.p_socio.Value)).ToList();
                }

                if (param.DropZona != null && param.DropZona.Count > 0 && param.DropZona[0] != null)
                {
                    //var DropZona = param.DropZona.Select(s => Convert.ToInt32(s));
                    var DropZona = param.DropZona
                        .Where(s => int.TryParse(s, out _)) // Filtra solo los elementos que se pueden convertir a int
                        .Select(s => int.Parse(s)) // Convierte los elementos a int
                        .ToList(); // Convierte el resultado en una lista
                    personas = personas.Where(x => x.IdZona.HasValue && DropZona.Contains(x.IdZona.Value)).ToList();
                }

                if (param.IdPrograma != null && param.IdPrograma.Count > 0 && param.IdPrograma[0] != null)
                {
                    //var IdPrograma = param.IdPrograma.Select(s => Convert.ToInt32(s));
                    var IdPrograma = param.IdPrograma
                        .Where(s => int.TryParse(s, out _)) // Filtra solo los elementos que se pueden convertir a int
                        .Select(s => int.Parse(s)) // Convierte los elementos a int
                        .ToList(); // Convierte el resultado en una lista
                    personas = personas.Where(x => x.IdPrograma.HasValue && IdPrograma.Contains(x.IdPrograma.Value)).ToList();
                }

                if (param.Sedes != null && param.Sedes.Count > 0 && param.Sedes[0] != null)
                {
                    // Filtrar y convertir elementos que pueden ser convertidos a int
                    var sedeIds = param.Sedes
                        .Select(s => int.TryParse(s, out var id) ? (int?)id : null)
                        .Where(id => id.HasValue)
                        .Select(id => id.Value)
                        .ToList();

                    // Obtener las sedes de la base de datos que coinciden con los IDs filtrados
                    var catSedes = _ctx.Sedes
                        .Where(sede => sedeIds.Contains(sede.IdSede))
                        .ToList();

                    // Filtrar personas que tienen una sede válida y que están en la lista de sedes filtradas
                    personas = personas
                        .Where(persona => persona.p_sede.HasValue && catSedes.Any(sede => sede.IdCatSede == persona.p_sede.Value))
                        .ToList();
                }

                if (param.Carreras != null && param.Carreras.Count > 0 && param.Carreras[0] != null)
                {
                    //var Carreras = param.Carreras.Select(s => Convert.ToInt32(s));
                    var Carreras = param.Carreras
                        .Where(s => int.TryParse(s, out _)) // Filtra solo los elementos que se pueden convertir a int
                        .Select(s => int.Parse(s)) // Convierte los elementos a int
                        .ToList(); // Convierte el resultado en una lista
                    personas = personas.Where(x => x.CarreraCursoGrado.HasValue && Carreras.Contains(x.CarreraCursoGrado.Value)).ToList();
                }

                if (param.Sexos != null && param.Sexos.Count > 0 && param.Sexos[0] != null)
                {
                    //var Sexos = param.Sexos.Select(s => Convert.ToInt32(s));
                    var Sexos = param.Sexos
                        .Where(s => int.TryParse(s, out _)) // Filtra solo los elementos que se pueden convertir a int
                        .Select(s => int.Parse(s)) // Convierte los elementos a int
                        .ToList(); // Convierte el resultado en una lista
                    personas = personas.Where(x => x.CarreraCursoGrado.HasValue && Sexos.Contains(x.Sexo.Value)).ToList();
                }

                if (param.TipoMatricula != null && param.TipoMatricula.Count > 0 && param.TipoMatricula[0] != null)
                {
                    //var TipoMatricula = param.TipoMatricula.Select(s => Convert.ToInt32(s));
                    var TipoMatricula = param.TipoMatricula
                        .Where(s => int.TryParse(s, out _)) // Filtra solo los elementos que se pueden convertir a int
                        .Select(s => int.Parse(s)) // Convierte los elementos a int
                        .ToList(); // Convierte el resultado en una lista
                    personas = personas.Where(x => x.IdTipomatricula.HasValue && TipoMatricula.Contains(x.IdTipomatricula.Value)).ToList();
                }

                if (param.Departamento != null && param.Departamento.Count > 0 && param.Departamento[0] != null)
                {
                    //var Departamento = param.Departamento.Select(s => Convert.ToInt32(s));
                    var Departamento = param.Departamento
                        .Where(s => int.TryParse(s, out _)) // Filtra solo los elementos que se pueden convertir a int
                        .Select(s => int.Parse(s)) // Convierte los elementos a int
                        .ToList(); // Convierte el resultado en una lista
                    personas = personas.Where(x => x.Departamento.HasValue && Departamento.Contains(x.Departamento.Value)).ToList();
                }

                if (param.Refiere != null && param.Refiere.Count > 0 && param.Refiere[0] != null)
                {
                    //var Refiere = param.Refiere.Select(s => Convert.ToInt32(s));
                    var Refiere = param.Refiere
                        .Where(s => int.TryParse(s, out _)) // Filtra solo los elementos que se pueden convertir a int
                        .Select(s => int.Parse(s)) // Convierte los elementos a int
                        .ToList(); // Convierte el resultado en una lista
                    personas = personas.Where(x => x.Refiere.HasValue && Refiere.Contains(x.Refiere.Value)).ToList();
                }

                //if (param.Year1 != null && param.Year1.Count > 0)
                //{
                //    var Year1 = param.Year1.Select(s => Convert.ToInt32(s));
                //    personas = personas.Where(x => Year1.Contains((int)x.Year)).ToList();
                //}

                if (param.AEstudio != null && param.AEstudio.Count > 0 && param.AEstudio[0] != null)
                {
                    //var AEstudio = param.AEstudio.Select(s => Convert.ToInt32(s));
                    var AEstudio = param.AEstudio
                        .Where(s => int.TryParse(s, out _)) // Filtra solo los elementos que se pueden convertir a int
                        .Select(s => int.Parse(s)) // Convierte los elementos a int
                        .ToList(); // Convierte el resultado en una lista
                    personas = personas.Where(x => x.añoestudio.HasValue && AEstudio.Contains(x.añoestudio.Value)).ToList();
                }

                if (param.Cohorte != null && param.Cohorte.Count > 0 && param.Cohorte[0] != null)
                {
                    //var Cohorte = param.Cohorte.Select(s => Convert.ToInt32(s));
                    var Cohorte = param.Cohorte
                        .Where(s => int.TryParse(s, out _)) // Filtra solo los elementos que se pueden convertir a int
                        .Select(s => int.Parse(s)) // Convierte los elementos a int
                        .ToList(); // Convierte el resultado en una lista
                    personas = personas.Where(x => x.Cohorte.HasValue && Cohorte.Contains(x.Cohorte.Value)).ToList();
                }

                if (param.Proyecto != null && param.Proyecto.Count > 0 && param.Proyecto[0] != null)
                {
                    //var Proyecto = param.Proyecto.Select(s => Convert.ToInt32(s));
                    var Proyecto = param.Proyecto
                        .Where(s => int.TryParse(s, out _)) // Filtra solo los elementos que se pueden convertir a int
                        .Select(s => int.Parse(s)) // Convierte los elementos a int
                        .ToList(); // Convierte el resultado en una lista
                    personas = personas.Where(x => x.proyecto.HasValue && Proyecto.Contains(x.proyecto.Value)).ToList();
                }

                if (param.Estado != null && param.Estado.Count > 0 && param.Estado[0] != null)
                {
                    //var Estado = param.Estado.Select(s => Convert.ToInt32(s));
                    var Estado = param.Estado
                        .Where(s => int.TryParse(s, out _)) // Filtra solo los elementos que se pueden convertir a int
                        .Select(s => int.Parse(s)) // Convierte los elementos a int
                        .ToList(); // Convierte el resultado en una lista
                    personas = personas.Where(x => x.IdEstado.HasValue && Estado.Contains(x.IdEstado.Value)).ToList();
                }

                if (param.Retornado != null && param.Retornado.Count > 0 && param.Retornado[0] != null)
                {
                    //var Retornado = param.Retornado.Select(s => Convert.ToInt32(s));
                    var Retornado = param.Retornado
                        .Where(s => int.TryParse(s, out _)) // Filtra solo los elementos que se pueden convertir a int
                        .Select(s => int.Parse(s)) // Convierte los elementos a int
                        .ToList(); // Convierte el resultado en una lista
                    personas = personas.Where(x => x.MigranteRetornado.HasValue && Retornado.Contains(x.MigranteRetornado.Value)).ToList();
                }
                return personas.ToList();

            }
            catch (Exception ex)
            {
                // Maneja la excepción adecuadamente
                Console.WriteLine("Se produjo una excepción: " + ex.Message);
                throw; // Lanza la excepción nuevamente para que sea manejada en un nivel superior
            }
        }

        public List<SearchBeneficiaryTable> GetSearchBeneficiaries(DTSearchBeneficiary param)
        {
            try
            {
                // Aquí se realiza la lógica para calcular el índice de inicio y la cantidad de registros por página
                int start = param.Start; // Índice de inicio de la página
                int length = param.Length; // Cantidad de registros por página
                int skip = start; // El índice de inicio ya es el número de registros a omitir


                List<SearchBeneficiaryTable> personas = new List<SearchBeneficiaryTable>();

                // Construye la consulta SQL
                string query = @"SELECT 
                                   p.Nombre,
                                   p.Apellido,
                                   CONCAT(p.Nombre, ' ', p.Apellido) AS NombreCompleto,
                                   p.Dui,
                                   p.Edad,
                                   p.Correo,
                                   p.Telefono1,
                                   p.Telefono2,
                                   ISNULL((SELECT s.Nombre FROM [dbo].[Sexo] s WHERE s.IdSexo = p.Sexo), '-') AS Sexo,
                                   ISNULL((SELECT pr.Nombre FROM [dbo].[Programa] pr WHERE pr.IdPrograma = p.IdPrograma), '-') AS NombrePrograma,
                                   ISNULL(
                                       (CASE 
                                           WHEN p.IdPrograma IN (3, 4, 6) THEN 
                                               (SELECT g.Nombre FROM [dbo].[Grados] g WHERE g.IdGrado = p.CarreraCursoGrado)
                                           ELSE 
                                               (SELECT c.Nombre FROM [dbo].[CatCarrera] c WHERE c.IdCatCarrera = p.CarreraCursoGrado)
                                       END), '-') AS CarreraCursoGrado
                               FROM 
                                   [dbo].[Persona] p;";

                // Crea la conexión y ejecuta la consulta
                using (SqlConnection connection = new SqlConnection(_ctx.Database.GetConnectionString()))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SearchBeneficiaryTable model = new SearchBeneficiaryTable
                            {
                                Nombre = Convert.ToString(reader["Nombre"]),
                                Apellido = Convert.ToString(reader["Apellido"]),
                                NombreCompleto = Convert.ToString(reader["NombreCompleto"]),
                                DUI = Convert.ToString(reader["Dui"]),
                                Edad = Convert.ToString(reader["Edad"]),
                                Sexo = Convert.ToString(reader["Sexo"]),
                                Programa = Convert.ToString(reader["NombrePrograma"]),
                                carrera = Convert.ToString(reader["CarreraCursoGrado"]),
                                correo = Convert.ToString(reader["Correo"]),
                                telefono1 = Convert.ToString(reader["Telefono1"]),
                                telefono2 = Convert.ToString(reader["Telefono2"])
                            };
                            personas.Add(model);
                        }
                    }
                }

                // Aplica filtros a la lista de personas si es necesario
                if (param.dui != null)
                {
                    personas = personas.Where(x => x.DUI.Contains(param.dui, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                if (param.nombre != null)
                {
                    personas = personas.Where(x => x.Nombre.Contains(param.nombre, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                if (param.aperllido != null)
                {
                    personas = personas.Where(x => x.Apellido.Contains(param.aperllido, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                if (param.telefono1 != null)
                {
                    personas = personas.Where(x => x.telefono1.Contains(param.telefono1, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                if (param.telefono2 != null)
                {
                    personas = personas.Where(x => x.telefono2.Contains(param.telefono2, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                if (param.correo != null)
                {
                    personas = personas.Where(x => x.correo.Contains(param.correo, StringComparison.OrdinalIgnoreCase)).ToList();
                }


                param.count = personas.Count();

                return personas.Skip(skip)
                                    .Take(length)
                                    .ToList();

            }
            catch (Exception ex)
            {
                // Maneja la excepción adecuadamente
                Console.WriteLine("Se produjo una excepción: " + ex.Message);
                throw; // Lanza la excepción nuevamente para que sea manejada en un nivel superior
            }
        }

        public List<PersonTableVM> RepPromovidos()
        {
            try
            {
                List<PersonTableVM> personas = new List<PersonTableVM>();

                // Construye la consulta SQL
                string query = @"SELECT
    x.PIdOim,
    x.NumeroInscripciones,
    x.NombreCompleto,
    x.IdPrograma,
    p.Nombre AS NombrePrograma,
    CASE 
        WHEN x.IdPrograma IN (3, 6) THEN (SELECT g.Nombre FROM [dbo].[Grados] g WHERE g.IdGrado = x.CarreraCursoGrado)
        ELSE c.Nombre
    END AS NombreCarrera,
    x.Telefono1,
    x.Telefono2,
    x.Sexo,
    s.Nombre AS NombreSexo,
    CASE WHEN x.LGBTIQ = 1 THEN 'sí' ELSE 'no' END AS LGBTIQ,
    x.FechaNacimiento,
    x.Edad,
    x.Discapacidad,
    x.VictimaViolencia,
    x.MigranteRetornado AS MigranteRetornadoID,
    CASE WHEN x.MigranteRetornado = 1 THEN 'sí' ELSE 'no' END AS MigranteRetornado,
    x.PiensaMigrar,
    x.FamiliaresMigrantes,
    x.FamiliaresRetornados,
    x.Empleo,
    x.Dui,
    x.Nie,
    x.Correo,
    x.Refiere,
    r.Nombre AS NombreRefiere,
    x.Departamento,
    d.Nombre AS NombreDepartamento,
    x.Municipio,
    m.Nombre AS NombreMunicipio,
    x.IdTipoMatricula,
    tm.Nombre AS NombreMatricula,
    x.Year,
    x.Cohorte,
    co.Nombre AS NombreCohorte,
    x.p_socio,
    si.Nombre AS NombreSocio,
    x.p_sede,
    se.Nombre AS NombreSede,
    x.UltimoGradoAprobado,
    x.NivelAcademico,
    educacion.d_estado,
    e.Nombre AS NombreEstado,
    x.IdZona,
    x.CarreraCursoGrado,
    educacion.year,
    x.IdProyecto
                                FROM 
                                    Persona AS x
                                LEFT JOIN 
                                    Programa AS p ON x.IdPrograma = p.IdPrograma
                                LEFT JOIN 
                                    CatCarrera AS c ON x.CarreraCursoGrado = c.IdCatCarrera 
                                LEFT JOIN 
                                    Sexo AS s ON x.Sexo = s.IdSexo 
                                LEFT JOIN 
                                    Refiere AS r ON x.Refiere = r.IdRefiere
                                LEFT JOIN 
                                    Departamento AS d ON x.Departamento = d.IdDepartamento
                                LEFT JOIN 
                                    Municipio AS m ON x.Municipio = m.IdMunicipio
                                LEFT JOIN 
                                    TipoMatricula AS tm ON x.IdTipoMatricula = tm.IdTipoMatricula
                                LEFT JOIN 
                                    Cohorte AS co ON x.Cohorte = co.IdCohorte
                                LEFT JOIN 
                                    SocioImplementador AS si ON x.p_socio = si.IdImplementador
                                LEFT JOIN 
                                    CatSede AS se ON x.p_sede = se.IdCatSede
                                LEFT JOIN 
                                     CargaEducacion AS educacion ON x.PIdOim = educacion.p_id_oim
                                LEFT JOIN 
                                    EstadoPersona AS e ON e.IdEstadoPersona = educacion.d_estado
                                WHERE 
                                    educacion.d_estado = 5
                                ORDER BY 
                                    educacion.r_fechaini DESC;";

                // Crea la conexión y ejecuta la consulta
                using (SqlConnection connection = new SqlConnection(_ctx.Database.GetConnectionString()))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PersonTableVM model = new PersonTableVM
                            {
                                PIdOim = Convert.ToString(reader["PIdOim"]),
                                NumeroInscripciones = Convert.ToInt32(reader["NumeroInscripciones"]),
                                NombreCompleto = Convert.ToString(reader["NombreCompleto"]),
                                programa = Convert.ToString(reader["NombrePrograma"]),
                                carrera = Convert.ToString(reader["NombreCarrera"]),
                                Telefono1 = Convert.ToString(reader["Telefono1"]),
                                Telefono2 = Convert.ToString(reader["Telefono2"]),
                                SexoNombre = Convert.ToString(reader["NombreSexo"]),
                                LGBTIQ = Convert.ToString(reader["LGBTIQ"]),
                                FechaNacimiento = Convert.ToString(reader["FechaNacimiento"]),
                                Edad = Convert.ToInt32(reader["Edad"]),
                                Discapacidad = Convert.ToString(reader["Discapacidad"]),
                                VictimaViolencia = Convert.ToString(reader["VictimaViolencia"]),
                                MigranteRetornadoNombre = Convert.ToString(reader["MigranteRetornado"]),
                                PiensaMigrar = Convert.ToString(reader["PiensaMigrar"]),
                                FamiliaresMigrantes = Convert.ToString(reader["FamiliaresMigrantes"]),
                                FamiliaresRetornados = Convert.ToString(reader["FamiliaresRetornados"]),
                                Empleo = Convert.ToString(reader["Empleo"]),
                                Dui = Convert.ToString(reader["Dui"]),
                                Nie = Convert.ToString(reader["Nie"]),
                                Correo = Convert.ToString(reader["Correo"]),
                                RefiereNombre = Convert.ToString(reader["NombreRefiere"]),
                                DepartamentoNombre = Convert.ToString(reader["NombreDepartamento"]),
                                MunicipioNombre = Convert.ToString(reader["NombreMunicipio"]),
                                tipomatricula = Convert.ToString(reader["NombreMatricula"]),
                                Year = Convert.ToInt32(reader["Year"]),
                                CohorteNombre = Convert.ToString(reader["NombreCohorte"]),
                                p_socioNombre = Convert.ToString(reader["NombreSocio"]),
                                p_sedeNombre = Convert.ToString(reader["NombreSede"]),
                                UltimoGradoAprobado = Convert.ToString(reader["UltimoGradoAprobado"]),
                                NivelAcademico = Convert.ToString(reader["NivelAcademico"]),
                                Estado = Convert.ToString(reader["NombreEstado"]),
                                p_socio = reader["p_socio"] != DBNull.Value ? Convert.ToInt32(reader["p_socio"]) : (int?)null,
                                IdZona = reader["IdZona"] != DBNull.Value ? Convert.ToInt32(reader["IdZona"]) : (int?)null,
                                IdPrograma = reader["IdPrograma"] != DBNull.Value ? Convert.ToInt32(reader["IdPrograma"]) : (int?)null,
                                p_sede = reader["p_sede"] != DBNull.Value ? Convert.ToInt32(reader["p_sede"]) : (int?)null,
                                CarreraCursoGrado = reader["CarreraCursoGrado"] != DBNull.Value ? Convert.ToInt32(reader["CarreraCursoGrado"]) : (int?)null,
                                Sexo = reader["Sexo"] != DBNull.Value ? Convert.ToInt32(reader["Sexo"]) : (int?)null,
                                IdTipomatricula = reader["IdTipomatricula"] != DBNull.Value ? Convert.ToInt32(reader["IdTipomatricula"]) : (int?)null,
                                Departamento = reader["Departamento"] != DBNull.Value ? Convert.ToInt32(reader["Departamento"]) : (int?)null,
                                Refiere = reader["Refiere"] != DBNull.Value ? Convert.ToInt32(reader["Refiere"]) : (int?)null,
                                añoestudio = Convert.ToInt32(reader["year"]),
                                Cohorte = reader["Cohorte"] != DBNull.Value ? Convert.ToInt32(reader["Cohorte"]) : (int?)null,
                                proyecto = reader["IdProyecto"] != DBNull.Value ? Convert.ToInt32(reader["IdProyecto"]) : (int?)null,
                                IdEstado = reader["d_estado"] != DBNull.Value ? Convert.ToInt32(reader["d_estado"]) : (int?)null,
                                MigranteRetornado = reader["MigranteRetornadoID"] != DBNull.Value ? Convert.ToInt32(reader["MigranteRetornadoID"]) : (int?)null,

                            };
                            personas.Add(model);
                        }
                    }
                }


                return personas;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Se produjo una excepción: " + ex.Message);
                throw; // Lanza la excepción nuevamente para que sea manejada en un nivel superior
            }
        }
    }
}
