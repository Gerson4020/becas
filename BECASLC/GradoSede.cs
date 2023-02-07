using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BECASLC
{
    public partial class GradoSede
    {
        public int IdGradoSede { get; set; }
        [ForeignKey("grado")]
        public int? IdGrado { get; set; }
        [ForeignKey("sede")]
        public int? IdSede { get; set; }
        public bool? Activo { get; set; }

        public Grado grado { get; set; }
        public CatSede sede { get; set; }
    }
}
