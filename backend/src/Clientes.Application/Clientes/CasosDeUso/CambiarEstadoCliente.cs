using Clientes.Application.Abstracciones;
using Clientes.Domain.Clientes;
using Clientes.Domain.Comunes;

namespace Clientes.Application.Clientes.CasosDeUso;

public class CambiarEstadoCliente
{
    private readonly IClientesRepository _repo;
    public CambiarEstadoCliente(IClientesRepository repo)
    {
        _repo = repo;
    }

    public async Task<ClienteDto> EjecutarAsyn(Guid id, EstadoCliente nuevoEstado, uint version, CancellationToken ct)
    {
        var cliente = await _repo.ObtenerPorIdAsync(id, ct) ?? throw new NoEncontradoException($"No existe el cliente {id}.");

        cliente.CambiarEstado(nuevoEstado);

        await _repo.GuardarCambiosAsync(versionEsperada: version, cliente: cliente, ct);

        return ClienteDto.DesdeEntidad(cliente);
    }
}