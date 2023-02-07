using System;
using System.Collections.Generic;

namespace BECASLC
{
    public partial class Zona
    {
        public int IdZona { get; set; }
        public int? IdSocio { get; set; }
        public string? Nombre { get; set; }
        public bool? Activo { get; set; }

        public LinkedList<Sede> sedes { get; set; }
    }
}
