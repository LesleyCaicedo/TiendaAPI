using EntityLayer.DTO;
using EntityLayer.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Repository
{
    public interface IProductosRepository
    {
        public Task<Response> IngresarProducto(ProductoDTO productoDTO);
        public Task<Response> ActualizarProducto(ProductoDTO productoDTO);
        public Task<Response> ObtenerProductos();
        public Task<Response> ObtenerProductoID(int productoID);
        public Task<Response> EliminarProductoID(int productoID);
    }
}
