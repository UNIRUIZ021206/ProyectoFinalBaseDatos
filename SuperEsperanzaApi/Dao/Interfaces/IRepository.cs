namespace SuperEsperanzaApi.Dao.Interfaces
{
    // Esta es la interfaz genérica que tu CategoriaDAO intenta usar
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<int> CreateAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id, int idUsuarioModificacion);
    }
}
