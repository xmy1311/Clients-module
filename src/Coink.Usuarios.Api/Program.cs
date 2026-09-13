using System.Reflection;
using Coink.Usuarios.Api.Errores;
using Coink.Usuarios.Api.Filtros;
using Coink.Usuarios.Api.Swagger;
using Coink.Usuarios.Application.Usuarios;
using Coink.Usuarios.Application.Usuarios.Dtos;
using Coink.Usuarios.Application.Usuarios.Interfaces;
using Coink.Usuarios.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(opciones => opciones.Filters.Add<ValidacionFilter>());

// Un único formato de error para toda la API, incluidos los que genera el framework.
builder.Services.AddProblemDetails(opciones =>
    opciones.CustomizeProblemDetails = contexto =>
    {
        contexto.ProblemDetails.Instance ??= contexto.HttpContext.Request.Path;
        contexto.ProblemDetails.Extensions["traceId"] = Problemas.TraceId(contexto.HttpContext);
    });

builder.Services.AddExceptionHandler<ManejadorGlobalExcepciones>();

// Los 400 automáticos del enlace de modelo salen con la misma forma que los del validador.
builder.Services.Configure<ApiBehaviorOptions>(opciones =>
    opciones.InvalidModelStateResponseFactory = contexto =>
    {
        var problema = Problemas.DecorarValidacion(
            contexto.HttpContext,
            new ValidationProblemDetails(contexto.ModelState));

        return new ObjectResult(problema)
        {
            StatusCode = StatusCodes.Status400BadRequest,
            ContentTypes = { "application/problem+json" }
        };
    });

// Aplicación e infraestructura 
builder.Services.AddValidatorsFromAssemblyContaining<RegistrarUsuarioValidator>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AgregarInfraestructura(builder.Configuration);

// Documentación
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "COINK · API de registro de usuarios",
        Version = "v1",
        Description =
            "Registro de usuarios con validación de la cadena país → departamento → municipio. " +
            "Todas las consultas se resuelven mediante objetos almacenados en PostgreSQL.\n\n" +
            "**Datos de prueba ya cargados:** país 1 (Colombia), departamento 1 (Antioquia), " +
            "municipio 1 (Medellín). Para ver los errores de coherencia: país 1 + departamento 6 " +
            "(Cusco, Perú) devuelve 422; departamento 1 + municipio 7 (Bogotá) también.\n\n" +
            "**Formato de error:** todas las respuestas de error son `ProblemDetails` (RFC 9457) " +
            "con un campo `codigo` estable y un `traceId` para correlacionar con los registros."
    });

    // Marca como obligatorias las propiedades no anulables, en lugar de mostrarlas
    // todas como opcionales.
    opciones.SupportNonNullableReferenceTypes();

    // Ejemplos con los identificadores que carga el script semilla.
    opciones.SchemaFilter<EjemplosSchemaFilter>();

    foreach (var ensamblado in new[] { Assembly.GetExecutingAssembly(), typeof(RegistrarUsuarioRequest).Assembly })
    {
        var archivoXml = Path.Combine(AppContext.BaseDirectory, $"{ensamblado.GetName().Name}.xml");

        if (File.Exists(archivoXml))
        {
            opciones.IncludeXmlComments(archivoXml, includeControllerXmlComments: true);
        }
    }
});

var app = builder.Build();

// UseExceptionHandler va primero: envuelve todo lo que venga después.
app.UseExceptionHandler();

// Sin esto, una ruta mal escrita devolvería un 404 con cuerpo vacío.
app.UseStatusCodePages();

if (app.Configuration.GetValue("Swagger:Habilitado", true))
{
    app.UseSwagger();
    app.UseSwaggerUI(opciones =>
    {
        opciones.SwaggerEndpoint("/swagger/v1/swagger.json", "COINK v1");
        opciones.DocumentTitle = "COINK · API de registro de usuarios";
        opciones.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        opciones.DisplayRequestDuration();
    });
}

app.MapControllers();

app.Run();

public partial class Program;
