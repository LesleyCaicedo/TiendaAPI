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
    public partial class UsuarioMapper
    {
        public partial UsuarioDTO UsuarioToUsuarioDTO(Usuario usuario);
        public partial Usuario UsuarioDTOToUsuario(UsuarioDTO usuarioDTO);
    }
}
