using System;
using System.Collections.Generic;

namespace BECASLC
{
    public partial class CargaSeguimientoPracticasPr
    {
        public int IdSeguimientoPracticasPr { get; set; }
        public string? PId { get; set; }
        public string? PpEmpresa { get; set; }
        public string? PpCargo { get; set; }
        public string? PpDocenteAsign { get; set; }
        public string? PpGestion { get; set; }
        public string? PpMontoRemuneracion { get; set; }
        public string? PpPosibilidadContratacion { get; set; }
        public int? IdCarga { get; set; }
        public int? Año { get; set; }
        public string? Mes { get; set; }
    }
}
