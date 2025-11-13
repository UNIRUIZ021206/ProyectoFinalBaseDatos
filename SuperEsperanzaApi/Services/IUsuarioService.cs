using SuperEsperanzaApi.Models;

namespace SuperEsperanzaApi.Services
{
    public interface IUsuarioService
    {
        Task<Usuario?> ValidarUsuarioAsync(string nombreUsuario, string contrasena);
        Task<string> GenerarTokenAsync(Usuario usuario);
    }
}
