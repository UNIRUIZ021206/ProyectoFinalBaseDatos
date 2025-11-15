using Microsoft.Data.SqlClient;
using SuperEsperanzaApi.Dao.Interfaces; // <-- Ahora sí encontrará esta interfaz
using SuperEsperanzaApi.Data;

namespace SuperEsperanzaApi.Dao
{
    // Esta es la clase base que CategoriaDAO necesita heredar
    public abstract class RepositoryBase<T> : IRepository<T> where T : class
    {
        protected readonly ConexionDB _conexion;

        // Este es el constructor que CategoriaDAO necesita
        public RepositoryBase(ConexionDB conexion)
        {
            _conexion = conexion;
        }

        // Métodos abstractos que CategoriaDAO debe implementar
        public abstract Task<IEnumerable<T>> GetAllAsync();
        public abstract Task<T?> GetByIdAsync(int id);
        public abstract Task<int> CreateAsync(T entity);
        public abstract Task<bool> UpdateAsync(T entity);
        public abstract Task<bool> DeleteAsync(int id, int idUsuarioModificacion);

        // ¡Este es el método que te faltaba!
        protected async Task<SqlConnection> GetOpenConnectionAsync()
        {
            var cn = _conexion.ObtenerConexion();
            await cn.OpenAsync();
            return cn;
        }
    }
}