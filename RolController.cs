csharp SuperEsperanzaApi\Controllers\RolController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SuperEsperanzaApi.Models;
using SuperEsperanzaApi.Services;

namespace SuperEsperanzaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Requiere autenticación para todas las acciones del controlador
    public class RolController : ControllerBase
    {
        private readonly IRolService _rolService;

        public RolController(IRolService rolService)
        {
            _rolService = rolService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _rolService.ListarRolesAsync();
            return Ok(roles);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var rol = await _rolService.ObtenerRolPorIdAsync(id);
            if (rol == null) return NotFound();
            return Ok(rol);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Rol input)
        {
            if (string.IsNullOrWhiteSpace(input.NombreRol)) return BadRequest("NombreRol es requerido.");

            var userId = GetUserIdFromClaims();
            if (userId == 0) return Unauthorized();

            var newId = await _rolService.InsertarRolAsync(input.NombreRol, userId);
            return CreatedAtAction(nameof(GetById), new { id = newId }, new { Id = newId });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Rol input)
        {
            if (string.IsNullOrWhiteSpace(input.NombreRol)) return BadRequest("NombreRol es requerido.");

            var userId = GetUserIdFromClaims();
            if (userId == 0) return Unauthorized();

            var ok = await _rolService.ActualizarRolAsync(id, input.NombreRol, userId);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserIdFromClaims();
            if (userId == 0) return Unauthorized();

            var ok = await _rolService.EliminarRolAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        // Obtener Id de usuario desde los claims del token
        private int GetUserIdFromClaims()
        {
            if (!(User?.Identity?.IsAuthenticated ?? false)) return 0;

            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? User.FindFirst("id")?.Value
                        ?? User.FindFirst("sub")?.Value;

            return int.TryParse(claim, out var id) ? id : 0;
        }
    }

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