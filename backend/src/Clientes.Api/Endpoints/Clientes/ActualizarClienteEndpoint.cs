using Clientes.Application.Clientes;
using Clientes.Application.Clientes.CasosDeUso;
using FastEndpoints;
using FluentValidation;

namespace Clientes.Api.Endpoints.Clientes;

public class ActualizarClienteRequest
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Rfc { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public uint Version { get; set; }
}

public class ActualizarClienteValidator : Validator<ActualizarClienteRequest>
{
    public ActualizarClienteValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().Length(3, 120);
        RuleFor(x => x.Rfc).NotEmpty().Matches(@"(?i)^[A-ZÑ&]{3,4}\d{6}[A-Z0-9]{3}$");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.Telefono)
            .Matches(@"^\+?\d{10,15}$")
            .When(x => !string.IsNullOrWhiteSpace(x.Telefono));
    }
}
public class ActualizarClienteEndpoint : Endpoint<ActualizarClienteRequest, ClienteDto>
{
    private readonly ActualizarCliente _casoDeUso;

    public ActualizarClienteEndpoint(ActualizarCliente casoDeUso)
    {
        _casoDeUso = casoDeUso;
    }

    public override void Configure()
    {
        Put("/clientes/{id}");
        Roles("Operador");
    }

    public override async Task HandleAsync(ActualizarClienteRequest req, CancellationToken ct)
    {
        var dto = await _casoDeUso.EjecutarAsync(
            req.Id, req.Nombre, req.Rfc, req.Email, req.Telefono, req.Version, ct);

        await Send.ResponseAsync(dto, cancellation: ct);
    }
}