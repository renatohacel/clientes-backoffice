using Clientes.Application.Clientes;
using Clientes.Application.Clientes.CasosDeUso;
using FastEndpoints;

namespace Clientes.Api.Endpoints.Clientes;

public class ListarClientesRequest
{
    public string? Busqueda { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanoPagina { get; set; } = 10;
}

public class ListarClientesEndpoint : Endpoint<ListarClientesRequest, ResultadoPaginado<ClienteDto>>
{
    private readonly ListarClientes _casoDeUso;

    public ListarClientesEndpoint(ListarClientes casoDeUso)
    {
        _casoDeUso = casoDeUso;
    }

    public override void Configure()
    {
        Get("/clientes");
        Roles("Operador");
    }

    public override async Task HandleAsync(ListarClientesRequest req, CancellationToken ct)
    {
        var resultado = await _casoDeUso.EjecutarAsync(req.Busqueda, req.Pagina, req.TamanoPagina, ct);
        await Send.ResponseAsync(resultado, cancellation: ct);
    }
}