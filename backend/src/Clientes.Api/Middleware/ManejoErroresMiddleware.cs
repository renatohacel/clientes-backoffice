using Clientes.Domain.Comunes;

namespace Clientes.Api.Middleware;

public class ManejoErroresMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ManejoErroresMiddleware> _logger;

    public ManejoErroresMiddleware(RequestDelegate next, ILogger<ManejoErroresMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await _next(contexto);
        }
        catch (ReglaDeNegocioException ex)
        {
            await EscribirError(contexto, StatusCodes.Status422UnprocessableEntity, ex.Codigo, ex.Message);
        }
        catch (NoEncontradoException ex)
        {
            await EscribirError(contexto, StatusCodes.Status404NotFound, "no_encontrado", ex.Message);
        }
        catch (ConflictoConcurrenciaException ex)
        {
            await EscribirError(contexto, StatusCodes.Status409Conflict, "conflicto_concurrencia", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado en {Ruta}", contexto.Request.Path);
            await EscribirError(contexto, StatusCodes.Status500InternalServerError,
                "error_interno", "Ocurrió un error inesperado.");
        }
    }

    private static Task EscribirError(HttpContext contexto, int status, string codigo, string mensaje)
    {
        contexto.Response.StatusCode = status;
        return contexto.Response.WriteAsJsonAsync(new { codigo, mensaje });
    }
}