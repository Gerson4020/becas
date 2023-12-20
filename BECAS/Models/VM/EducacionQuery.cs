using BECASLC;

namespace BECAS.Models.VM
{
    public class EducacionQuery
    {
        public SocioImplementador p_socio { get; set; }
        public CatSede p_sede { get; set; }
        public Programa programa { get; set; }
        public CatCarrera CarreraCursoGrado { get; set; }
        public TipoMatricula p_matricula { get; set; }
        public string Year { get; set; }
        public int? Cohorte { get; set; }
        public Cohorte cohorte { get; set; }
        public string? Sector { get; set; }
        public string? EstadoMF { get; set; }
        public string? DEstado { get; set; }
    }
}
