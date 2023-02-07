using BECASLC;
namespace BECAS.Models.VM
{
    public class SeguimientoAutoempleoTable
    {
        public int IdSeguimientoAutoempleo { get; set; }
        public string? PId { get; set; }
        public string? AutoempEmpresa { get; set; }
        public string? AutoempTipoCapital { get; set; }
        public string? AutoempEstado { get; set; }
        public string? AutoempTipoFinanciamiento { get; set; }
        public string? AutoempTipoEmpresa { get; set; }
        public string? AutoempTipoEmpresaOtro { get; set; }
        public string? AutoempPlanNegocios { get; set; }
        public string? AutoempRegistro { get; set; }
        public DateTime? AutoempFechaInicio { get; set; }
        public int? IdCarga { get; set; }
        public Carga? carga { get; set; }
        public int? Año { get; set; }
        public string? Mes { get; set; }
        public Persona? persona { get; set; }
    }
}
