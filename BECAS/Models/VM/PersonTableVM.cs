using BECASLC;
namespace BECAS.Models.VM
{
    public class PersonTableVM
    {
        public int IdPersona { get; set; }
        public DateTime? FechaEntrevista { get; set; }
        public string? Id { get; set; }
        public string? PIdOim { get; set; }
        public int? NumeroInscripciones { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? NombreCompleto { get; set; }
        public string? UltimoGradoAprobado { get; set; }
        public string? NivelAcademico { get; set; }
        public string? Telefono1 { get; set; }
        public string? Telefono2 { get; set; }
        public int? Sexo { get; set; }
        public bool? LGBTIQ { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int? Edad { get; set; }
        public string? Discapacidad { get; set; }
        public string? VictimaViolencia { get; set; }
        public int? MigranteRetornado { get; set; }
        public string? PiensaMigrar { get; set; }
        public string? FamiliaresMigrantes { get; set; }
        public string? FamiliaresRetornados { get; set; }
        public string? Empleo { get; set; }
        public string? Dui { get; set; }
        public string? Nie { get; set; }
        public string? Correo { get; set; }
        public int? Refiere { get; set; }
        public string? RefiereNombre { get; set; }
        public int? Departamento { get; set; }
        public string? DepartamentoNombre { get; set; }
        public int? Municipio { get; set; }
        public string? MunicipioNombre { get; set; }
        public string? EstadoInscripcion { get; set; }
        public string? MedioVerificacion { get; set; }
        public int? IdCarga { get; set; }

        public int? IdPrograma { get; set; }
        public int? Year { get; set; }
        public int? Cohorte { get; set; }
        public int? p_socio { get; set; }
        public int? IdZona { get; set; }
        public int? p_sede { get; set; }
        public string? EstadoMF { get; set; }
        public int? CarreraCursoGrado { get; set; }
        public string? grado { get; set; }
        public int? Sector { get; set; }
        public string? programa { get; set; }
        public string? carrera { get; set; }
        public string? tipomatricula { get; set; }
        public int? añoestudio { get; set; }
        public string? CohorteNombre { get; set; }
        public string? p_socioNombre { get; set; }
        public string? p_sedeNombre { get; set; }
        public string? Estado { get; set; }
        public string? SexoNombre { get; set; }

        public CargaEducacion? educacion { get; set; }
    }
}
