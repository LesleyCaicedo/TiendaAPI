using BusinessLayer.Services;
using EntityLayer.DTO;
using EntityLayer.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _productoService;
        Response response = new();

        public ProductoController(IProductoService productoService) 
        {
            _productoService = productoService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> IngresarProducto([FromBody] ProductoDTO productoDTO)
        {
            response = await _productoService.IngresarProducto(productoDTO);
            return Ok(response);
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> ActualizarProducto([FromBody] ProductoDTO productoDTO)
        {
            response = await _productoService.ActualizarProducto(productoDTO);
            return Ok(response);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ObtenerProductos()
        {
            response = await _productoService.ObtenerProductos();
            return Ok(response);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ObtenerProducto(int productoID)
        {
            response = await _productoService.ObtenerProductoID(productoID);
            return Ok(response);
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> EliminarProductoID(int productoID)
        {
            response = await _productoService.EliminarProductoID(productoID);
            return Ok(response);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GenerarArchivo()
        {
            byte[] pdfData = await _productoService.GenerarPDF2();

            return File(pdfData, "application/pdf", "test.pdf");
        }
    }
}
