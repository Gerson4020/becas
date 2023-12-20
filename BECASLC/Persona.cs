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
        public int? NumeroInscripciones { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? NombreCompleto { get; set; }
        public string? UltimoGradoAprobado { get; set; }
        public string? NivelAcademico { get; set; } 
        public string? Telefono1 { get; set; }
        public string? Telefono2 { get; set; }
        [ForeignKey("sexo")]
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
        [ForeignKey("refiere")]
        public int? Refiere { get; set; }
        [ForeignKey("departamento")]
        public int? Departamento { get; set; }
        [ForeignKey("municipio")]
        public int? Municipio { get; set; }
        public string? EstadoInscripcion { get; set; }
        public string? MedioVerificacion { get; set; }
        public int? IdCarga { get; set; }
        public int? IdSede { get; set; }

        public int? IdPrograma { get; set; }
        public int? Year { get; set; }
        public int? Cohorte { get; set; }
        public int? p_socio { get; set; }
        public int? IdZona { get; set; }
        public int? p_sede { get; set; }
        public string? EstadoMF { get; set; }
        public int? CarreraCursoGrado { get; set; }
        public int? Sector { get; set; }
        public int? IdTipoMatricula { get; set; }
        public int? IdProyecto { get; set; }


        public Sexo sexo { get; set; }
        public Refiere refiere { get; set; }
        public Departamento departamento { get; set; }
        public Municipio municipio { get; set; }

        //public virtual ICollection<CargaEducacion> CargaEducacions { get; set; }

    }
}
