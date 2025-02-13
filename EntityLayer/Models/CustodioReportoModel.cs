using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Models
{
    public class CustodioReportoModel
    {
        public string? Cod_casa {  get; set; }
        public string? casaNombre { get; set; }
        public ServerDateAndTimeResponse? fechaHora { get; set; }
        public object? logo { get; set; }
        public string? sfechaValoracionUltima { get; set; }
        public List<ClientePortafolio>? listaClientesPortafolio { get; set; }
        public List<DocumentsCustodyResponse>? itemNegociacion { get; set; }
    }

    public class ClientePortafolio
    {
        public string? TipoPortafolio { get; set; }
        public string? CICliente { get; set; }
        public string? Nombre_oficina { get; set; }
    }
}
