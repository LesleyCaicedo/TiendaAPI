using Riok.Mapperly.Abstractions;
using EntityLayer.DTO;
using EntityLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Mapper
{
    [Mapper]
    public partial class ProductoMapper
    {
        public partial ProductoDTO ProductoToProductoDTO(Producto producto);
        public partial Producto ProductoDTOToProducto(ProductoDTO productoDTO);
        public partial List<ProductoDTO> ProductosToProductosDTO(List<Producto> productos);
    }
}
