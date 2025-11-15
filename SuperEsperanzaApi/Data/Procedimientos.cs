namespace SuperEsperanzaApi.Data
{
    internal static class Procedimientos
    {
        public const string SP_VALIDAR_USUARIO = "sp_ValidarUsuario_Super";

        // Roles
        public const string SP_LISTAR_ROLES = "sp_ListarRoles";
        public const string SP_OBTENER_ROL_POR_ID = "sp_ObtenerRolPorId";
        public const string SP_INSERTAR_ROL = "sp_InsertarRol";
        public const string SP_ACTUALIZAR_ROL = "sp_ActualizarRol";
        public const string SP_ELIMINAR_ROL = "sp_EliminarRol";

        // (Añadir más constantes para otros sp: categorias, proveedores, clientes...)
        // ... dentro de la clase Procedimientos
        public const string SP_LISTAR_CATEGORIAS = "sp_ListarCategorias";
        public const string SP_OBTENER_CATEGORIA_POR_ID = "sp_ObtenerCategoriaPorId";
        public const string SP_INSERTAR_CATEGORIA = "sp_InsertarCategoria";
        public const string SP_ACTUALIZAR_CATEGORIA = "sp_ActualizarCategoria";
        public const string SP_ELIMINAR_CATEGORIA = "sp_EliminarCategoria";
    }
}
