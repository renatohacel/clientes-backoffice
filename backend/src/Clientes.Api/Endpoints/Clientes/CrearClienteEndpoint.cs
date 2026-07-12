using Clientes.Application.Clientes;
using Clientes.Application.Clientes.CasosDeUso;
using FastEndpoints;
using FluentValidation;

namespace Clientes.Api.Endpoints.Clientes;

public class CrearClienteRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Rfc { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
}

public class CrearClienteValidator : Validator<CrearClienteRequest>
{
    public CrearClienteValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .Length(3, 120).WithMessage("El nombre debe tener entre 3 y 120 caracteres.");

        RuleFor(x => x.Rfc)
            .NotEmpty().WithMessage("El RFC es requerido.")
            .Matches(@"(?i)^[A-ZÑ&]{3,4}\d{6}[A-Z0-9]{3}$")
            .WithMessage("El RFC no tiene un formato válido.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.")
            .MaximumLength(254);

        RuleFor(x => x.Telefono)
            .Matches(@"^\+?\d{10,15}$")
            .WithMessage("El teléfono debe tener de 10 a 15 dígitos.")
            .When(x => !string.IsNullOrWhiteSpace(x.Telefono));
    }
}

public class CrearClienteEndpoint : Endpoint<CrearClienteRequest, ClienteDto>
{
    private readonly CrearCliente _casoDeUso;

    public CrearClienteEndpoint(CrearCliente casoDeUso)
    {
        _casoDeUso = casoDeUso;
    }

    public override void Configure()
    {
        Post("/clientes");
        Roles("Operador");
    }

    public override async Task HandleAsync(CrearClienteRequest req, CancellationToken ct)
    {
        var dto = await _casoDeUso.EjecutarAsync(req.Nombre, req.Rfc, req.Email, req.Telefono, ct);
        await Send.ResponseAsync(dto, StatusCodes.Status201Created, ct);
    }
}