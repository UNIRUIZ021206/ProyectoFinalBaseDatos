using SuperEsperanzaApi.Dao;
using SuperEsperanzaApi.Models;

namespace SuperEsperanzaApi.Services
{
    public class RolService : IRolService
    {
        private readonly RolDAO _rolDao;

        public RolService(RolDAO rolDao)
        {
            _rolDao = rolDao;
        }

        public Task<IEnumerable<Rol>> ListarRolesAsync() => _rolDao.ListarRolesAsync();

        public Task<Rol?> ObtenerRolPorIdAsync(int id) => _rolDao.ObtenerRolPorIdAsync(id);

        public Task<int> InsertarRolAsync(string nombreRol, int idUsuarioCreacion) =>
            _rolDao.InsertarRolAsync(nombreRol, idUsuarioCreacion);

        public Task<bool> ActualizarRolAsync(int idRol, string nombreRol, int idUsuarioModificacion) =>
            _rolDao.ActualizarRolAsync(idRol, nombreRol, idUsuarioModificacion);

        public Task<bool> EliminarRolAsync(int idRol) => _rolDao.EliminarRolAsync(idRol);
    }
}