using System;

namespace SuperEsperanzaApi.Models
{
    public class Usuario
    {
        public int Id { get; set; } // Mapea Id_Usuario
        public string CodigoUsuario { get; set; } = string.Empty; // Mapea CodigoUsuario (login)
        public string NombreUsuario { get; set; } = string.Empty; // Nombre de usuario lógico si la app lo requiere
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public byte[] Clave { get; set; } = Array.Empty<byte>(); // Mapea Clave VARBINARY
        public int IdRol { get; set; } // Mapea Id_Rol
        public string? Rol { get; set; } // Nombre del rol (opcional)
        public bool Estado { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? IdUsuarioCreacion { get; set; }
        public int? IdUsuarioModificacion { get; set; }
    }
}               
