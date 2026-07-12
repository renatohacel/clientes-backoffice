namespace Clientes.Application.Clientes;

public record ResultadoPaginado<T>(
    IReadOnlyList<T> Items,
    int Total,
    int Pagina,
    int TamanoPagina
);