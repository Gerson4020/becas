using BECASLC;
namespace BECAS.Models.VM
{
    public class CargaSeguimientoPasantiaTable
    {
        public int IdSeguimientoPasantias { get; set; }
        public string? PId { get; set; }
        public string? PasEmpresa { get; set; }
        public string? PasEntrevista { get; set; }
        public string? PasPruebas { get; set; }
        public string? PasContratacion { get; set; }
        public string? PasCargo { get; set; }
        public string? PasFechaContratacion { get; set; }
        public string? PasMontoRemuneracion { get; set; }
        public int? IdCarga { get; set; }
        public int? Año { get; set; }
        public string? Mes { get; set; }
        public Persona? persona { get; set; }
    }
}
