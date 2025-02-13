using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTO
{
    public class ProductoDTO
    {
        public int IdProducto { get; set; }

        public string Nombre { get; set; }

        public decimal Precio { get; set; }

        public int Stock { get; set; }

        public string? Detalle { get; set; }

        public bool Estado { get; set; }

        public int IdCategoria { get; set; }
    }
}
