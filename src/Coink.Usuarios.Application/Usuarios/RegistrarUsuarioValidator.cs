using Coink.Usuarios.Application.Usuarios.Dtos;
using FluentValidation;

namespace Coink.Usuarios.Application.Usuarios;


public sealed class RegistrarUsuarioValidator : AbstractValidator<RegistrarUsuarioRequest>
{
    // Letras de cualquier alfabeto con sus tildes (incluye ñ), espacios, apóstrofo y guion.
    private const string PatronNombre = @"^[\p{L}\p{M}][\p{L}\p{M}'’\- ]*$";

    // '+', indicativo que no empieza en cero y entre 8 y 15 dígitos en total.
    private const string PatronTelefono = @"^\+[1-9][0-9]{7,14}$";

    // patrón direcciones colombianas reales: letras, dígitos y # - . , ° /
    private const string PatronDireccion = @"^[\p{L}\p{M}0-9 #\-\.,°/]+$";

    /// <summary>Define las reglas de validación del registro.</summary>
    public RegistrarUsuarioValidator()
    {
        // Se reportan todos los errores de validación en cada campo, no se detiene en el primero.

        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Stop;

 
        RuleFor(peticion => peticion.NombreNormalizado)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .Length(2, 100).WithMessage("El nombre debe tener entre 2 y 100 caracteres.")
            .Matches(PatronNombre).WithMessage("El nombre solo puede contener letras, espacios, apóstrofos y guiones.")
            .OverridePropertyName("nombre");

        RuleFor(peticion => peticion.TelefonoNormalizado)
            .NotEmpty().WithMessage("El teléfono es obligatorio.")
            .Matches(PatronTelefono).WithMessage("El teléfono debe tener formato internacional, por ejemplo +573001234567.")
            .OverridePropertyName("telefono");

        RuleFor(peticion => peticion.DireccionNormalizada)
            .NotEmpty().WithMessage("La dirección es obligatoria.")
            .Length(5, 200).WithMessage("La dirección debe tener entre 5 y 200 caracteres.")
            .Matches(PatronDireccion).WithMessage("La dirección contiene caracteres no permitidos.")
            .Must(direccion => direccion.Any(char.IsLetterOrDigit))
                .WithMessage("La dirección debe contener al menos una letra o un número.")
            .OverridePropertyName("direccion");

        RuleFor(peticion => peticion.PaisId)
            .GreaterThan(0).WithMessage("El identificador de país debe ser mayor que cero.")
            .OverridePropertyName("paisId");

        RuleFor(peticion => peticion.DepartamentoId)
            .GreaterThan(0).WithMessage("El identificador de departamento debe ser mayor que cero.")
            .OverridePropertyName("departamentoId");

        RuleFor(peticion => peticion.MunicipioId)
            .GreaterThan(0).WithMessage("El identificador de municipio debe ser mayor que cero.")
            .OverridePropertyName("municipioId");
    }
}
