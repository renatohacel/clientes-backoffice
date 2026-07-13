using Clientes.Application.Abstracciones;
using Clientes.Domain.Clientes;

namespace Clientes.Tests.Aplicacion;
public class RepositorioFalso : IClientesRepository
{
    public List<Cliente> Clientes { get; } = [];
    public int VecesGuardado { get; private set; }

    public Task<Cliente?> ObtenerPorIdAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Clientes.FirstOrDefault(c => c.Id == id));

    public Task<bool> ExisteRfcAsync(string rfc, Guid? excluirId, CancellationToken ct) =>
        Task.FromResult(Clientes.Any(c => c.Rfc == rfc && c.Id != excluirId));

    public Task<bool> ExisteEmailAsync(string email, Guid? excluirId, CancellationToken ct) =>
        Task.FromResult(Clientes.Any(c => c.Email == email && c.Id != excluirId));

    public Task<(IReadOnlyList<Cliente> Items, int Total)> BuscarAsync(
        string? busqueda, int pagina, int tamanoPagina, CancellationToken ct)
    {
        var filtrados = string.IsNullOrWhiteSpace(busqueda)
            ? Clientes
            : Clientes.Where(c =>
                c.Nombre.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
                c.Rfc.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
                c.Email.Contains(busqueda, StringComparison.OrdinalIgnoreCase)).ToList();

        var items = filtrados
            .OrderBy(c => c.Nombre)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToList();

        return Task.FromResult<(IReadOnlyList<Cliente>, int)>((items, filtrados.Count));
    }

    public void Agregar(Cliente cliente) => Clientes.Add(cliente);

    public Task GuardarCambiosAsync(uint? versionEsperada, Cliente? cliente, CancellationToken ct)
    {
        VecesGuardado++;
        return Task.CompletedTask;
    }
}