using SistemaGYM.Logica;
using SistemaGYM.Logica.DTOs;

namespace SistemaGYM.Endpoints;

public static class AlimentacionEndpoints
{
    public static void MapAlimentacionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/alimentacion");

        group.MapGet("/", async (IAlimentacionLogica logica) =>
        {
            var lista = await logica.ObtenerTodosAsync();
            return Results.Ok(lista);
        });

        group.MapGet("/{id:int}", async (int id, IAlimentacionLogica logica) =>
        {
            var item = await logica.ObtenerPorIdAsync(id);
            return item is not null ? Results.Ok(item) : Results.NotFound();
        });

        group.MapPost("/", async (AlimentacionCreateDto dto, IAlimentacionLogica logica) =>
        {
            var creado = await logica.CrearAsync(dto);
            return Results.Created($"/api/alimentacion/{creado.Id}", creado);
        });

        group.MapPut("/{id:int}", async (int id, AlimentacionCreateDto dto, IAlimentacionLogica logica) =>
        {
            var modificado = await logica.ActualizarAsync(id, dto);
            return modificado ? Results.Ok() : Results.NotFound();
        });

        group.MapDelete("/{id:int}", async (int id, IAlimentacionLogica logica) =>
        {
            var eliminado = await logica.EliminarAsync(id);
            return eliminado ? Results.NoContent() : Results.NotFound();
        });
    }
}