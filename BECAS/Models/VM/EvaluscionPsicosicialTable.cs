using BECASLC;
namespace BECAS.Models.VM
{
    public class EvaluscionPsicosicialTable
    {
        public int IdCargaEvaluacionPsicosocial { get; set; }
        public string? PId { get; set; }
        public bool? OvParticipacion { get; set; }
        public string? OvPuntajePret { get; set; }
        public string? OvPuntajePos { get; set; }
        public string? EpInstrumentoRiesgo { get; set; }
        public string? EpVulnerabilidades { get; set; }
        public string? EpAlertaDesercion { get; set; }
        public Persona? persona { get; set; }
        public int? IdCarga { get; set; }
        public int? Año { get; set; }
        public string? Mes { get; set; }
    }
}
