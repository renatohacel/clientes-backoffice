using Clientes.Domain.Comunes;

namespace Clientes.Domain.Clientes;

public class Cliente
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = null!;
    public string Rfc { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string? Telefono { get; private set; }
    public EstadoCliente Estado { get; private set; }
    public DateTime FechaAlta { get; private set; }


    private Cliente() { }

    public static Cliente Crear(string nombre, string rfc, string email, string? telefono)
    {
        return new Cliente
        {
            Id = Guid.NewGuid(),
            Nombre = nombre.Trim(),
            Rfc = rfc.Trim().ToUpperInvariant(),
            Email = email.Trim().ToLowerInvariant(),
            Telefono = string.IsNullOrWhiteSpace(telefono) ? null : telefono.Trim(),
            Estado = EstadoCliente.Activo,
            FechaAlta = DateTime.UtcNow
        };
    }

    public void ActualizarDatos(string nombre, string rfc, string email, string? telefono)
    {
        Nombre = nombre.Trim();
        Rfc = rfc.Trim().ToUpperInvariant();
        Email = email.Trim().ToLowerInvariant();
        Telefono = string.IsNullOrWhiteSpace(telefono) ? null : telefono.Trim();
    }

    public void CambiarEstado(EstadoCliente nuevoEstado)
    {
        if (!TransicionesEstadoCliente.EsValida(Estado, nuevoEstado))
        {
            throw new ReglaDeNegocioException(
                codigo: "transicion_invalida",
                mensaje: $"No se permite la transición de estado de '{Estado}' a '{nuevoEstado}'."
            );
        }

        Estado = nuevoEstado;
    }
    public uint Version { get; private set; }
}