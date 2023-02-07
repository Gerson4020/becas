using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BECASLC
{
    public partial class Carrera
    {
        public int IdCarrera { get; set; }
        public string? Nombre { get; set; }
        public int? IdSede { get; set; }
        public bool? Activo { get; set; }
        public int? Cohorte { get; set; }
        [ForeignKey("carrera")]
        public int? IdCatCarrera { get; set; }

        public CatCarrera carrera { get; set; }
    }
}
