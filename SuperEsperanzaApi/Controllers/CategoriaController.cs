using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperEsperanzaApi.Dto;
using SuperEsperanzaApi.Models;
using SuperEsperanzaApi.Services.Interfaces; // Asegúrate de tener la interfaz IService
using System.Security.Claims;

namespace SuperEsperanzaApi.Controlador
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Todo el controlador está protegido por defecto
    public class CategoriaController : ControllerBase
    {
        private readonly IService<Categoria> _service;
        private readonly IMapper _mapper;

        public CategoriaController(IService<Categoria> service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        // Helper para obtener el ID del usuario desde el token
        private int GetUserId()
        {
            // Busca el Claim "NameIdentifier" que guardamos en el JwtService
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null && int.TryParse(claim.Value, out int id) ? id : 0;
        }

        // GET: api/Categoria
        [HttpGet]
        [AllowAnonymous] // Opcional: Permite que cualquiera vea las categorías sin un token
        public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetAll()
        {
            var lista = await _service.ListarAsync();
            // Mapea la lista de Modelos (Categoria) a una lista de DTOs (CategoriaDto)
            return Ok(_mapper.Map<IEnumerable<CategoriaDto>>(lista));
        }

        // GET: api/Categoria/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaDto>> GetById(int id)
        {
            var obj = await _service.ObtenerPorIdAsync(id);
            if (obj == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CategoriaDto>(obj));
        }

        // POST: api/Categoria
        [HttpPost]
        [Authorize(Roles = "Administrador,Bodeguero")] // Solo estos roles pueden crear
        public async Task<ActionResult<CategoriaDto>> Create(CategoriaCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // 1. Mapea el DTO (entrada) a un Modelo (para la base de datos)
            var entity = _mapper.Map<Categoria>(dto);

            // 2. Añade el ID del usuario que está creando (Auditoría)
            entity.Id_UsuarioCreacion = GetUserId();

            // 3. Llama al servicio para crear
            var (ok, error) = await _service.CrearAsync(entity);
            if (!ok)
            {
                return BadRequest(new ErrorResponse { Error = error });
            }

            // 4. Mapea la entidad (ya con el ID de la BD) a un DTO para devolverla
            var dtoCreado = _mapper.Map<CategoriaDto>(entity);

            return CreatedAtAction(nameof(GetById), new { id = entity.Id_Categoria }, dtoCreado);
        }

        // PUT: api/Categoria/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Bodeguero")]
        public async Task<ActionResult> Update(int id, CategoriaUpdateDto dto)
        {
            // 1. Busca la entidad que ya existe
            var entity = await _service.ObtenerPorIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            // 2. Mapea los cambios del DTO sobre la entidad existente
            _mapper.Map(dto, entity);

            // 3. Añade el ID del usuario que está modificando (Auditoría)
            entity.Id_UsuarioModificacion = GetUserId();

            // 4. Llama al servicio para actualizar
            var (ok, error) = await _service.ActualizarAsync(entity);
            if (!ok)
            {
                return BadRequest(new ErrorResponse { Error = error });
            }

            return Ok(new MensajeResponse { Mensaje = "Categoría actualizada" });
        }

        // DELETE: api/Categoria/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")] // Solo Admin puede borrar
        public async Task<ActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var (ok, error) = await _service.EliminarAsync(id, userId);

            if (!ok)
            {
                return BadRequest(new ErrorResponse { Error = error });
            }
            return NoContent(); // Éxito
        }
    }
}