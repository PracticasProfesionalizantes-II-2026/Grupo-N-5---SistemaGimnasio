using SistemaGYM.Logica;
using SistemaGYM.Logica.DTOs;

namespace SistemaGYM.Endpoints;

public static class PagoEndpoints
{
    public static void MapPagoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/pagos");

        group.MapGet("/", async (IPagoLogica logica) =>
        {
            var lista = await logica.ObtenerTodosAsync();
            return Results.Ok(lista);
        });

        group.MapGet("/{id:int}", async (int id, IPagoLogica logica) =>
        {
            var item = await logica.ObtenerPorIdAsync(id);
            return item is not null ? Results.Ok(item) : Results.NotFound();
        });

        group.MapPost("/", async (PagoCreateDto dto, IPagoLogica logica) =>
        {
            var creado = await logica.CrearAsync(dto);
            return Results.Created($"/api/pagos/{creado.Id}", creado);
        });

        group.MapPut("/{id:int}", async (int id, PagoCreateDto dto, IPagoLogica logica) =>
        {
            var modificado = await logica.ActualizarAsync(id, dto);
            return modificado ? Results.Ok() : Results.NotFound();
        });

        group.MapDelete("/{id:int}", async (int id, IPagoLogica logica) =>
        {
            var eliminado = await logica.EliminarAsync(id);
            return eliminado ? Results.NoContent() : Results.NotFound();
        });
    }
}