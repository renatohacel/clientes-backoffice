namespace Clientes.Domain.Comunes;
public class ReglaDeNegocioException : Exception
{
    public string Codigo { get; }

    public ReglaDeNegocioException(string codigo, string mensaje)
        : base(mensaje)
    {
        Codigo = codigo;
    }
}
