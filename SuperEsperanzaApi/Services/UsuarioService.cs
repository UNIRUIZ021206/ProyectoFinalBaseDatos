using SuperEsperanzaApi.Dao;
using SuperEsperanzaApi.Models;

namespace SuperEsperanzaApi.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly UsuarioDAO _usuarioDAO;
        private readonly JwtService _jwtService;

        public UsuarioService(UsuarioDAO usuarioDAO, JwtService jwtService)
        {
            _usuarioDAO = usuarioDAO;
            _jwtService = jwtService;
        }

        public Task<string> GenerarTokenAsync(Usuario usuario)
        {
            var token = _jwtService.GenerateToken(usuario);
            return Task.FromResult(token);
        }

        public async Task<Usuario?> ValidarUsuarioAsync(string nombreUsuario, string contrasena)
        {
            // "nombreUsuario" es el CodigoUsuario que viene del DTO
            return await _usuarioDAO.ValidarUsuarioAsync(nombreUsuario, contrasena);
        }
    }
}

