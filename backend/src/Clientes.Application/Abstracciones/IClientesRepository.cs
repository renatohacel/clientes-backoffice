using Clientes.Domain.Clientes;

namespace Clientes.Application.Abstracciones;

public interface IClientesRepository
{
    Task<Cliente?> ObtenerPorIdAsync(Guid id, CancellationToken ct);
    Task<bool> ExisteRfcAsync(string rfc, Guid? excluirId, CancellationToken ct);
    Task<bool> ExisteEmailAsync(string email, Guid? excluirId, CancellationToken ct);
    Task<(IReadOnlyList<Cliente> Items, int Total)> BuscarAsync(
        string? busqueda, int pagina, int tamanoPagina, CancellationToken ct);
    void Agregar(Cliente cliente);
    Task GuardarCambiosAsync(uint? versionEsperada, Cliente? cliente, CancellationToken ct);
}