using Microsoft.Data.SqlClient;
using SuperEsperanzaApi.Dao.Interfaces;
using SuperEsperanzaApi.Data;
using SuperEsperanzaApi.Models;
using System.Data;

namespace SuperEsperanzaApi.Dao
{
    public class CategoriaDAO : RepositoryBase<Categoria>, IRepository<Categoria>
    {
        public CategoriaDAO(ConexionDB conexion) : base(conexion) { }

        public override async Task<IEnumerable<Categoria>> GetAllAsync()
        {
            var lista = new List<Categoria>();
            using var cn = await GetOpenConnectionAsync();
            using var cmd = new SqlCommand(Procedimientos.SP_LISTAR_CATEGORIAS, cn) { CommandType = CommandType.StoredProcedure };
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(MapReader(reader));
            }
            return lista;
        }

        public override async Task<Categoria?> GetByIdAsync(int id)
        {
            using var cn = await GetOpenConnectionAsync();
            using var cmd = new SqlCommand(Procedimientos.SP_OBTENER_CATEGORIA_POR_ID, cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id_Categoria", id);
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapReader(reader) : null;
        }

        public override async Task<int> CreateAsync(Categoria entity)
        {
            using var cn = await GetOpenConnectionAsync();
            using var cmd = new SqlCommand(Procedimientos.SP_INSERTAR_CATEGORIA, cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@CodigoCategoria", entity.CodigoCategoria);
            cmd.Parameters.AddWithValue("@Nombre", entity.Nombre);
            cmd.Parameters.AddWithValue("@Descripcion", (object?)entity.Descripcion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Id_UsuarioCreacion", entity.Id_UsuarioCreacion);

            var res = await cmd.ExecuteScalarAsync();
            return res != null ? Convert.ToInt32(res) : 0;
        }

        public override async Task<bool> UpdateAsync(Categoria entity)
        {
            using var cn = await GetOpenConnectionAsync();
            using var cmd = new SqlCommand(Procedimientos.SP_ACTUALIZAR_CATEGORIA, cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id_Categoria", entity.Id_Categoria);
            cmd.Parameters.AddWithValue("@CodigoCategoria", entity.CodigoCategoria);
            cmd.Parameters.AddWithValue("@Nombre", entity.Nombre);
            cmd.Parameters.AddWithValue("@Descripcion", (object?)entity.Descripcion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Estado", entity.Estado);
            cmd.Parameters.AddWithValue("@Id_UsuarioModificacion", entity.Id_UsuarioModificacion);

            var res = await cmd.ExecuteScalarAsync();
            return res != null; // El SP devuelve 1
        }

        public override async Task<bool> DeleteAsync(int id, int idUsuarioModificacion)
        {
            using var cn = await GetOpenConnectionAsync();
            using var cmd = new SqlCommand(Procedimientos.SP_ELIMINAR_CATEGORIA, cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id_Categoria", id);
            cmd.Parameters.AddWithValue("@Id_UsuarioModificacion", idUsuarioModificacion);

            var res = await cmd.ExecuteScalarAsync();
            return res != null;
        }

        private Categoria MapReader(SqlDataReader reader)
        {
            return new Categoria
            {
                Id_Categoria = Convert.ToInt32(reader["Id_Categoria"]),
                CodigoCategoria = reader["CodigoCategoria"].ToString() ?? "",
                Nombre = reader["Nombre"].ToString() ?? "",
                Descripcion = reader["Descripcion"] is DBNull ? null : reader["Descripcion"].ToString(),
                Estado = Convert.ToBoolean(reader["Estado"])
            };
        }
    }
}