using Clientes.Application.Abstracciones;
using Clientes.Domain.Clientes;
using Clientes.Domain.Comunes;

namespace Clientes.Application.Clientes.CasosDeUso;

public class CrearCliente
{
    private readonly IClientesRepository _repo;

    public CrearCliente(IClientesRepository repo)
    {
        _repo = repo;
    }

    public async Task<ClienteDto> EjecutarAsync(string nombre, string rfc, string email, string? telefono, CancellationToken ct)
    {
        var rfcNormalizado = rfc.Trim().ToUpperInvariant();
        var emailNormalizado = email.Trim().ToLowerInvariant();

        if (await _repo.ExisteRfcAsync(rfcNormalizado, excluirId: null, ct))
            throw new ReglaDeNegocioException("rfc_duplicado", "Ya existe un cliente con ese RFC.");

        if (await _repo.ExisteEmailAsync(emailNormalizado, excluirId: null, ct))
            throw new ReglaDeNegocioException("email_duplicado", "Ya existe un cliente con ese email.");

        var cliente = Cliente.Crear(nombre, rfc, email, telefono);
        _repo.Agregar(cliente);

        await _repo.GuardarCambiosAsync(versionEsperada: null, cliente: null, ct);

        return ClienteDto.DesdeEntidad(cliente);
    }
}

