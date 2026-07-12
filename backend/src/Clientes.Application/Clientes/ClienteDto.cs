using Clientes.Domain.Clientes;

namespace Clientes.Application.Clientes;

public record ClienteDto(
    Guid Id,
    string Nombre,
    string Rfc,
    string Email,
    string? Telefono,
    string Estado,
    DateTime FechaAltaUtc,
    uint Version,
    IReadOnlyList<string> TransicionesPermitidas)
{
    public static ClienteDto DesdeEntidad(Cliente c) => new(
        c.Id,
        c.Nombre,
        c.Rfc,
        c.Email,
        c.Telefono,
        c.Estado.ToString(),
        c.FechaAlta,
        c.Version,
        TransicionesEstadoCliente.DestinosDesde(c.Estado)
        .Select(e => e.ToString())
        .ToList()
    );
}