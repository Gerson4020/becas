using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BECASLC
{
    public partial class CargaEducacion
    {
        public int IdCargaEducacion { get; set; }
        public string? PIdOim { get; set; }
        [ForeignKey("catAño")]
        public int? RAño { get; set; }
        public string? RMes { get; set; }
        public DateTime? r_fechaini { get; set; }
        public DateTime? r_fechafin { get; set; }
        [ForeignKey("socio")]
        public int? p_socio { get; set; }
        [ForeignKey("sede")]
        public int? p_sede { get; set; }
        [ForeignKey("programa")]
        public int? p_tipobeca { get; set; }
        [ForeignKey("carrera")]
        public int? CarreraCursoGrado { get; set; }
        [ForeignKey("tipoMatricula")]
        public int? p_matricula { get; set; }
        public int? Year { get; set; }
        [ForeignKey("cohorte")]
        public int? Cohorte { get; set; }
        [ForeignKey("sector")]
        public int? Sector { get; set; }
        public string? EstadoMF { get; set; }
        public DateTime? DFechaReasg { get; set; }
        [ForeignKey("estado")]
        public int? DEstado { get; set; }
        public DateTime? DFechades { get; set; }
        public string? DMotivodesercion { get; set; }
        public int? IDiasAsistenciaEstablecidos { get; set; }
        public int? IDiasAsistenciaEfectivos { get; set; }
        public string? i_proc_asistencia { get; set; }
        public string? IMotivoInasistencia { get; set; }
        public int? IModulosInscritos { get; set; }
        public int? IModulosAprobados { get; set; }
        public string? prueba_realizada { get; set; }
        public int? IModulosReprobados { get; set; }
        public string? ICausaReprobacion { get; set; }
        public int? IdZona { get; set; }

        public int? IdCarga { get; set; }
        public virtual CatAño? catAño { get; set; }
        public virtual SocioImplementador socio { get; set; }
        public virtual CatSede sede { get; set; }
        public virtual Programa programa { get; set; }
        public virtual CatCarrera carrera { get; set; }
        public virtual TipoMatricula tipoMatricula { get; set; }
        public virtual Cohorte cohorte { get; set; }
        public virtual Sector sector { get; set; }
        public virtual EstadoPersona estado { get; set; }
        
    }
}
