using DataLayer.Repository;
using EntityLayer.DTO;
using EntityLayer.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class SesionService : ISesionService
    {
        private readonly ISesionRepository _sesionRepository;
        Response response = new();

        public SesionService(ISesionRepository sesionRepository)
        {
            _sesionRepository = sesionRepository;
        }

        public async Task<Response> RegistroUsuario(UsuarioDTO usuarioDTO)
        {
            response = await _sesionRepository.RegistroUsuario(usuarioDTO);
            return response;
        }

        public async Task<Response> MostrarCategoria()
        {
            response = await _sesionRepository.MostrarCategoria();
            return response;
        }
    }
}
