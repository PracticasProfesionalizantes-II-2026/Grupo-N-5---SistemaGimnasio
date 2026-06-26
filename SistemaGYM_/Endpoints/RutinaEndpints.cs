using SistemaGYM.Logica;
using SistemaGYM.Logica.DTOs;

namespace SistemaGYM.Endpoints;

public static class RutinaEndpoints
{
    public static void MapRutinaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/rutinas");

        group.MapGet("/", async (IRutinaLogica logica) =>
        {
            var lista = await logica.ObtenerTodosAsync();
            return Results.Ok(lista);
        });

        group.MapGet("/{id:int}", async (int id, IRutinaLogica logica) =>
        {
            var item = await logica.ObtenerPorIdAsync(id);
            return item is not null ? Results.Ok(item) : Results.NotFound();
        });

        group.MapPost("/", async (RutinaCreateDto dto, IRutinaLogica logica) =>
        {
            var creado = await logica.CrearAsync(dto);
            return Results.Created($"/api/rutinas/{creado.Id}", creado);
        });

        group.MapPut("/{id:int}", async (int id, RutinaCreateDto dto, IRutinaLogica logica) =>
        {
            var modificado = await logica.ActualizarAsync(id, dto);
            return modificado ? Results.Ok() : Results.NotFound();
        });

        group.MapDelete("/{id:int}", async (int id, IRutinaLogica logica) =>
        {
            var eliminado = await logica.EliminarAsync(id);
            return eliminado ? Results.NoContent() : Results.NotFound();
        });
    }
}