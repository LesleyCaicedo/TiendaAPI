using System;
using System.Collections.Generic;

namespace EntityLayer.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Nombre { get; set; }

    public string Correo { get; set; }

    public string Direccion { get; set; }

    public string Ci { get; set; }

    public string Rol { get; set; }

    public bool Estado { get; set; }

    public string Clave { get; set; }

    public virtual ICollection<Ventum> Venta { get; set; } = new List<Ventum>();
}
