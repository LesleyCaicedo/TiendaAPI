using System;
using System.Collections.Generic;

namespace EntityLayer.Models;

public partial class Producto
{
    public int IdProducto { get; set; }

    public string Nombre { get; set; }

    public decimal Precio { get; set; }

    public int Stock { get; set; }

    public string? Detalle { get; set; }

    public bool Estado { get; set; }

    public int IdCategoria { get; set; }

    public virtual Categorium IdCategoriaNavigation { get; set; } = null!;

    public virtual ICollection<Ventum> Venta { get; set; } = new List<Ventum>();
}
