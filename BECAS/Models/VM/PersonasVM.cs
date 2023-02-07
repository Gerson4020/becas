using BECASLC;
namespace BECAS.Models.VM
{
    public class PersonasVM
    {
        public List<PersonTableVM>? Personas { get; set; }
        public List<Programa>? Programa { get; set; }
        public List<SocioImplementador>? Socios { get; set; }
        public List<Sede>? LIstaSedes { get; set; }
        public List<Carrera>? Carreras { get; set; }
        public List<Sexo>? sexos { get; set; }
        public List<TipoMatricula>? tipomatricula { get; set; }
        public List<Departamento>? departamentos { get; set; }
        public List<Refiere>? dropRefiereCM { get; set; }

    }
}
