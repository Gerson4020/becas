namespace BECAS.Models
{
    public class DTSearchBeneficiary
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }

        public int? count { get; set; }

        public SearchBeneficiary Search { get; set; }

        public string? dui { get; set; }
        public string? nombre { get; set; }
        public string? aperllido { get; set; }
        public string? telefono1 { get; set; }
        public string? telefono2 { get; set; }
        public string? correo { get; set; }
    }
    public class SearchBeneficiary
    {
        public string Value { get; set; }
        public bool Regex { get; set; }
    }
}
