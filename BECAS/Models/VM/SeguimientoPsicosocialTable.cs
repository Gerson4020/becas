using BECASLC;
namespace BECAS.Models.VM
{
    public class SeguimientoPsicosocialTable
    {
        public string? Id { get; set; }
        public string? Nombre { get; set; }
        public string? SegMotivo { get; set; }
        public string? SegEstado { get; set; }
        public string? SegMedida { get; set; }
        public DateTime? fecha_atencion { get; set; }
        public string? SegAlertaDesercion { get; set; }
        public string? EpAlertaDesercion { get; set; }
        public int? Año { get; set; }
        public string? Mes { get; set; }
        public DateTime? fechainicio { get; set; }
        public int? idsocio { get; set; }
        public int? idzona { get; set; }
        public int? idprograma { get; set; }
        public int? idsede { get; set; }
        public int? idsexo { get; set; }
        public int? idTipomatricula { get; set; }
        public int? idDepartament { get; set; }
        public int? refiere { get; set; }
        public int? year1 { get; set; }
        public string? nombreSocio { get; set; }
        public string? nombreSede { get; set; }
        public string? tipoMatricula { get; set; }
        public string? nombreCarrera { get; set; }
        public int? cohorte { get; set; }
        public string? nombreCohorte { get; set; }
        public int? idCarreta { get; set; }
    }
}
