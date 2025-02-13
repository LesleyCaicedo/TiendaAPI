using EntityLayer.DTO;
using EntityLayer.Responses;

namespace BusinessLayer.Services
{
    public interface IProductoService
    {
        Task<Response> IngresarProducto(ProductoDTO productoDTO);
        Task<Response> ActualizarProducto(ProductoDTO productoDTO);
        Task<Response> ObtenerProductos();
        Task<Response> ObtenerProductoID(int productoID);
        Task<Response> EliminarProductoID(int productoID);
        Task<byte[]> GenerarPDF();
        Task<byte[]> GenerarPDF2();
    }
}
