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
    public partial class CategoriaMapper
    {
        public partial CategoriaDTO CategoriaToCategoriaDTO(Categorium categorium);

        public partial Categorium CategoriaDTOToCategoria(CategoriaDTO categoriaDTO);
    }
}
