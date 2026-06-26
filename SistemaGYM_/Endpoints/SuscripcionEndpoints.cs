using SistemaGYM.Logica;
using SistemaGYM.Logica.DTOs;

namespace SistemaGYM.Endpoints;

public static class SuscripcionEndpoints
{
    public static void MapSuscripcionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/suscripciones");

        group.MapGet("/", async (ISuscripcionLogica logica) =>
        {
            var lista = await logica.ObtenerTodosAsync();
            return Results.Ok(lista);
        });

        group.MapGet("/{id:int}", async (int id, ISuscripcionLogica logica) =>
        {
            var item = await logica.ObtenerPorIdAsync(id);
            return item is not null ? Results.Ok(item) : Results.NotFound();
        });

        group.MapPost("/", async (SuscripcionCreateDto dto, ISuscripcionLogica logica) =>
        {
            var creado = await logica.CrearAsync(dto);
            return Results.Created($"/api/suscripciones/{creado.Id}", creado);
        });

        group.MapPut("/{id:int}", async (int id, SuscripcionCreateDto dto, ISuscripcionLogica logica) =>
        {
            var modificado = await logica.ActualizarAsync(id, dto);
            return modificado ? Results.Ok() : Results.NotFound();
        });

        group.MapDelete("/{id:int}", async (int id, ISuscripcionLogica logica) =>
        {
            var eliminado = await logica.EliminarAsync(id);
            return eliminado ? Results.NoContent() : Results.NotFound();
        });
    }
}