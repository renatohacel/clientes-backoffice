namespace Clientes.Domain.Comunes;

public class ConflictoConcurrenciaException : Exception
{
    public ConflictoConcurrenciaException() : base("El registro fue modificado por otro operador. Recarga los datos e intenta de nuevo.") { }
}