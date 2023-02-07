using BECASLC;
namespace BECAS.Models.VM
{
    public class PersonTableVM
    {
        public int IdPersona { get; set; }
        public DateTime? FechaEntrevista { get; set; }
        public string? Id { get; set; }
        public string? PIdOim { get; set; }
        public TipoMatricula? TipoMatricula { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? NombreCompleto { get; set; }
        public string? UltimoGradoAprobado { get; set; }
        public string? Telefono1 { get; set; }
        public string? Telefono2 { get; set; }
        public Sexo? Sexo { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int? Edad { get; set; }
        public string? Discapacidad { get; set; }
        public string? VictimaViolencia { get; set; }
        public string? MigranteRetornado { get; set; }
        public string? PiensaMigrar { get; set; }
        public string? FamiliaresMigrantes { get; set; }
        public string? FamiliaresRetornados { get; set; }
        public string? Empleo { get; set; }
        public string? Dui { get; set; }
        public string? Nie { get; set; }
        public string? Correo { get; set; }
        public Refiere? Refiere { get; set; }
        public Departamento? Departamento { get; set; }
        public Programa? Programa { get; set; }
        public string? Cohorte { get; set; }
        public CatSede? Sede { get; set; }
        public SocioImplementador? SocioIm { get; set; }
        public Zona? Zona { get; set; }
        public string? EstadoInscripcion { get; set; }
        public string? EstadoMf { get; set; }
        public CatCarrera? CarreraCursoGrado { get; set; }
        public string? EstadoPersona { get; set; }
        public int? IdCarga { get; set; }
    }
}
