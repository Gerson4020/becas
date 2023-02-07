using System;
using System.Collections.Generic;

namespace BECASLC
{
    public partial class CargaEstipendio
    {
        public int IdCargaEstipendios { get; set; }
        public string? PId { get; set; }
        public decimal? AlimEfectivo { get; set; }
        public decimal? AlimMontoEfectivo { get; set; }
        public decimal? AlimDiasPresencialesEfectivo { get; set; }
        public decimal? AlimSubtotalEfectivo { get; set; }
        public decimal? AlimTransferencia { get; set; }
        public decimal? AlimMontoTransferencia { get; set; }
        public decimal? AlimDiasPresencialesTransferencia { get; set; }
        public decimal? AlimSubtotalTransferencia { get; set; }
        public decimal? AlimEspecie { get; set; }
        public decimal? AlimMontoEspecie { get; set; }
        public decimal? AlimDiasPresencialesEspecie { get; set; }
        public decimal? AlimSubtotalEspecie { get; set; }
        public decimal? AlimMontoTotal { get; set; }
        public decimal? TranspEfectivo { get; set; }
        public decimal? TranspMontoEfectivo { get; set; }
        public decimal? TranspDiasPresencialesEfectivo { get; set; }
        public decimal? TranspSubtotalEfectivo { get; set; }
        public decimal? TranspTransferencia { get; set; }
        public decimal? TranspTarifaDiferenciada { get; set; }
        public decimal? TranspMontoTransferencia { get; set; }
        public decimal? TranspDiasPresencialesTransferencia { get; set; }
        public decimal? TranspSubtotalTransferencia { get; set; }
        public decimal? TranspMontoTotal { get; set; }
        public decimal? ConecEfectivo { get; set; }
        public decimal? ConecMontoEfectivo { get; set; }
        public decimal? ConecDiasPresencialesEfectivo { get; set; }
        public decimal? ConecSubtotalEfectivo { get; set; }
        public decimal? ConecTransferencia { get; set; }
        public decimal? ConecMontoTransferencia { get; set; }
        public decimal? ConecDiasPresencialesTransferencia { get; set; }
        public decimal? ConecSubtotalTransferencia { get; set; }
        public decimal? ConecMontoTotal { get; set; }
        public decimal? EstipendioTotal { get; set; }
        public int? IdCarga { get; set; }
        public int? Año { get; set; }
        public string? Mes { get; set; }
    }
}
