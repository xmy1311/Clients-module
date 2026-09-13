using Coink.Usuarios.Application.Catalogos.Interfaces;
using Coink.Usuarios.Application.Usuarios.Interfaces;
using Coink.Usuarios.Infrastructure.Persistencia;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Coink.Usuarios.Infrastructure;

/// <summary>
/// Registro de la infraestructura. Encapsularlo aquí mantiene Program.cs como un
/// índice legible de la aplicación.
/// </summary>
public static class RegistroDependencias
{
    /// <summary>
    /// Registra el origen de datos de PostgreSQL y los repositorios.
    /// Si falta la cadena de conexión, la aplicación no arranca
    /// </summary>
    public static IServiceCollection AgregarInfraestructura(
        this IServiceCollection servicios,
        IConfiguration configuracion)
    {
        var cadenaConexion = configuracion.GetConnectionString("Default");

        if (string.IsNullOrWhiteSpace(cadenaConexion))
        {
            throw new InvalidOperationException(
                "Falta la cadena de conexión. Defina la variable de entorno 'ConnectionStrings__Default'.");
        }

        // Singleton: NpgsqlDataSource administra el pool de conexiones.
        servicios.AddSingleton(_ => new NpgsqlDataSourceBuilder(cadenaConexion).Build());

        servicios.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
        servicios.AddScoped<ICatalogoRepositorio, CatalogoRepositorio>();

        return servicios;
    }
}
