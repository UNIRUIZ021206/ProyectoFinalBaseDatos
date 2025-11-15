using System;

namespace SuperEsperanzaApi.Models
{
    public class Rol
    {
        public int Id { get; set; } // map to Id_Rol
        public string NombreRol { get; set; } = string.Empty;
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}