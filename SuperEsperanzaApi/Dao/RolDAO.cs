using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SuperEsperanzaApi.Data;
using SuperEsperanzaApi.Models;
using System.Data;

namespace SuperEsperanzaApi.Dao
{
    public class RolDAO
    {
        private readonly ConexionDB _conexion;
        private readonly ILogger<RolDAO> _logger;

        public RolDAO(ConexionDB conexion, ILogger<RolDAO> logger)
        {
            _conexion = conexion;
            _logger = logger;
        }

        public async Task<IEnumerable<Rol>> ListarRolesAsync()
        {
            var resultados = new List<Rol>();
            try
            {
                using var conn = _conexion.ObtenerConexion();
                await conn.OpenAsync();
                using var cmd = new SqlCommand(Procedimientos.SP_LISTAR_ROLES, conn) { CommandType = CommandType.StoredProcedure };

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    resultados.Add(new Rol
                    {
                        Id = Convert.ToInt32(reader["Id_Rol"]),
                        NombreRol = reader["NombreRol"].ToString() ?? string.Empty
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar roles");
                throw;
            }

            return resultados;
        }

        public async Task<Rol?> ObtenerRolPorIdAsync(int id)
        {
            try
            {
                using var conn = _conexion.ObtenerConexion();
                await conn.OpenAsync();
                using var cmd = new SqlCommand(Procedimientos.SP_OBTENER_ROL_POR_ID, conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@Id_Rol", SqlDbType.Int) { Value = id });

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Rol
                    {
                        Id = Convert.ToInt32(reader["Id_Rol"]),
                        NombreRol = reader["NombreRol"].ToString() ?? string.Empty,
                        FechaCreacion = reader.IsDBNull(reader.GetOrdinal("FechaCreacion")) ? null : (DateTime?)Convert.ToDateTime(reader["FechaCreacion"]),
                        FechaModificacion = reader.IsDBNull(reader.GetOrdinal("FechaModificacion")) ? null : (DateTime?)Convert.ToDateTime(reader["FechaModificacion"])
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rol por id {Id}", id);
                throw;
            }
        }

        public async Task<int> InsertarRolAsync(string nombreRol, int idUsuarioCreacion)
        {
            try
            {
                using var conn = _conexion.ObtenerConexion();
                await conn.OpenAsync();
                using var cmd = new SqlCommand(Procedimientos.SP_INSERTAR_ROL, conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@NombreRol", SqlDbType.VarChar, 50) { Value = nombreRol });
                cmd.Parameters.Add(new SqlParameter("@Id_UsuarioCreacion", SqlDbType.Int) { Value = idUsuarioCreacion });

                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar rol {NombreRol}", nombreRol);
                throw;
            }
        }

        public async Task<bool> ActualizarRolAsync(int idRol, string nombreRol, int idUsuarioModificacion)
        {
            try
            {
                using var conn = _conexion.ObtenerConexion();
                await conn.OpenAsync();
                using var cmd = new SqlCommand(Procedimientos.SP_ACTUALIZAR_ROL, conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@Id_Rol", SqlDbType.Int) { Value = idRol });
                cmd.Parameters.Add(new SqlParameter("@NombreRol", SqlDbType.VarChar, 50) { Value = nombreRol });
                cmd.Parameters.Add(new SqlParameter("@Id_UsuarioModificacion", SqlDbType.Int) { Value = idUsuarioModificacion });

                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result) == 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar rol {Id}", idRol);
                throw;
            }
        }

        public async Task<bool> EliminarRolAsync(int idRol)
        {
            try
            {
                using var conn = _conexion.ObtenerConexion();
                await conn.OpenAsync();
                using var cmd = new SqlCommand(Procedimientos.SP_ELIMINAR_ROL, conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@Id_Rol", SqlDbType.Int) { Value = idRol });

                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result) == 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar rol {Id}", idRol);
                throw;
            }
        }
    }
}