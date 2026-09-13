using Coink.Usuarios.Application.Catalogos.Dtos;
using Coink.Usuarios.Application.Usuarios.Dtos;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Coink.Usuarios.Api.Swagger;

/// <summary>
/// Ejemplos realistas en la documentación. Los valores no son de relleno: usan
/// los identificadores que carga el script semilla, de modo que quien abra
/// Swagger pueda pulsar "Try it out" y obtener un 201 sin buscar datos válidos.
/// </summary>
internal sealed class EjemplosSchemaFilter : ISchemaFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiSchema esquema, SchemaFilterContext contexto)
    {
        if (contexto.Type == typeof(RegistrarUsuarioRequest))
        {
            esquema.Example = new OpenApiObject
            {
                ["nombre"] = new OpenApiString("Xiomara Zapata"),
                ["telefono"] = new OpenApiString("+573001234567"),
                ["paisId"] = new OpenApiInteger(1),
                ["departamentoId"] = new OpenApiInteger(1),
                ["municipioId"] = new OpenApiInteger(1),
                ["direccion"] = new OpenApiString("Calle 10 # 43-12 Apto 301")
            };
        }
        else if (contexto.Type == typeof(UsuarioRegistradoResponse))
        {
            esquema.Example = new OpenApiObject
            {
                ["usuarioId"] = new OpenApiInteger(1),
                ["fechaRegistro"] = new OpenApiString("2026-09-12T21:48:24.027870+00:00")
            };
        }
        else if (contexto.Type == typeof(UsuarioDetalleResponse))
        {
            esquema.Example = new OpenApiObject
            {
                ["usuarioId"] = new OpenApiInteger(1),
                ["nombre"] = new OpenApiString("Xiomara Zapata"),
                ["telefono"] = new OpenApiString("+573001234567"),
                ["direccion"] = new OpenApiString("Calle 10 # 43-12 Apto 301"),
                ["pais"] = new OpenApiString("Colombia"),
                ["departamento"] = new OpenApiString("Antioquia"),
                ["municipio"] = new OpenApiString("Medellín"),
                ["fechaRegistro"] = new OpenApiString("2026-09-12T21:48:24.027870+00:00")
            };
        }
        else if (contexto.Type == typeof(PaisResponse))
        {
            esquema.Example = new OpenApiObject
            {
                ["paisId"] = new OpenApiInteger(1),
                ["codigo"] = new OpenApiString("COL"),
                ["nombre"] = new OpenApiString("Colombia")
            };
        }
        else if (contexto.Type == typeof(DepartamentoResponse))
        {
            esquema.Example = new OpenApiObject
            {
                ["departamentoId"] = new OpenApiInteger(1),
                ["paisId"] = new OpenApiInteger(1),
                ["codigo"] = new OpenApiString("05"),
                ["nombre"] = new OpenApiString("Antioquia")
            };
        }
        else if (contexto.Type == typeof(MunicipioResponse))
        {
            esquema.Example = new OpenApiObject
            {
                ["municipioId"] = new OpenApiInteger(1),
                ["departamentoId"] = new OpenApiInteger(1),
                ["codigo"] = new OpenApiString("05001"),
                ["nombre"] = new OpenApiString("Medellín")
            };
        }
    }
}
