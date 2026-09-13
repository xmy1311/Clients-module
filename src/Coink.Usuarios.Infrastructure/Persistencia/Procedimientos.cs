namespace Coink.Usuarios.Infrastructure.Persistencia;

/// <summary>
/// Invocaciones a los objetos almacenados. Aquí no hay lógica SQL: solo la llamada.
/// </summary>
internal static class Procedimientos
{

    internal const string RegistrarUsuario =
        "CALL sp_registrar_usuario(@nombre, @telefono, @pais_id, @departamento_id, @municipio_id, @direccion, NULL, NULL, NULL)";

    internal const string ObtenerUsuario = "SELECT * FROM fn_obtener_usuario(@usuario_id)";

    internal const string ListarPaises = "SELECT * FROM fn_listar_paises()";

    internal const string ListarDepartamentos = "SELECT * FROM fn_listar_departamentos(@pais_id)";

    internal const string ListarMunicipios = "SELECT * FROM fn_listar_municipios(@departamento_id)";
}
