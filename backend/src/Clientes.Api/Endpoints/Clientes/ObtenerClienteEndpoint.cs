using Clientes.Application.Clientes;
using Clientes.Application.Clientes.CasosDeUso;
using FastEndpoints;

namespace Clientes.Api.Endpoints.Clientes;

public class ObtenerClienteRequest
{
    public Guid Id { get; set; }
}

public class ObtenerClienteEndpoint : Endpoint<ObtenerClienteRequest, ClienteDto>
{
    private readonly ObtenerCliente _casoDeUso;

    public ObtenerClienteEndpoint(ObtenerCliente casoDeUso)
    {
        _casoDeUso = casoDeUso;
    }

    public override void Configure()
    {
        Get("/clientes/{id}");
        Roles("Operador");
    }

    public override async Task HandleAsync(ObtenerClienteRequest req, CancellationToken ct)
    {
        var dto = await _casoDeUso.EjecutarAsync(req.Id, ct);
        await Send.ResponseAsync(dto, cancellation: ct);
    }
}