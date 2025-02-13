using System;
using System.Collections.Generic;

namespace EntityLayer.Models;

public partial class Ventum
{
    public int IdVenta { get; set; }

    public int IdProducto { get; set; }

    public int IdUsuario { get; set; }

    public string? Detalle { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
