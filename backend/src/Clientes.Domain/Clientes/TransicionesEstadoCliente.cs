namespace Clientes.Domain.Clientes;
public static class TransicionesEstadoCliente
{
    private static readonly IReadOnlyDictionary<EstadoCliente, EstadoCliente[]> Permitidas =
        new Dictionary<EstadoCliente, EstadoCliente[]>
        {
            [EstadoCliente.Activo] = [EstadoCliente.Inactivo, EstadoCliente.Suspendido],
            [EstadoCliente.Inactivo] = [EstadoCliente.Activo],
            [EstadoCliente.Suspendido] = [EstadoCliente.Activo]
        };

    public static bool EsValida(EstadoCliente desde, EstadoCliente hacia) =>
        desde != hacia
        && Permitidas.TryGetValue(desde, out var destinos)
        && destinos.Contains(hacia);

    public static IReadOnlyList<EstadoCliente> DestinosDesde(EstadoCliente desde) =>
        Permitidas.TryGetValue(desde, out var destinos) ? destinos : [];
}
