using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTO
{
    public class VentaDTO
    {
        public int IdVenta { get; set; }

        public int IdProducto { get; set; }

        public int IdUsuario { get; set; }

        public string? Detalle { get; set; }
    }
}
