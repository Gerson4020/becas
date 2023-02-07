using BECASLC;
namespace BECAS.Models.VM
{
    public class DetallePersonaVM
    {
        public List<CargaEducacion>? cargaEducacions { get; set; }
        public List<CargaEvaluacionPsicosocial>? cargaEvaluacionPsicosocial { get; set; }
        public List<CargaSeguimientoPsicosocial>? cargaSeguimientoPsicosocial { get; set; }
        public List<CargaSeguimientoPracticasPr>? cargaSeguimientoPracticasPr { get; set; }
        public List<CargaEstipendio>? cargaEstipendio { get; set; }
        public List<CargaSeguimientoPasantia>? pasantias { get; set; }
        public List<CargaSeguimientoAutoempleo>? empleo { get; set; }

    }
}
