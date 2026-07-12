using Clientes.Application.Clientes;
using Clientes.Application.Clientes.CasosDeUso;
using Clientes.Domain.Clientes;
using FastEndpoints;
using FluentValidation;

namespace Clientes.Api.Endpoints.Clientes;

public class CambiarEstadoClienteRequest
{
    public Guid Id { get; set; }
    public string NuevoEstado { get; set; } = string.Empty;
    public uint Version { get; set; }
}

public class CambiarEstadoClienteValidator : Validator<CambiarEstadoClienteRequest>
{
    public CambiarEstadoClienteValidator()
    {
        RuleFor(x => x.NuevoEstado)
            .NotEmpty()
            .Must(valor => Enum.TryParse<EstadoCliente>(valor, ignoreCase: true, out _))
            .WithMessage("El estado debe ser Activo, Inactivo o Suspendido.");
    }

    public class CambiarEstadoClienteEndpoint : Endpoint<CambiarEstadoClienteRequest, ClienteDto>
    {
        private readonly CambiarEstadoCliente _casoDeUso;

        public CambiarEstadoClienteEndpoint(CambiarEstadoCliente casoDeUso)
        {
            _casoDeUso = casoDeUso;
        }

        public override void Configure()
        {
            Post("/clientes/{id}/estado");
            Roles("Operador");
        }

        public override async Task HandleAsync(CambiarEstadoClienteRequest req, CancellationToken ct)
        {
            var estado = Enum.Parse<EstadoCliente>(req.NuevoEstado, ignoreCase: true);
            var dto = await _casoDeUso.EjecutarAsync(req.Id, estado, req.Version, ct);
            await Send.ResponseAsync(dto, cancellation: ct);
        }
    }
}