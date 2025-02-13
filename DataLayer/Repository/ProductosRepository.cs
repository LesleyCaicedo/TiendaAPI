using EntityLayer.DTO;
using EntityLayer.Mapper;
using EntityLayer.Models;
using EntityLayer.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Repository
{
    public class ProductosRepository : IProductosRepository
    {
        private readonly TiendaContext _context;
        private readonly ProductoMapper productoMapper = new();

        Response response = new();

        public ProductosRepository(TiendaContext context)
        {
            _context = context;
        }

        public async Task<Response> IngresarProducto(ProductoDTO productoDTO)
        {
            try
            {
                Producto producto2 = new() { 
                    Nombre = productoDTO.Nombre,
                    IdCategoria = productoDTO.IdCategoria,
                    Precio = productoDTO.Precio,
                    Stock = productoDTO.Stock,
                    Detalle = productoDTO.Detalle,
                    Estado = productoDTO.Estado
                };
                //Producto producto = productoMapper.ProductoDTOToProducto(productoDTO);
                await _context.Productos.AddAsync(producto2);

                await _context.SaveChangesAsync();
                response.Code = ResponseType.Success;
                response.Message = "Producto ingresado correctamente";
                response.Data = productoDTO;
            }
            catch (Exception ex)
            {
                response.Code = ResponseType.Error;
                response.Message = "No se pudo registrar";
                response.Data = ex.Data;
            }
            return response;
        }

        public async Task<Response> ActualizarProducto(ProductoDTO productoDTO)
        {
            try
            {
                Producto producto = await _context.Productos.FindAsync(productoDTO.IdProducto);
                producto.Nombre = string.IsNullOrEmpty(productoDTO.Nombre)? producto.Nombre: productoDTO.Nombre;
                producto.IdCategoria = productoDTO.IdCategoria;
                producto.Precio = productoDTO.Precio < 0? producto.Precio: productoDTO.Precio;
                producto.Stock = productoDTO.Stock < 0? producto.Stock: productoDTO.Stock;
                producto.Detalle = productoDTO.Detalle;
                producto.Estado = productoDTO.Estado;

                await _context.SaveChangesAsync();
                response.Code = ResponseType.Success;
                response.Message = "Producto Actualizado correctamente";
                response.Data = productoDTO;
            }
            catch(Exception ex)
            {
                response.Code = ResponseType.Error;
                response.Message = "No se pudo Actualizar";
                response.Data = ex.Data;
            }
            return response;
        }

        public async Task<Response> ObtenerProductos()
        {
            try
            {
                List<ProductoDTO> productosDTO = productoMapper.ProductosToProductosDTO(await _context.Productos.ToListAsync());

                response.Code = ResponseType.Success;
                response.Message = "Productos Encontrados";
                response.Data = productosDTO;
            }
            catch (Exception ex)
            {
                response.Code = ResponseType.Error;
                response.Message = "No hay productos";
                response.Data = ex.Data;
            }
            return response;

        }
    
        public async Task<Response> ObtenerProductoID(int productoID)
        {
            try
            {
                ProductoDTO productoDTO = productoMapper.ProductoToProductoDTO(await _context.Productos.FindAsync(productoID));
                response.Code = ResponseType.Success;
                response.Message = "Producto Actualizado correctamente";
                response.Data = productoDTO;
            }
            catch (Exception ex) 
            {
                response.Code = ResponseType.Error;
                response.Message = "Producto no existe";
                response.Data = ex.Data;
            }
            return response;
        }

        public async Task<Response> EliminarProductoID(int productoID)
        {
            try
            {
                _context.Productos.Remove(await _context.Productos.FindAsync(productoID));

                await _context.SaveChangesAsync();
                response.Code = ResponseType.Success;
                response.Message = "Producto eliminado correctamente";
                response.Data = null;
            }
            catch (Exception ex)
            {
                response.Code = ResponseType.Error;
                response.Message = "No se pudo eliminar el producto";
                response.Data = ex.Data;
            }
            return response;
        }
    
        
    }
}
