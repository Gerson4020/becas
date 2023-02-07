using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BECASLC
{
    public partial class Sede
    {
        public int IdSede { get; set; }
        public bool? Activo { get; set; }
        [ForeignKey("socio")]
        public int? IdSocio { get; set; }
        [ForeignKey("zona")]
        public int? IdZona { get; set; }
        [ForeignKey("programa")]
        public int? IdPrograma { get; set; }
        [ForeignKey("catsede")]
        public int? IdCatSede { get; set; }

        public SocioImplementador socio { get; set; }
        public Zona zona { get; set; }
        public Programa programa { get; set; }
        public CatSede catsede { get; set; }
    }
}
