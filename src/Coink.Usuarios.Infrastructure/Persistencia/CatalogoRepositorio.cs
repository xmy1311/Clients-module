using Coink.Usuarios.Application.Catalogos.Dtos;
using Coink.Usuarios.Application.Catalogos.Interfaces;
using Npgsql;
using NpgsqlTypes;

namespace Coink.Usuarios.Infrastructure.Persistencia;

/// <summary>
/// Lecturas de catálogo, resueltas con funciones almacenadas.
/// </summary>
internal sealed class CatalogoRepositorio : ICatalogoRepositorio
{
    private readonly NpgsqlDataSource _origenDatos;

    public CatalogoRepositorio(NpgsqlDataSource origenDatos) => _origenDatos = origenDatos;

    public async Task<IReadOnlyList<PaisResponse>> ObtenerPaisesAsync(CancellationToken cancelacion)
    {
        await using var comando = _origenDatos.CreateCommand(Procedimientos.ListarPaises);
        await using var lector = await comando.ExecuteReaderAsync(cancelacion);

        var columnaId = lector.GetOrdinal("pais_id");
        var columnaCodigo = lector.GetOrdinal("codigo");
        var columnaNombre = lector.GetOrdinal("nombre");

        var paises = new List<PaisResponse>();

        while (await lector.ReadAsync(cancelacion))
        {
            paises.Add(new PaisResponse(
                lector.GetInt32(columnaId),
                lector.GetString(columnaCodigo),
                lector.GetString(columnaNombre)));
        }

        return paises;
    }

    public async Task<IReadOnlyList<DepartamentoResponse>> ObtenerDepartamentosAsync(int paisId, CancellationToken cancelacion)
    {
        await using var comando = _origenDatos.CreateCommand(Procedimientos.ListarDepartamentos);
        comando.Parameters.Add(new NpgsqlParameter("pais_id", NpgsqlDbType.Integer) { Value = paisId });

        await using var lector = await comando.ExecuteReaderAsync(cancelacion);

        var columnaId = lector.GetOrdinal("departamento_id");
        var columnaPais = lector.GetOrdinal("pais_id");
        var columnaCodigo = lector.GetOrdinal("codigo");
        var columnaNombre = lector.GetOrdinal("nombre");

        var departamentos = new List<DepartamentoResponse>();

        while (await lector.ReadAsync(cancelacion))
        {
            departamentos.Add(new DepartamentoResponse(
                lector.GetInt32(columnaId),
                lector.GetInt32(columnaPais),
                lector.GetString(columnaCodigo),
                lector.GetString(columnaNombre)));
        }

        return departamentos;
    }

    public async Task<IReadOnlyList<MunicipioResponse>> ObtenerMunicipiosAsync(int departamentoId, CancellationToken cancelacion)
    {
        await using var comando = _origenDatos.CreateCommand(Procedimientos.ListarMunicipios);
        comando.Parameters.Add(new NpgsqlParameter("departamento_id", NpgsqlDbType.Integer) { Value = departamentoId });

        await using var lector = await comando.ExecuteReaderAsync(cancelacion);

        var columnaId = lector.GetOrdinal("municipio_id");
        var columnaDepartamento = lector.GetOrdinal("departamento_id");
        var columnaCodigo = lector.GetOrdinal("codigo");
        var columnaNombre = lector.GetOrdinal("nombre");

        var municipios = new List<MunicipioResponse>();

        while (await lector.ReadAsync(cancelacion))
        {
            municipios.Add(new MunicipioResponse(
                lector.GetInt32(columnaId),
                lector.GetInt32(columnaDepartamento),
                lector.GetString(columnaCodigo),
                lector.GetString(columnaNombre)));
        }

        return municipios;
    }
}
