using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SuperEsperanzaApi.Data;
using SuperEsperanzaApi.Models;
using System.Data;


namespace SuperEsperanzaApi.Dao
{
    public class UsuarioDAO
    {
        private readonly ConexionDB _conexion;
        private readonly ILogger<UsuarioDAO> _logger;
        
        public UsuarioDAO(ConexionDB conexion, ILogger<UsuarioDAO> logger)
        {
            _conexion = conexion;
            _logger = logger;
        }

        public async Task<Usuario?> ValidarUsuarioAsync(string codigoUsuario, string contrasena)
        {
            try
            {
                using var conn = _conexion.ObtenerConexion();
                await conn.OpenAsync();
                using var cmd = new SqlCommand(Procedimientos.SP_VALIDAR_USUARIO, conn)
                { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.Add(new SqlParameter("@CodigoUsuario", SqlDbType.NVarChar) { Value = codigoUsuario ?? (object)DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Contrasena", SqlDbType.NVarChar) { Value = contrasena ?? (object)DBNull.Value });

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Usuario
                    {
                        Id = Convert.ToInt32(reader["IdUsuario"]),
                        NombreUsuario = reader["NombreUsuario"].ToString() ?? string.Empty,
                        Rol = reader["NombreRol"].ToString() ?? string.Empty
                    };
                }
                return null;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error de SQL al validar usuario. Número: {Number}, Mensaje: {Message}", ex.Number, ex.Message);
                throw new InvalidOperationException($"Error de base de datos al validar usuario: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al validar usuario. Tipo: {Type}, Mensaje: {Message}", ex.GetType().Name, ex.Message);
                throw new InvalidOperationException($"Error inesperado al validar usuario: {ex.Message}", ex);
            }
        }
    }
}