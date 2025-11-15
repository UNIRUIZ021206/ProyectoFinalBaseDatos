using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperEsperanzaApi.Dto;
using SuperEsperanzaApi.Services;
using System.Linq;
using System.Security.Claims; // <-- Asegúrate de tener este 'using'

namespace SuperEsperanzaApi.Controlador
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Protege todo el controlador por defecto
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;

        public AuthController(IUsuarioService usuarioService,
            ILogger<AuthController> logger,
            IConfiguration configuration)
        {
            _usuarioService = usuarioService;
            _logger = logger;
            _configuration = configuration;
        }

        // Endpoint de estado de la API
        [HttpGet]
        [AllowAnonymous] // Permite esta ruta sin token
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            var response = new ApiStatusResponse
            {
                Mensaje = "API Super Esperanza funcionando correctamente",
                Version = "v1"
            };
            return Ok(response);
        }

        // Endpoint de Login
        [HttpPost("login")]
        [AllowAnonymous] // Permite esta ruta sin token
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(MensajeResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ErrorResponse { Error = "El cuerpo de la solicitud no puede estar vacío" });
            }

            _logger.LogInformation("Intento de inicio de sesión para: {Usuario}", request.NombreUsuario);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Datos de inicio de sesión inválidos para: {Usuario}", request.NombreUsuario);
                var errores = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors.Select(e => $"{x.Key}: {e.ErrorMessage}"))
                    .ToList();
                return BadRequest(new ErrorResponse
                {
                    Error = "Datos de entrada inválidos: " + string.Join("; ", errores)
                });
            }

            try
            {
                var usuario = await _usuarioService.ValidarUsuarioAsync(request.NombreUsuario, request.Contrasena);
                if (usuario == null)
                {
                    _logger.LogWarning("Fallo en la autenticación para: {Usuario}", request.NombreUsuario);
                    return Unauthorized(new MensajeResponse { Mensaje = "Credenciales inválidas" });
                }

                var token = await _usuarioService.GenerarTokenAsync(usuario);

                _logger.LogInformation("Inicio de sesión exitoso para: {Usuario}", request.NombreUsuario);
                var response = new LoginResponse
                {
                    Token = token,
                    Usuario = usuario.NombreUsuario,
                    Rol = usuario.Rol,
                    Expiracion = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"]))
                };
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error de operación al procesar el inicio de sesión para: {Usuario}. Error: {Error}", request.NombreUsuario, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Error = $"Error al validar usuario: {ex.Message}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al procesar el inicio de sesión para: {Usuario}. Tipo: {Tipo}, Mensaje: {Mensaje}",
                    request.NombreUsuario, ex.GetType().Name, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Error = "Error interno del servidor" });
            }
        }

        // Endpoint de prueba para verificar que la autorización funcione
        [HttpGet("test")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult TestAuth()
        {
            var user = User.Identity?.Name;
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            return Ok(new
            {
                Mensaje = "Autorización funcionando correctamente",
                Usuario = user,
                UsuarioId = userId,

                // --- CORRECCIÓN AQUÍ ---
                // (Arregla CS8601)
                Rol = role ?? "N/A", // Se asigna "N/A" si el rol es nulo
                // --- FIN CORRECCIÓN ---

                IsAuthenticated = User.Identity?.IsAuthenticated ?? false
            });
        }
    }
}