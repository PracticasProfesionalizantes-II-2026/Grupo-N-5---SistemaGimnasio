using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace SistemaGYM.Middleware;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Excepción no controlada en {Path}: {Message}",
            httpContext.Request.Path, exception.Message);

        var (statusCode, mensaje) = exception switch
        {
           
            DbUpdateException => (
                StatusCodes.Status409Conflict,
                "No se pudo completar la operación: los datos entran en conflicto con información existente o con una referencia inválida (por ejemplo, un ID que no existe)."
            ),
            JsonException or BadHttpRequestException => (
            StatusCodes.Status400BadRequest,
                "El formato de los datos enviados no es válido. Verificá los tipos y valores de cada campo."
            ),  

            _ => (
                StatusCodes.Status500InternalServerError,
                "Ocurrió un error inesperado en el servidor. Ya quedó registrado en los logs."
            )
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(new
        {
            status = statusCode,
            message = mensaje
        }, cancellationToken);

        return true;  
    }
}