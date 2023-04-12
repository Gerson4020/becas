using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BECASLC
{
    public partial class Persona
    {
        public int IdPersona { get; set; }
        public DateTime? FechaEntrevista { get; set; }
        public string? Id { get; set; }
        public string? PIdOim { get; set; }
        [ForeignKey("matricula")]
        public int? TipoMatricula { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? NombreCompleto { get; set; }
        public string? UltimoGradoAprobado { get; set; }
        public string? Telefono1 { get; set; }
        public string? Telefono2 { get; set; }
        [ForeignKey("sexo")]
        public int? Sexo { get; set; }
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
        [ForeignKey("refiere")]
        public int? Refiere { get; set; }
        public int? Departamento { get; set; }
        [ForeignKey("programa")]
        public int? Programa { get; set; }
        [ForeignKey("cohorte")]
        public int? Cohorte { get; set; }
        [ForeignKey("sede")]
        public int? Sede { get; set; }
        [ForeignKey("socio")]
        public int? SocioIm { get; set; }
        [ForeignKey("zona")]
        public int? Zona { get; set; }
        public string? EstadoInscripcion { get; set; }
        public string? EstadoMf { get; set; }
        [ForeignKey("carrera")]
        public int? CarreraCursoGrado { get; set; }
        public string? EstadoPersona { get; set; }
        public int? IdCarga { get; set; }
        public string? CartaCompromiso { get; set; }

        public Programa programa { get; set; }
        public Sexo sexo { get; set; }
        public SocioImplementador socio { get; set; }
        public TipoMatricula matricula { get; set; }
        public CatCarrera carrera { get; set; }
        public CatSede sede { get; set; }
        public Refiere refiere { get; set; }
        public Zona zona { get; set; }
        public Cohorte cohorte { get; set; }
    }
}
