
using DataLayer.Repository;
using EntityLayer.DTO;
using EntityLayer.Models;
using EntityLayer.Responses;
using Newtonsoft.Json;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using RazorLight;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using System.Net;

namespace BusinessLayer.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductosRepository _productosRepository;
        Response response = new();

        #region OTROS_METODOS

        public ProductoService(IProductosRepository productosRepository)
        {
            _productosRepository = productosRepository;
        }

        public async Task<Response> IngresarProducto(ProductoDTO productoDTO)
        {
            response = await _productosRepository.IngresarProducto(productoDTO);
            return response;
        }

        public async Task<Response> ActualizarProducto(ProductoDTO productoDTO)
        {
            response = await _productosRepository.ActualizarProducto(productoDTO);
            return response;
        }

        public async Task<Response> ObtenerProductos()
        {
            response = await _productosRepository.ObtenerProductos();
            return response;
        }

        public async Task<Response> ObtenerProductoID(int productoID)
        {
            response = await _productosRepository.ObtenerProductoID(productoID);
            return response;
        }

        public async Task<Response> EliminarProductoID(int productoID)
        {
            response = await _productosRepository.EliminarProductoID(productoID);
            return response;
        }

        #endregion

        #region HTML_ESTATICO

        public async Task<byte[]> GenerarPDF()
        {
            string htmlRoute = Path.Combine(Directory.GetCurrentDirectory(), "Aplicacion", "Templates", "CustodiaReport.html");
            string html = await File.ReadAllTextAsync(htmlRoute);

            //await new BrowserFetcher().DownloadAsync();
            using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                ExecutablePath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"
            });
            using var page = await browser.NewPageAsync();
            await page.SetContentAsync(html);
            Stream pdfBytes = await page.PdfStreamAsync(new PdfOptions
            {
                Format = PaperFormat.A4,
                Landscape = true,
                PrintBackground = true,
                MarginOptions = new MarginOptions
                {
                    Top = "0",
                    Right = "0",
                    Bottom = "0",
                    Left = "0"
                }
            });
            byte[] pdfData = UseStreamDotReadMethod(pdfBytes);

            return pdfData;
        }

        #endregion

        public async Task<byte[]> GenerarPDF2()
        {
            string html = await RenderRazorViewToString("CustodioReporto.cshtml");

            //await new BrowserFetcher().DownloadAsync();
            using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                ExecutablePath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"
            });
            using var page = await browser.NewPageAsync();
            await page.SetContentAsync(html);
            Stream pdfBytes = await page.PdfStreamAsync(new PdfOptions
            {
                Format = PaperFormat.A4,
                Landscape = true,
                PreferCSSPageSize = true,
                PrintBackground = true,
                DisplayHeaderFooter = true,
                MarginOptions = new MarginOptions
                {
                    Top = 0,
                    Right = "10px",
                    Bottom = 0,
                    Left = "10px"
                },
                HeaderTemplate = await RenderRazorViewToString("CustodioReportoHeader.cshtml"),
                FooterTemplate = await File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "View", "CustodioReportoFooter.html"))
            });
            byte[] pdfData = UseStreamDotReadMethod(pdfBytes);

            return pdfData;
        }

        private async Task<string> RenderRazorViewToString(string View)
        {
            var engine = new RazorLightEngineBuilder()
            .UseFileSystemProject(Path.Combine(Directory.GetCurrentDirectory(), "View")) // Ruta donde está el .cshtml
            .UseMemoryCachingProvider()
            .Build();

            List<DocumentsCustodyResponse> itemNegociacion = itemNegociacionTEST();

            DateTime fechaValoracionUltima = new DateTime();
            foreach (var item in itemNegociacion)
            {
                var F_valoracion = DateTime.ParseExact(item.F_valoracion, "dd-MM-yyyy", null);
                if (F_valoracion > fechaValoracionUltima)
                {
                    fechaValoracionUltima = F_valoracion;
                }
            }
            string sfechaValoracionUltima = fechaValoracionUltima.ToString("dd/MM/yyyy");

            CustodioReportoModel modelo = null;

            if (View.Equals("CustodioReporto.cshtml"))
            {
                itemNegociacion = itemNegociacion.OrderBy(c => c.CI_cliente).ToList();
                string Cod_casa = itemNegociacion.FirstOrDefault().Cod_casa;
                string casaNombre = itemNegociacion.FirstOrDefault().Nombre_casa;
                List<ClientePortafolio> listaClientesPortafolio = itemNegociacion
                .GroupBy(c => new { c.Tipo_portafolio, c.CI_cliente, c.Nombre_oficina })
                .Select(g => new ClientePortafolio
                {
                    TipoPortafolio = g.Key.Tipo_portafolio,
                    CICliente = g.Key.CI_cliente,
                    Nombre_oficina = g.Key.Nombre_oficina
                }).ToList();

                modelo = new()
                {
                    Cod_casa = Cod_casa,
                    casaNombre = casaNombre,
                    sfechaValoracionUltima = sfechaValoracionUltima,
                    listaClientesPortafolio = listaClientesPortafolio,
                    itemNegociacion = itemNegociacion,
                    fechaHora = new()
                    {
                        Fecha = DateTime.Now.ToString("dd/MM/yyyy"),
                        Hora = DateTime.Now.ToString("HH:mm")
                    }
                };
            }
            else if (View.Equals("CustodioReportoCompra.cshtml"))
            {
                itemNegociacion = itemNegociacion.OrderBy(c => c.CI_cliente).ToList();
                string Cod_casa = itemNegociacion.FirstOrDefault().Cod_casa;
                string casaNombre = itemNegociacion.FirstOrDefault().Nombre_casa;
                List<ClientePortafolio> listaClientesPortafolio = itemNegociacion
                .GroupBy(c => new { c.Tipo_portafolio, c.CI_cliente, c.Nombre_oficina })
                .Select(g => new ClientePortafolio
                {
                    TipoPortafolio = g.Key.Tipo_portafolio,
                    CICliente = g.Key.CI_cliente,
                    Nombre_oficina = g.Key.Nombre_oficina
                }).ToList();

                modelo = new()
                {
                    Cod_casa = Cod_casa,
                    casaNombre = casaNombre,
                    sfechaValoracionUltima = sfechaValoracionUltima,
                    listaClientesPortafolio = listaClientesPortafolio,
                    itemNegociacion = itemNegociacion,
                    fechaHora = new()
                    {
                        Fecha = DateTime.Now.ToString("dd/MM/yyyy"),
                        Hora = DateTime.Now.ToString("HH:mm")
                    }
                };
            }
            else if(View.Equals("CustodioReportoHeader.cshtml")) 
            {
                string Cod_casa = itemNegociacion.FirstOrDefault().Cod_casa;
                string casaNombre = itemNegociacion.FirstOrDefault().Nombre_casa;
                modelo = new()
                {
                    Cod_casa = Cod_casa,
                    casaNombre = casaNombre,
                    sfechaValoracionUltima = sfechaValoracionUltima.Contains(" ")? sfechaValoracionUltima.Remove(sfechaValoracionUltima.IndexOf(" ")): sfechaValoracionUltima,
                    logo = await ImageUrlToBase64("https://suratlantida.com/assets/nuestraempresa4-CyAJyoiP.png"),
                    fechaHora = new()
                    {
                        Fecha = DateTime.Now.ToString("dd/MM/yyyy"),
                        Hora = DateTime.Now.ToString("HH:mm")
                    }
                };
            }

            return await engine.CompileRenderAsync(View, modelo);
        }

        private byte[] UseStreamDotReadMethod(Stream stream)
        {
            byte[] bytes;
            List<byte> totalStream = new();
            byte[] buffer = new byte[32];
            int read;
            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                totalStream.AddRange(buffer.Take(read));
            }
            bytes = totalStream.ToArray();
            return bytes;
        }

        private List<DocumentsCustodyResponse> itemNegociacionTEST()
        {
            //string json = @"[{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""30-01-2025"",""Docum"":""MO"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1792470935001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""09-12-2024"",""F_recompra"":""06-06-2025"",""Cod_emisor"":""E.B9"",""Nombre_emisor"":""CORPORACION FAVORITA C.A."",""Cod_titulo"":null,""Nombre_titulo"":null,""Referencia"":null,""F_emision"":null,""F_vcto"":null,""Cantidad"":""22223"",""V_nominal_unitario"":""1"",""Asv_nominal_total"":""22223"",""Monto_exigido"":""40000.00"",""V_custodia"":""42001.47"",""Tasa_descuento"":"""",""Precio"":""1.89"",""No_operacion"":""335717"",""Tipogarantia"":"""",""V_actual"":""1.89"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""30-01-2025"",""Docum"":""MO"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1793184243001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""09-12-2024"",""F_recompra"":""06-06-2025"",""Cod_emisor"":""E.B9"",""Nombre_emisor"":""CORPORACION FAVORITA C.A."",""Cod_titulo"":null,""Nombre_titulo"":null,""Referencia"":null,""F_emision"":null,""F_vcto"":null,""Cantidad"":""15229"",""V_nominal_unitario"":""1"",""Asv_nominal_total"":""15229"",""Monto_exigido"":""27411.00"",""V_custodia"":""28782.81"",""Tasa_descuento"":"""",""Precio"":""1.89"",""No_operacion"":""335748"",""Tipogarantia"":"""",""V_actual"":""1.89"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""30-01-2025"",""Docum"":""MO"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1792470935001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""11-12-2024"",""F_recompra"":""09-06-2025"",""Cod_emisor"":""E.B9"",""Nombre_emisor"":""CORPORACION FAVORITA C.A."",""Cod_titulo"":null,""Nombre_titulo"":null,""Referencia"":null,""F_emision"":null,""F_vcto"":null,""Cantidad"":""29742"",""V_nominal_unitario"":""1"",""Asv_nominal_total"":""29742"",""Monto_exigido"":""53533.00"",""V_custodia"":""56212.38"",""Tasa_descuento"":"""",""Precio"":""1.89"",""No_operacion"":""335899"",""Tipogarantia"":"""",""V_actual"":""1.89"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""30-01-2025"",""Docum"":""MO"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1793184243001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""06-01-2025"",""F_recompra"":""04-07-2025"",""Cod_emisor"":""E.B9"",""Nombre_emisor"":""CORPORACION FAVORITA C.A."",""Cod_titulo"":null,""Nombre_titulo"":null,""Referencia"":null,""F_emision"":null,""F_vcto"":null,""Cantidad"":""39452"",""V_nominal_unitario"":""1"",""Asv_nominal_total"":""39452"",""Monto_exigido"":""74957.00"",""V_custodia"":""74564.28"",""Tasa_descuento"":"""",""Precio"":""1.89"",""No_operacion"":""337117"",""Tipogarantia"":"""",""V_actual"":""1.89"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""16-12-2024"",""Docum"":""GA"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1792470935001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""09-12-2024"",""F_recompra"":""06-06-2025"",""Cod_emisor"":"""",""Nombre_emisor"":null,""Cod_titulo"":"""",""Nombre_titulo"":""EFECTIVO"",""Referencia"":"""",""F_emision"":"""",""F_vcto"":"""",""Cantidad"":"""",""V_nominal_unitario"":""4088.33"",""Asv_nominal_total"":""4088.33"",""Monto_exigido"":"""",""V_custodia"":""4090.85"",""Tasa_descuento"":"""",""Precio"":"""",""No_operacion"":""335717"",""Tipogarantia"":""E"",""V_actual"":""4090.85"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""16-12-2024"",""Docum"":""GA"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1793184243001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""09-12-2024"",""F_recompra"":""06-06-2025"",""Cod_emisor"":"""",""Nombre_emisor"":null,""Cod_titulo"":"""",""Nombre_titulo"":""EFECTIVO"",""Referencia"":"""",""F_emision"":"""",""F_vcto"":"""",""Cantidad"":"""",""V_nominal_unitario"":""2801.64"",""Asv_nominal_total"":""2801.64"",""Monto_exigido"":"""",""V_custodia"":""2803.61"",""Tasa_descuento"":"""",""Precio"":"""",""No_operacion"":""335748"",""Tipogarantia"":""E"",""V_actual"":""2803.61"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""16-12-2024"",""Docum"":""GA"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1792470935001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""11-12-2024"",""F_recompra"":""09-06-2025"",""Cod_emisor"":"""",""Nombre_emisor"":null,""Cod_titulo"":"""",""Nombre_titulo"":""EFECTIVO"",""Referencia"":"""",""F_emision"":"""",""F_vcto"":"""",""Cantidad"":"""",""V_nominal_unitario"":""5487.13"",""Asv_nominal_total"":""5487.13"",""Monto_exigido"":"""",""V_custodia"":""5492.09"",""Tasa_descuento"":"""",""Precio"":"""",""No_operacion"":""335899"",""Tipogarantia"":""E"",""V_actual"":""5492.09"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""14-01-2025"",""Docum"":""GA"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1793184243001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""06-01-2025"",""F_recompra"":""04-07-2025"",""Cod_emisor"":"""",""Nombre_emisor"":null,""Cod_titulo"":"""",""Nombre_titulo"":""EFECTIVO"",""Referencia"":"""",""F_emision"":"""",""F_vcto"":"""",""Cantidad"":"""",""V_nominal_unitario"":""7661.23"",""Asv_nominal_total"":""7661.23"",""Monto_exigido"":"""",""V_custodia"":""7661.61"",""Tasa_descuento"":"""",""Precio"":"""",""No_operacion"":""337117"",""Tipogarantia"":""E"",""V_actual"":""7661.61"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""}]";

            string json = @"[{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""30-01-2025"",""Docum"":""MO"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1792470935001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""09-12-2024"",""F_recompra"":""06-06-2025"",""Cod_emisor"":""E.B9"",""Nombre_emisor"":""CORPORACION FAVORITA C.A."",""Cod_titulo"":null,""Nombre_titulo"":null,""Referencia"":null,""F_emision"":null,""F_vcto"":null,""Cantidad"":""22223"",""V_nominal_unitario"":""1"",""Asv_nominal_total"":""22223"",""Monto_exigido"":""40000.00"",""V_custodia"":""42001.47"",""Tasa_descuento"":"""",""Precio"":""1.89"",""No_operacion"":""335717"",""Tipogarantia"":"""",""V_actual"":""1.89"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""30-01-2025"",""Docum"":""MO"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1793184243001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""09-12-2024"",""F_recompra"":""06-06-2025"",""Cod_emisor"":""E.B9"",""Nombre_emisor"":""CORPORACION FAVORITA C.A."",""Cod_titulo"":null,""Nombre_titulo"":null,""Referencia"":null,""F_emision"":null,""F_vcto"":null,""Cantidad"":""15229"",""V_nominal_unitario"":""1"",""Asv_nominal_total"":""15229"",""Monto_exigido"":""27411.00"",""V_custodia"":""28782.81"",""Tasa_descuento"":"""",""Precio"":""1.89"",""No_operacion"":""335748"",""Tipogarantia"":"""",""V_actual"":""1.89"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""30-01-2025"",""Docum"":""MO"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1792470935001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""11-12-2024"",""F_recompra"":""09-06-2025"",""Cod_emisor"":""E.B9"",""Nombre_emisor"":""CORPORACION FAVORITA C.A."",""Cod_titulo"":null,""Nombre_titulo"":null,""Referencia"":null,""F_emision"":null,""F_vcto"":null,""Cantidad"":""29742"",""V_nominal_unitario"":""1"",""Asv_nominal_total"":""29742"",""Monto_exigido"":""53533.00"",""V_custodia"":""56212.38"",""Tasa_descuento"":"""",""Precio"":""1.89"",""No_operacion"":""335899"",""Tipogarantia"":"""",""V_actual"":""1.89"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""30-01-2025"",""Docum"":""MO"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1793184243001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""06-01-2025"",""F_recompra"":""04-07-2025"",""Cod_emisor"":""E.B9"",""Nombre_emisor"":""CORPORACION FAVORITA C.A."",""Cod_titulo"":null,""Nombre_titulo"":null,""Referencia"":null,""F_emision"":null,""F_vcto"":null,""Cantidad"":""39452"",""V_nominal_unitario"":""1"",""Asv_nominal_total"":""39452"",""Monto_exigido"":""74957.00"",""V_custodia"":""74564.28"",""Tasa_descuento"":"""",""Precio"":""1.89"",""No_operacion"":""337117"",""Tipogarantia"":"""",""V_actual"":""1.89"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""16-12-2024"",""Docum"":""GA"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1792470935001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""09-12-2024"",""F_recompra"":""06-06-2025"",""Cod_emisor"":"""",""Nombre_emisor"":null,""Cod_titulo"":"""",""Nombre_titulo"":""EFECTIVO"",""Referencia"":"""",""F_emision"":"""",""F_vcto"":"""",""Cantidad"":"""",""V_nominal_unitario"":""4088.33"",""Asv_nominal_total"":""4088.33"",""Monto_exigido"":"""",""V_custodia"":""4090.85"",""Tasa_descuento"":"""",""Precio"":"""",""No_operacion"":""335717"",""Tipogarantia"":""E"",""V_actual"":""4090.85"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""16-12-2024"",""Docum"":""GA"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1793184243001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""09-12-2024"",""F_recompra"":""06-06-2025"",""Cod_emisor"":"""",""Nombre_emisor"":null,""Cod_titulo"":"""",""Nombre_titulo"":""EFECTIVO"",""Referencia"":"""",""F_emision"":"""",""F_vcto"":"""",""Cantidad"":"""",""V_nominal_unitario"":""2801.64"",""Asv_nominal_total"":""2801.64"",""Monto_exigido"":"""",""V_custodia"":""2803.61"",""Tasa_descuento"":"""",""Precio"":"""",""No_operacion"":""335748"",""Tipogarantia"":""E"",""V_actual"":""2803.61"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""16-12-2024"",""Docum"":""GA"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1792470935001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""11-12-2024"",""F_recompra"":""09-06-2025"",""Cod_emisor"":"""",""Nombre_emisor"":null,""Cod_titulo"":"""",""Nombre_titulo"":""EFECTIVO"",""Referencia"":"""",""F_emision"":"""",""F_vcto"":"""",""Cantidad"":"""",""V_nominal_unitario"":""5487.13"",""Asv_nominal_total"":""5487.13"",""Monto_exigido"":"""",""V_custodia"":""5492.09"",""Tasa_descuento"":"""",""Precio"":"""",""No_operacion"":""335899"",""Tipogarantia"":""E"",""V_actual"":""5492.09"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""14-01-2025"",""Docum"":""GA"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1793184243001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""06-01-2025"",""F_recompra"":""04-07-2025"",""Cod_emisor"":"""",""Nombre_emisor"":null,""Cod_titulo"":"""",""Nombre_titulo"":""EFECTIVO"",""Referencia"":"""",""F_emision"":"""",""F_vcto"":"""",""Cantidad"":"""",""V_nominal_unitario"":""7661.23"",""Asv_nominal_total"":""7661.23"",""Monto_exigido"":"""",""V_custodia"":""7661.61"",""Tasa_descuento"":"""",""Precio"":"""",""No_operacion"":""337117"",""Tipogarantia"":""E"",""V_actual"":""7661.61"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""30-01-2025"",""Docum"":""MO"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1792470935001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""09-12-2024"",""F_recompra"":""06-06-2025"",""Cod_emisor"":""E.B9"",""Nombre_emisor"":""CORPORACION FAVORITA C.A."",""Cod_titulo"":null,""Nombre_titulo"":null,""Referencia"":null,""F_emision"":null,""F_vcto"":null,""Cantidad"":""22223"",""V_nominal_unitario"":""1"",""Asv_nominal_total"":""22223"",""Monto_exigido"":""40000.00"",""V_custodia"":""42001.47"",""Tasa_descuento"":"""",""Precio"":""1.89"",""No_operacion"":""335717"",""Tipogarantia"":"""",""V_actual"":""1.89"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""30-01-2025"",""Docum"":""MO"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1793184243001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""09-12-2024"",""F_recompra"":""06-06-2025"",""Cod_emisor"":""E.B9"",""Nombre_emisor"":""CORPORACION FAVORITA C.A."",""Cod_titulo"":null,""Nombre_titulo"":null,""Referencia"":null,""F_emision"":null,""F_vcto"":null,""Cantidad"":""15229"",""V_nominal_unitario"":""1"",""Asv_nominal_total"":""15229"",""Monto_exigido"":""27411.00"",""V_custodia"":""28782.81"",""Tasa_descuento"":"""",""Precio"":""1.89"",""No_operacion"":""335748"",""Tipogarantia"":"""",""V_actual"":""1.89"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""30-01-2025"",""Docum"":""MO"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1792470935001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""11-12-2024"",""F_recompra"":""09-06-2025"",""Cod_emisor"":""E.B9"",""Nombre_emisor"":""CORPORACION FAVORITA C.A."",""Cod_titulo"":null,""Nombre_titulo"":null,""Referencia"":null,""F_emision"":null,""F_vcto"":null,""Cantidad"":""29742"",""V_nominal_unitario"":""1"",""Asv_nominal_total"":""29742"",""Monto_exigido"":""53533.00"",""V_custodia"":""56212.38"",""Tasa_descuento"":"""",""Precio"":""1.89"",""No_operacion"":""335899"",""Tipogarantia"":"""",""V_actual"":""1.89"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""30-01-2025"",""Docum"":""MO"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1793184243001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""06-01-2025"",""F_recompra"":""04-07-2025"",""Cod_emisor"":""E.B9"",""Nombre_emisor"":""CORPORACION FAVORITA C.A."",""Cod_titulo"":null,""Nombre_titulo"":null,""Referencia"":null,""F_emision"":null,""F_vcto"":null,""Cantidad"":""39452"",""V_nominal_unitario"":""1"",""Asv_nominal_total"":""39452"",""Monto_exigido"":""74957.00"",""V_custodia"":""74564.28"",""Tasa_descuento"":"""",""Precio"":""1.89"",""No_operacion"":""337117"",""Tipogarantia"":"""",""V_actual"":""1.89"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""16-12-2024"",""Docum"":""GA"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1792470935001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""09-12-2024"",""F_recompra"":""06-06-2025"",""Cod_emisor"":"""",""Nombre_emisor"":null,""Cod_titulo"":"""",""Nombre_titulo"":""EFECTIVO"",""Referencia"":"""",""F_emision"":"""",""F_vcto"":"""",""Cantidad"":"""",""V_nominal_unitario"":""4088.33"",""Asv_nominal_total"":""4088.33"",""Monto_exigido"":"""",""V_custodia"":""4090.85"",""Tasa_descuento"":"""",""Precio"":"""",""No_operacion"":""335717"",""Tipogarantia"":""E"",""V_actual"":""4090.85"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""16-12-2024"",""Docum"":""GA"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1793184243001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""09-12-2024"",""F_recompra"":""06-06-2025"",""Cod_emisor"":"""",""Nombre_emisor"":null,""Cod_titulo"":"""",""Nombre_titulo"":""EFECTIVO"",""Referencia"":"""",""F_emision"":"""",""F_vcto"":"""",""Cantidad"":"""",""V_nominal_unitario"":""2801.64"",""Asv_nominal_total"":""2801.64"",""Monto_exigido"":"""",""V_custodia"":""2803.61"",""Tasa_descuento"":"""",""Precio"":"""",""No_operacion"":""335748"",""Tipogarantia"":""E"",""V_actual"":""2803.61"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""16-12-2024"",""Docum"":""GA"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1792470935001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""11-12-2024"",""F_recompra"":""09-06-2025"",""Cod_emisor"":"""",""Nombre_emisor"":null,""Cod_titulo"":"""",""Nombre_titulo"":""EFECTIVO"",""Referencia"":"""",""F_emision"":"""",""F_vcto"":"""",""Cantidad"":"""",""V_nominal_unitario"":""5487.13"",""Asv_nominal_total"":""5487.13"",""Monto_exigido"":"""",""V_custodia"":""5492.09"",""Tasa_descuento"":"""",""Precio"":"""",""No_operacion"":""335899"",""Tipogarantia"":""E"",""V_actual"":""5492.09"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""14-01-2025"",""Docum"":""GA"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1793184243001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""06-01-2025"",""F_recompra"":""04-07-2025"",""Cod_emisor"":"""",""Nombre_emisor"":null,""Cod_titulo"":"""",""Nombre_titulo"":""EFECTIVO"",""Referencia"":"""",""F_emision"":"""",""F_vcto"":"""",""Cantidad"":"""",""V_nominal_unitario"":""7661.23"",""Asv_nominal_total"":""7661.23"",""Monto_exigido"":"""",""V_custodia"":""7661.61"",""Tasa_descuento"":"""",""Precio"":"""",""No_operacion"":""337117"",""Tipogarantia"":""E"",""V_actual"":""7661.61"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""30-01-2025"",""Docum"":""MO"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1792470935001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""09-12-2024"",""F_recompra"":""06-06-2025"",""Cod_emisor"":""E.B9"",""Nombre_emisor"":""CORPORACION FAVORITA C.A."",""Cod_titulo"":null,""Nombre_titulo"":null,""Referencia"":null,""F_emision"":null,""F_vcto"":null,""Cantidad"":""22223"",""V_nominal_unitario"":""1"",""Asv_nominal_total"":""22223"",""Monto_exigido"":""40000.00"",""V_custodia"":""42001.47"",""Tasa_descuento"":"""",""Precio"":""1.89"",""No_operacion"":""335717"",""Tipogarantia"":"""",""V_actual"":""1.89"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""30-01-2025"",""Docum"":""MO"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1793184243001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""09-12-2024"",""F_recompra"":""06-06-2025"",""Cod_emisor"":""E.B9"",""Nombre_emisor"":""CORPORACION FAVORITA C.A."",""Cod_titulo"":null,""Nombre_titulo"":null,""Referencia"":null,""F_emision"":null,""F_vcto"":null,""Cantidad"":""15229"",""V_nominal_unitario"":""1"",""Asv_nominal_total"":""15229"",""Monto_exigido"":""27411.00"",""V_custodia"":""28782.81"",""Tasa_descuento"":"""",""Precio"":""1.89"",""No_operacion"":""335748"",""Tipogarantia"":"""",""V_actual"":""1.89"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""30-01-2025"",""Docum"":""MO"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1792470935001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""11-12-2024"",""F_recompra"":""09-06-2025"",""Cod_emisor"":""E.B9"",""Nombre_emisor"":""CORPORACION FAVORITA C.A."",""Cod_titulo"":null,""Nombre_titulo"":null,""Referencia"":null,""F_emision"":null,""F_vcto"":null,""Cantidad"":""29742"",""V_nominal_unitario"":""1"",""Asv_nominal_total"":""29742"",""Monto_exigido"":""53533.00"",""V_custodia"":""56212.38"",""Tasa_descuento"":"""",""Precio"":""1.89"",""No_operacion"":""335899"",""Tipogarantia"":"""",""V_actual"":""1.89"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""30-01-2025"",""Docum"":""MO"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1793184243001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""06-01-2025"",""F_recompra"":""04-07-2025"",""Cod_emisor"":""E.B9"",""Nombre_emisor"":""CORPORACION FAVORITA C.A."",""Cod_titulo"":null,""Nombre_titulo"":null,""Referencia"":null,""F_emision"":null,""F_vcto"":null,""Cantidad"":""39452"",""V_nominal_unitario"":""1"",""Asv_nominal_total"":""39452"",""Monto_exigido"":""74957.00"",""V_custodia"":""74564.28"",""Tasa_descuento"":"""",""Precio"":""1.89"",""No_operacion"":""337117"",""Tipogarantia"":"""",""V_actual"":""1.89"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""16-12-2024"",""Docum"":""GA"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1792470935001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""09-12-2024"",""F_recompra"":""06-06-2025"",""Cod_emisor"":"""",""Nombre_emisor"":null,""Cod_titulo"":"""",""Nombre_titulo"":""EFECTIVO"",""Referencia"":"""",""F_emision"":"""",""F_vcto"":"""",""Cantidad"":"""",""V_nominal_unitario"":""4088.33"",""Asv_nominal_total"":""4088.33"",""Monto_exigido"":"""",""V_custodia"":""4090.85"",""Tasa_descuento"":"""",""Precio"":"""",""No_operacion"":""335717"",""Tipogarantia"":""E"",""V_actual"":""4090.85"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""16-12-2024"",""Docum"":""GA"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1793184243001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""09-12-2024"",""F_recompra"":""06-06-2025"",""Cod_emisor"":"""",""Nombre_emisor"":null,""Cod_titulo"":"""",""Nombre_titulo"":""EFECTIVO"",""Referencia"":"""",""F_emision"":"""",""F_vcto"":"""",""Cantidad"":"""",""V_nominal_unitario"":""2801.64"",""Asv_nominal_total"":""2801.64"",""Monto_exigido"":"""",""V_custodia"":""2803.61"",""Tasa_descuento"":"""",""Precio"":"""",""No_operacion"":""335748"",""Tipogarantia"":""E"",""V_actual"":""2803.61"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""16-12-2024"",""Docum"":""GA"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1792470935001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""11-12-2024"",""F_recompra"":""09-06-2025"",""Cod_emisor"":"""",""Nombre_emisor"":null,""Cod_titulo"":"""",""Nombre_titulo"":""EFECTIVO"",""Referencia"":"""",""F_emision"":"""",""F_vcto"":"""",""Cantidad"":"""",""V_nominal_unitario"":""5487.13"",""Asv_nominal_total"":""5487.13"",""Monto_exigido"":"""",""V_custodia"":""5492.09"",""Tasa_descuento"":"""",""Precio"":"""",""No_operacion"":""335899"",""Tipogarantia"":""E"",""V_actual"":""5492.09"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""},{""Cod_casa"":""H9"",""Nombre_casa"":""ACCITLAN"",""F_valoracion"":""14-01-2025"",""Docum"":""GA"",""Tipo_portafolio"":""Cliente"",""CI_cliente"":""1793184243001"",""Cod_oficina"":""01"",""Nombre_oficina"":""Matriz"",""F_ingreso"":""06-01-2025"",""F_recompra"":""04-07-2025"",""Cod_emisor"":"""",""Nombre_emisor"":null,""Cod_titulo"":"""",""Nombre_titulo"":""EFECTIVO"",""Referencia"":"""",""F_emision"":"""",""F_vcto"":"""",""Cantidad"":"""",""V_nominal_unitario"":""7661.23"",""Asv_nominal_total"":""7661.23"",""Monto_exigido"":"""",""V_custodia"":""7661.61"",""Tasa_descuento"":"""",""Precio"":"""",""No_operacion"":""337117"",""Tipogarantia"":""E"",""V_actual"":""7661.61"",""V_exigido_a"":""0"",""V_exigido_b"":null,""M"":""D"",""Postura"":""VENTA""}]";

            List<DocumentsCustodyResponse> lista = JsonConvert.DeserializeObject<List<DocumentsCustodyResponse>>(json);

            return lista;
        }

        public async Task<string> ImageUrlToBase64(string imageUrl)
        {
            using var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync(imageUrl);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error al descargar la imagen: {response.StatusCode}");

            var contentType = response.Content.Headers.ContentType?.MediaType;
            var imageBytes = await response.Content.ReadAsByteArrayAsync();
            string b64image = Convert.ToBase64String(imageBytes);

            string extension = contentType switch
            {
                "image/jpeg" => "jpg",
                "image/png" => "png",
                "image/gif" => "gif",
                "image/webp" => "webp",
                "image/svg+xml" => "svg",
                "image/bmp" => "bmp",
                _ => "png"
            };

            return $"data:image/{extension};base64,{b64image}";
        }


    }
}
