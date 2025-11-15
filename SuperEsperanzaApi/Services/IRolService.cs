using SuperEsperanzaApi.Models;

namespace SuperEsperanzaApi.Services
{
    public interface IRolService
    {
        Task<IEnumerable<Rol>> ListarRolesAsync();
        Task<Rol?> ObtenerRolPorIdAsync(int id);
        Task<int> InsertarRolAsync(string nombreRol, int idUsuarioCreacion);
        Task<bool> ActualizarRolAsync(int idRol, string nombreRol, int idUsuarioModificacion);
        Task<bool> EliminarRolAsync(int idRol);
    }
}