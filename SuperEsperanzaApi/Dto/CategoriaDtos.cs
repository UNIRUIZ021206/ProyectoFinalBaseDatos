using System.ComponentModel.DataAnnotations;

namespace SuperEsperanzaApi.Dto
{
    public class CategoriaDto
    {
        public int Id_Categoria { get; set; }
        public string CodigoCategoria { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
    }

    public class CategoriaCreateDto
    {
        [Required] public string CodigoCategoria { get; set; } = string.Empty;
        [Required] public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }

    public class CategoriaUpdateDto
    {
        [Required] public string CodigoCategoria { get; set; } = string.Empty;
        [Required] public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        [Required] public bool Estado { get; set; }
    }
}
