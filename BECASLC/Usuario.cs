using System;
using System.Collections.Generic;

namespace BECASLC
{
    public partial class Usuario
    {
        public int IdUsuario { get; set; }
        public int? IdRol { get; set; }
        public string? Nombre { get; set; }
        public string? Password { get; set; }
        public bool? Estado { get; set; }
    }
}
