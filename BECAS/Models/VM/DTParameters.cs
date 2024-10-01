namespace BECAS.Models.VM
{
    public class DTParameters
    {
        public DTParameters()
        {
            AdditionalParameters = new Dictionary<string, string>();
        }
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public Search Search { get; set; }
        public List<string> socios { get; set; }
        public List<string> DropZona { get; set; }
        public List<string> IdPrograma { get; set; }
        public List<string> Sedes { get; set; }
        public List<string> Carreras { get; set; }
        public List<string> Sexos { get; set; }
        public List<string> TipoMatricula { get; set; }
        public List<string> Departamento { get; set; }
        public List<string> Refiere { get; set; }
        public List<string> Year1 { get; set; }
        public List<string> AEstudio { get; set; }
        public List<string> Cohorte { get; set; }
        public List<string> Proyecto { get; set; }
        public List<string> Estado { get; set; }
        public List<string> Retornado { get; set; }
        public IDictionary<string, string> AdditionalParameters { get; set; }
        public int? count { get; set; }
    }
    public class Search
    {
        public string Value { get; set; }
        public bool Regex { get; set; }
    }
}
