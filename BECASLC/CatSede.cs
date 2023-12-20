using System;
using System.Collections.Generic;

namespace BECASLC
{
    public partial class CatSede
    {
        public int IdCatSede { get; set; }
        public string? Nombre { get; set; }
        public bool? Activo { get; set; }
        public int? IdDepartamento { get; set; }
        public int? IdMunicipio { get; set; }
        public string? Direccion { get; set; }
        public string? Latitud { get; set; }
        public string? Longitud { get; set; }
    }
}
