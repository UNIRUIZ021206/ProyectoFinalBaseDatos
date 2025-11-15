csharp SuperEsperanzaApi\Models\Rol.cs
using System;

namespace SuperEsperanzaApi.Models
{
    public class Rol
    {
        public int Id { get; set; } // map to Id_Rol

        // Nombre del rol (no nulo)
        public string NombreRol { get; set; } = string.Empty;

        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}