using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BECASLC
{
    public partial class CargaEducacion
    {
        public int IdCargaEducacion { get; set; }
        public string? PIdOim { get; set; }
        public DateTime? DFechaReasg { get; set; }
        public string? DEstado { get; set; }
        public DateTime? DFechades { get; set; }
        public string? DMotivodesercion { get; set; }
        public int? IDiasAsistenciaEstablecidos { get; set; }
        public int? IDiasAsistenciaEfectivos { get; set; }
        public string? IMotivoInasistencia { get; set; }
        public int? IModulosInscritos { get; set; }
        public int? IModulosAprobados { get; set; }
        public int? IModulosReprobados { get; set; }
        public string? ICausaReprobacion { get; set; }
        public int? IdCarga { get; set; }
        public int? RAño { get; set; }
        public string? RMes { get; set; }

    }
}
