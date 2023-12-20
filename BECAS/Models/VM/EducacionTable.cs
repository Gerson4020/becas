using BECASLC;
namespace BECAS.Models.VM
{
    public class EducacionTable
    {
        public string? PIdOim { get; set; }
        public DateTime? DFechaReasg { get; set; }
        public string? DEstado { get; set; }
        public DateTime? DFechades { get; set; }
        public int? IDiasAsistenciaEstablecidos { get; set; }
        public int? IDiasAsistenciaEfectivos { get; set; }
        public string? IMotivoInasistencia { get; set; }
        public int? IModulosInscritos { get; set; }
        public int? IModulosAprobados { get; set; }
        public int? IModulosReprobados { get; set; }
        public string? ICausaReprobacion { get; set; }
        public string? nombre { get; set; }
        public int? p_socio { get; set; }
        public string? socio { get; set; }
        public int? p_sede { get; set; }
        public string? sede { get; set; }
        public int? CarreraCursoGrado { get; set; }
        public string? carrera { get; set; }
        public int? sexoID { get; set; }
        public string? sexoNombre { get; set; }
        public int? departamento { get; set; }
        public int? refiere { get; set; }
        public string? refiereNombre { get; set; }
        public int? programa { get; set; }
        public string? programaNombre { get; set; }
        public int? zona { get; set; }
        public int? RAño { get; set; }
        public string? RMes { get; set; }
        public int? Cohorte { get; set; }
        public string? CohorteNombre { get; set; }
        public int? year { get; set; }   
        public string? tipoMatricula { get; set; }
        public string? motivodesercion { get; set; }
        public string? pocentajeasistencia { get; set; }
        public DateTime? fechainicio { get; set; }
        public int? EstadoPersona { get; set; }
        public int? p_matricula { get; set; }
    }
}
