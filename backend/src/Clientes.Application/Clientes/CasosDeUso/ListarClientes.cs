using Clientes.Application.Abstracciones;

namespace Clientes.Application.Clientes.CasosDeUso;

public class ListarClientes
{
    private readonly IClientesRepository _repo;

    public ListarClientes(IClientesRepository repo)
    {
        _repo = repo;
    }

    public async Task<ResultadoPaginado<ClienteDto>> EjecutarAsync(string? busqueda, int pagina, int tamanoPagina, CancellationToken ct)
    {
        pagina = Math.Max(1, pagina);
        tamanoPagina = Math.Clamp(tamanoPagina, 1, 100);

        var (items, total) = await _repo.BuscarAsync(busqueda, pagina, tamanoPagina, ct);

        return new ResultadoPaginado<ClienteDto>(
            items.Select(ClienteDto.DesdeEntidad).ToList(),
            total,
            pagina,
            tamanoPagina
        );
    }
}