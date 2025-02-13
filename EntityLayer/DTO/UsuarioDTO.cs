using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTO
{
    public class UsuarioDTO
    {
        public string Nombre { get; set; }

        public string Correo { get; set; }

        public string Direccion { get; set; }

        public string Ci { get; set; }
        public string Clave { get; set; }
    }
}
