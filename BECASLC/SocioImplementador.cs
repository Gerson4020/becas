using System;
using System.Collections.Generic;

namespace BECASLC
{
    public partial class SocioImplementador
    {
        public int IdImplementador { get; set; }
        public string? Nombre { get; set; }
        public bool? Activo { get; set; }

        public List<Sede> Sedes { get; set; }
    }
}
