using BECASLC;
namespace BECAS.Models.VM
{
    public class SeguimientoPracticasPrTable
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
        public Carga? carga { get; set; }
        public int? Año { get; set; }
        public string? Mes { get; set; }
        public Persona? persona { get; set; }
    }
}
