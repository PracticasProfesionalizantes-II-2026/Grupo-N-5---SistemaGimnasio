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

            return Results.Ok(new { status = 200, message = "Anuncios obtenidos correctamente", data = lista });
        })
        .WithName("GetAllAnuncios")
        .WithSummary("Obtiene todos los anuncios")
        .WithDescription("Retorna la lista completa de anuncios publicados en el sistema.")
        .WithTags("Anuncio")
        .Produces<IEnumerable<AnuncioDto>>(200)
        .Produces(500);

        group.MapGet("/{id:int}", async (int id, IAnuncioLogica logica) =>
        {
            var item = await logica.ObtenerPorIdAsync(id);
            if (item is null)
                return Results.Json(new { status = 404, message = "Anuncio no encontrado" }, statusCode: 404);

            return Results.Ok(new { status = 200, message = "Anuncio encontrado", data = item });
        })
        .WithName("GetAnuncioById")
        .WithSummary("Obtiene un anuncio por ID")
        .WithDescription("Retorna los datos de un anuncio específico según su ID.")
        .WithTags("Anuncio")
        .Produces<AnuncioDto>(200)
        .Produces(404)
        .Produces(500);

        group.MapPost("/", async (AnuncioCreateDto dto, IAnuncioLogica logica) =>
        {
            var creado = await logica.CrearAsync(dto);
            return Results.Json(new { status = 201, message = "Anuncio creado correctamente", data = creado }, statusCode: 201);
        })
        .WithName("CreateAnuncio")
        .WithSummary("Crea un nuevo anuncio")
        .WithDescription("Publica un nuevo anuncio asociado a un profesor.")
        .WithTags("Anuncio")
        .Produces<AnuncioDto>(201)
        .Produces(400)
        .Produces(500);

        group.MapPut("/{id:int}", async (int id, AnuncioCreateDto dto, IAnuncioLogica logica) =>
        {
            var modificado = await logica.ActualizarAsync(id, dto);
            if (!modificado)
                return Results.Json(new { status = 404, message = "Anuncio no encontrado" }, statusCode: 404);

            return Results.Ok(new { status = 200, message = "Anuncio actualizado correctamente" });
        })
        .WithName("UpdateAnuncio")
        .WithSummary("Actualiza un anuncio existente")
        .WithDescription("Modifica los datos de un anuncio ya publicado.")
        .WithTags("Anuncio")
        .Produces(200)
        .Produces(404)
        .Produces(400)
        .Produces(500);

        group.MapDelete("/{id:int}", async (int id, IAnuncioLogica logica) =>
        {
            var eliminado = await logica.EliminarAsync(id);
            if (!eliminado)
                return Results.Json(new { status = 404, message = "Anuncio no encontrado" }, statusCode: 404);

            return Results.NoContent();
        })
        .WithName("DeleteAnuncio")
        .WithSummary("Elimina un anuncio")
        .WithDescription("Elimina un anuncio del sistema.")
        .WithTags("Anuncio")
        .Produces(204)
        .Produces(404)
        .Produces(500);
    }
}
