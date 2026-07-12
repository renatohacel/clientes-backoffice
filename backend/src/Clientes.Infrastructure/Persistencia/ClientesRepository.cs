using Clientes.Application.Abstracciones;
using Clientes.Domain.Clientes;
using Clientes.Domain.Comunes;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Clientes.Infrasctructure.Persistencia;

public class ClientesRepository : IClientesRepository
{
    private readonly ClientesDbContext _db;

    public ClientesRepository(ClientesDbContext db)
    {
        _db = db;
    }

    public Task<Cliente?> ObtenerPorIdAsync(Guid id, CancellationToken ct) => _db.Clientes.FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<bool> ExisteRfcAsync(string rfc, Guid? excluirId, CancellationToken ct) => _db.Clientes.AnyAsync(c => c.Rfc == rfc && (excluirId == null || c.Id != excluirId), ct);

    public Task<bool> ExisteEmailAsync(string email, Guid? excluirId, CancellationToken ct) => _db.Clientes.AnyAsync(c => c.Email == email && (excluirId == null || c.Id != excluirId), ct);

    public async Task<(IReadOnlyList<Cliente> Items, int Total)> BuscarAsync(
        string? busqueda, int pagina, int tamanoPagina, CancellationToken ct)
    {
        var query = _db.Clientes.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var patron = $"%{busqueda.Trim()}%";
            query = query.Where(c =>
                EF.Functions.ILike(c.Nombre, patron) ||
                EF.Functions.ILike(c.Rfc, patron) ||
                EF.Functions.ILike(c.Email, patron));
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(c => c.Nombre)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync(ct);

        return (items, total);
    }

    public void Agregar(Cliente cliente) => _db.Clientes.Add(cliente);

    public async Task GuardarCambiosAsync(uint? versionEsperada, Cliente? cliente, CancellationToken ct)
    {
        if (versionEsperada is not null && cliente is not null)
        {
            _db.Entry(cliente).Property(c => c.Version).OriginalValue = versionEsperada.Value;
        }

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictoConcurrenciaException();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } pg)
        {
            var campo = pg.ConstraintName?.Contains("rfc", StringComparison.OrdinalIgnoreCase) == true
            ? ("rfc_duplicado", "Ya existe un cliente con ese RFC.")
            : ("email_duplicado", "Ya existe un cliente con ese email.");

            throw new ReglaDeNegocioException(campo.Item1, campo.Item2);
        }
    }
}