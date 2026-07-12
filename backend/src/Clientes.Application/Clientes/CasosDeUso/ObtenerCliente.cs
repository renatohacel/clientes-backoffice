using Clientes.Application.Abstracciones;
using Clientes.Domain.Comunes;

namespace Clientes.Application.Clientes.CasosDeUso;

public class ObtenerCliente
{
    private readonly IClientesRepository _repo;
    public ObtenerCliente(IClientesRepository repo)
    {
        _repo = repo;
    }

    public async Task<ClienteDto> EjecutarAsync(Guid id, CancellationToken ct)
    {
        var cliente = await _repo.ObtenerPorIdAsync(id, ct) ?? throw new NoEncontradoException($"No existe el cliente {id}");

        return ClienteDto.DesdeEntidad(cliente);
    }
}