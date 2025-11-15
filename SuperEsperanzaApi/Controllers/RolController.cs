using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SuperEsperanzaApi.Models;
using SuperEsperanzaApi.Services;

namespace SuperEsperanzaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 1. Esto protege TODOS los métodos de la clase por defecto
    public class RolController : ControllerBase
    {
        private readonly IRolService _rolService;

        public RolController(IRolService rolService) => _rolService = rolService;

        [HttpGet]
        // [AllowAnonymous] <--- 2. AL BORRAR ESTA LÍNEA, AHORA EXIGE TOKEN
        public async Task<IActionResult> GetAll() => Ok(await _rolService.ListarRolesAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var rol = await _rolService.ObtenerRolPorIdAsync(id);
            return rol is null ? NotFound() : Ok(rol);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Este exige token Y ADEMÁS ser Admin
        public async Task<IActionResult> Create([FromBody] Rol input)
        {
            if (string.IsNullOrWhiteSpace(input.NombreRol)) return BadRequest("NombreRol es requerido.");

            var userId = GetUserIdFromClaims();
            if (userId == 0) return Unauthorized();

            var newId = await _rolService.InsertarRolAsync(input.NombreRol, userId);
            return CreatedAtAction(nameof(GetById), new { id = newId }, new { Id = newId });
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] Rol input)
        {
            if (string.IsNullOrWhiteSpace(input.NombreRol)) return BadRequest("NombreRol es requerido.");

            var userId = GetUserIdFromClaims();
            if (userId == 0) return Unauthorized();

            var ok = await _rolService.ActualizarRolAsync(id, input.NombreRol, userId);
            return ok ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")] // Probablemente quieras proteger borrar también
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _rolService.EliminarRolAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        // Método Helper para extraer el ID del usuario del Token JWT
        private int GetUserIdFromClaims()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out int userId))
            {
                return userId;
            }
            return 0;
        }
    }

    // (El resto de clases auxiliares sigue igual)
    internal static class RolModelExtensions
    {
        public static int IdUsuarioCreacionSafe(this Rol r) => r is null ? 0 : (r is RolWithAudit rwa ? rwa.IdUsuarioCreacion : 0);
        public static int IdUsuarioModificacionSafe(this Rol r) => r is null ? 0 : (r is RolWithAudit rwa ? rwa.IdUsuarioModificacion : 0);
    }

    internal class RolWithAudit : Rol
    {
        public int IdUsuarioCreacion { get; set; }
        public int IdUsuarioModificacion { get; set; }
    }
}