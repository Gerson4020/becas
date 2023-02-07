using System;
using System.Collections.Generic;

namespace BECASLC
{
    public partial class Carga
    {
        public int IdCarga { get; set; }
        public DateTime? FechaCarga { get; set; }
        public int? RAño { get; set; }
        public string? RMes { get; set; }
        public DateTime? RFechaini { get; set; }
        public DateTime? RFechafin { get; set; }
        public string? PIdOim { get; set; }
        public int? TotalEducacion { get; set; }
        public int? TotalEvaPsicosocial { get; set; }
        public int? TotalSegPsicosocial { get; set; }
        public int? TotalSeguimientoPracticasPr { get; set; }
        public int? TotalSeguimientoPasantias { get; set; }
        public int? TotalSeguimientoAutoempleo { get; set; }
    }
}
