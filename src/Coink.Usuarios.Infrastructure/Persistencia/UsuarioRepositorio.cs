using Coink.Usuarios.Application.Usuarios.Dtos;
using Coink.Usuarios.Application.Usuarios.Interfaces;
using Coink.Usuarios.Application.Usuarios.Modelos;
using Npgsql;
using NpgsqlTypes;

namespace Coink.Usuarios.Infrastructure.Persistencia;

/// <summary>
/// Repositorio de usuarios.
/// </summary>
internal sealed class UsuarioRepositorio : IUsuarioRepositorio
{
    private readonly NpgsqlDataSource _origenDatos;

    public UsuarioRepositorio(NpgsqlDataSource origenDatos) => _origenDatos = origenDatos;

    public async Task<RespuestaRegistro> RegistrarAsync(NuevoUsuario usuario, CancellationToken cancelacion)
    {
        await using var comando = _origenDatos.CreateCommand(Procedimientos.RegistrarUsuario);

        comando.Parameters.Add(new NpgsqlParameter("nombre", NpgsqlDbType.Varchar) { Value = usuario.Nombre });
        comando.Parameters.Add(new NpgsqlParameter("telefono", NpgsqlDbType.Varchar) { Value = usuario.Telefono });
        comando.Parameters.Add(new NpgsqlParameter("pais_id", NpgsqlDbType.Integer) { Value = usuario.PaisId });
        comando.Parameters.Add(new NpgsqlParameter("departamento_id", NpgsqlDbType.Integer) { Value = usuario.DepartamentoId });
        comando.Parameters.Add(new NpgsqlParameter("municipio_id", NpgsqlDbType.Integer) { Value = usuario.MunicipioId });
        comando.Parameters.Add(new NpgsqlParameter("direccion", NpgsqlDbType.Varchar) { Value = usuario.Direccion });

        await using var lector = await comando.ExecuteReaderAsync(cancelacion);

        if (!await lector.ReadAsync(cancelacion))
        {
            throw new InvalidOperationException("sp_registrar_usuario no devolvió la fila de parámetros de salida.");
        }

        var codigo = lector.GetInt16(lector.GetOrdinal("p_codigo_resultado"));
        var columnaId = lector.GetOrdinal("p_usuario_id");
        var columnaFecha = lector.GetOrdinal("p_fecha_registro");

        int? usuarioId = await lector.IsDBNullAsync(columnaId, cancelacion)
            ? null
            : lector.GetInt32(columnaId);

        DateTimeOffset? fechaRegistro = await lector.IsDBNullAsync(columnaFecha, cancelacion)
            ? null
            : lector.GetFieldValue<DateTimeOffset>(columnaFecha);

        return new RespuestaRegistro(codigo, usuarioId, fechaRegistro);
    }


    public async Task<UsuarioDetalleResponse?> ObtenerPorIdAsync(int usuarioId, CancellationToken cancelacion)
    {
        await using var comando = _origenDatos.CreateCommand(Procedimientos.ObtenerUsuario);
        comando.Parameters.Add(new NpgsqlParameter("usuario_id", NpgsqlDbType.Integer) { Value = usuarioId });

        await using var lector = await comando.ExecuteReaderAsync(cancelacion);

        if (!await lector.ReadAsync(cancelacion))
        {
            return null;
        }

        return new UsuarioDetalleResponse(
            lector.GetInt32(lector.GetOrdinal("usuario_id")),
            lector.GetString(lector.GetOrdinal("nombre")),
            lector.GetString(lector.GetOrdinal("telefono")),
            lector.GetString(lector.GetOrdinal("direccion")),
            lector.GetString(lector.GetOrdinal("pais")),
            lector.GetString(lector.GetOrdinal("departamento")),
            lector.GetString(lector.GetOrdinal("municipio")),
            lector.GetFieldValue<DateTimeOffset>(lector.GetOrdinal("fecha_registro")));
    }
}
