using System.ComponentModel.DataAnnotations;

namespace SuperEsperanzaApi.Dto
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "El código de usuario es obligatorio")]
        public string NombreUsuario { get; set; } = string.Empty; // Recibirá el CodigoUsuario
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Contrasena { get; set; } = string.Empty;
    }
}
