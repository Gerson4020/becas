using BECASLC;
namespace BECAS.Models.VM
{
    public class SeguimientoPsicosocialTable
    {
        public int IdSeguimientoPsicosocial { get; set; }
        public string? PId { get; set; }
        public string? SegMotivo { get; set; }
        public string? SegEstado { get; set; }
        public string? SegMedida { get; set; }
        public string? SegAlertaDesercion { get; set; }
        public int? IdCarga { get; set; }
        public int? Año { get; set; }
        public string? Mes { get; set; }
        public Persona? persona { get; set; }
    }
}
