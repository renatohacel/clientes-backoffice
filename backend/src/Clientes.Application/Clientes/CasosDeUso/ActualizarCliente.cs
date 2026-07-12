using Clientes.Application.Abstracciones;
using Clientes.Domain.Clientes;
using Clientes.Domain.Comunes;

namespace Clientes.Application.Clientes.CasosDeUso;

public class ActualizarCliente
{
    private readonly IClientesRepository _repo;
    public ActualizarCliente(IClientesRepository repo)
    {
        _repo = repo;
    }

    public async Task<ClienteDto> EjecutarAsync(Guid id, string nombre, string rfc, string email, string? telefono, uint version, CancellationToken ct)
    {
        var cliente = await _repo.ObtenerPorIdAsync(id, ct) ?? throw new NoEncontradoException($"No existe el cliente {id}");

        var rfcNormalizado = rfc.Trim().ToUpperInvariant();
        var emailNormalizado = email.Trim().ToLowerInvariant();


        if (await _repo.ExisteRfcAsync(rfcNormalizado, excluirId: id, ct))
            throw new ReglaDeNegocioException("rfc_duplicado", "Ya existe otro cliente con ese RFC.");

        if (await _repo.ExisteEmailAsync(emailNormalizado, excluirId: id, ct))
            throw new ReglaDeNegocioException("email_duplicado", "Ya existe otro cliente con ese email.");

        cliente.ActualizarDatos(nombre, rfc, email, telefono);
        await _repo.GuardarCambiosAsync(versionEsperada: version, cliente: cliente, ct);

        return ClienteDto.DesdeEntidad(cliente);
    }
}