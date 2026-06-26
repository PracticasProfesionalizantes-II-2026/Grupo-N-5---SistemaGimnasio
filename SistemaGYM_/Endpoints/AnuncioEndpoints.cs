using SistemaGYM.Logica;
using SistemaGYM.Logica.DTOs;

namespace SistemaGYM.Endpoints;

public static class AnuncioEndpoints
{
    public static void MapAnuncioEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/anuncios");

        group.MapGet("/", async (IAnuncioLogica logica) =>
        {
            var lista = await logica.ObtenerTodosAsync();
            return Results.Ok(lista);
        });

        group.MapGet("/{id:int}", async (int id, IAnuncioLogica logica) =>
        {
            var item = await logica.ObtenerPorIdAsync(id);
            return item is not null ? Results.Ok(item) : Results.NotFound();
        });

        group.MapPost("/", async (AnuncioCreateDto dto, IAnuncioLogica logica) =>
        {
            var creado = await logica.CrearAsync(dto);
            return Results.Created($"/api/anuncios/{creado.Id}", creado);
        });

        group.MapPut("/{id:int}", async (int id, AnuncioCreateDto dto, IAnuncioLogica logica) =>
        {
            var modificado = await logica.ActualizarAsync(id, dto);
            return modificado ? Results.Ok() : Results.NotFound();
        });

        group.MapDelete("/{id:int}", async (int id, IAnuncioLogica logica) =>
        {
            var eliminado = await logica.EliminarAsync(id);
            return eliminado ? Results.NoContent() : Results.NotFound();
        });
    }
}