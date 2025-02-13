using BusinessLayer.Services;
using EntityLayer.DTO;
using EntityLayer.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SesionController : ControllerBase
    {
        private readonly ISesionService _sesionService;
        Response response = new();

        public SesionController(ISesionService sesionService)
        {
            _sesionService = sesionService;
        }

        [HttpPost("[action]")]
        public async Task<Response> RegistroUsuario(UsuarioDTO usuarioDTO)
        {
            response = await _sesionService.RegistroUsuario(usuarioDTO);
            return response;
        }

        [HttpGet("[action]")]
        public async Task<Response> MostrarCategoria()
        {
            response = await _sesionService.MostrarCategoria();
            return response;
        }
    }
}
