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
    public class SesionRepository : ISesionRepository
    {
        Response response = new Response();

        private readonly TiendaContext _context;
        private readonly UsuarioMapper _usuarioMapper = new();

        public SesionRepository(TiendaContext context)
        {
            _context = context;
        }

        public async Task<Response> RegistroUsuario(UsuarioDTO usuarioDTO)
        {

            try
            {

                string mensaje = await ValidarUsuario(usuarioDTO.Ci, usuarioDTO.Correo);

                if (string.IsNullOrEmpty(mensaje))
                {
                    Usuario nuevousuario = new()
                    {
                        Nombre = usuarioDTO.Nombre,
                        Ci = usuarioDTO.Ci,
                        Correo = usuarioDTO.Correo,
                        Direccion = usuarioDTO.Direccion,
                        Clave = usuarioDTO.Clave,
                        Estado = true,
                        Rol = "User"
                    };
                    await _context.Usuarios.AddAsync(nuevousuario);

                    await _context.SaveChangesAsync(); 
                }

                if (string.IsNullOrEmpty(mensaje))
                {
                    response.Code = ResponseType.Success;
                    response.Message = "Usuario registrado correctamente";
                    response.Data = null;
                }
                else
                {
                    response.Code = ResponseType.Error;
                    response.Message = mensaje +" Esta repetido.";
                    response.Data = null;
                }

            }
            catch (Exception ex)
            {
                response.Code = ResponseType.Error;
                response.Message = "No se pudo registrar";
                response.Data = ex.Data;

            }

            return response;
        }


        public async Task<string> ValidarUsuario(string Cedula, string correo)
        {
            string resultado = string.Empty;
            
            bool fallaCedula = await _context.Usuarios.AnyAsync(x => x.Ci == Cedula);
            bool fallaCorreo = await _context.Usuarios.AnyAsync(x => x.Correo == correo);

            if (fallaCedula && fallaCorreo)
            {
                resultado = "cedula y correo";
            }
            else if (fallaCorreo)
            {
                resultado = "correo";

            }
            else if (fallaCedula)
            {
                resultado = "cedula";
            }

            return resultado;
        }


        public async Task<Response> MostrarCategoria()
        {
            List<CategoriaProductoDTO> categoriaconProductos = new List<CategoriaProductoDTO>();

            categoriaconProductos = await _context.Categoria.Include(p => p.Productos).Select(c => new CategoriaProductoDTO
            {
                Nombrecategoria = c.Nombre,
                Nombreproducto = _context.Productos.Select(p => p.Nombre).ToList(),
            }).ToListAsync();

            response.Code = ResponseType.Success;
            response.Message = "Lista de productos por categorias";
            response.Data = categoriaconProductos;

            return response;
        }
    }
}
