namespace BECAS.Models.VM
{
    public class CarreraListaCm
    {
        public int idCarrera { get; set; }
        public string? nombre { get; set; }
        public int? idSede { get; set; }
        public bool? activo { get; set; }
        public int? cohorte { get; set; }
        public bool IsCarrera { get; set; }
    }
}
