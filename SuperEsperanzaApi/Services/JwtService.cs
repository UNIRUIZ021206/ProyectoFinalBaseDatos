using Microsoft.IdentityModel.Tokens;
using SuperEsperanzaApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SuperEsperanzaApi.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;
        public JwtService(IConfiguration config) { _config = config; }

        public string GenerateToken(Usuario usuario)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                
                // --- CORRECCIÓN AQUÍ ---
                // Se añade '?? string.Empty' para eliminar la advertencia CS8604
                new Claim(ClaimTypes.Name, usuario.NombreUsuario ?? string.Empty),
                new Claim(ClaimTypes.Role, usuario.Rol ?? string.Empty),
                // --- FIN CORRECCIÓN ---
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}