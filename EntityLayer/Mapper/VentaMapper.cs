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
    public partial class VentaMapper
    {
        public partial VentaDTO VentaToVentaDTO(Ventum ventum);
        public partial Ventum VentaDTOToVenta(VentaDTO ventaDTO);
    }
}
