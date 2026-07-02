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

            return Results.Ok(new { status = 200, message = "Pagos obtenidos correctamente", data = lista });
        })
        .WithName("GetAllPagos")
        .WithSummary("Obtiene todos los pagos")
        .WithDescription("Retorna la lista completa de pagos registrados en el sistema.")
        .WithTags("Pago")
        .Produces<IEnumerable<PagoDto>>(200)
        .Produces(500);

        group.MapGet("/{id:int}", async (int id, IPagoLogica logica) =>
        {
            var item = await logica.ObtenerPorIdAsync(id);
            if (item is null)
                return Results.Json(new { status = 404, message = "Pago no encontrado" }, statusCode: 404);

            return Results.Ok(new { status = 200, message = "Pago encontrado", data = item });
        })
        .WithName("GetPagoById")
        .WithSummary("Obtiene un pago por ID")
        .WithDescription("Retorna los datos de un pago específico según su ID.")
        .WithTags("Pago")
        .Produces<PagoDto>(200)
        .Produces(404)
        .Produces(500);

        group.MapPost("/", async (PagoCreateDto dto, IPagoLogica logica) =>
        {
            var creado = await logica.CrearAsync(dto);
            return Results.Json(new { status = 201, message = "Pago registrado correctamente", data = creado }, statusCode: 201);
        })
        .WithName("CreatePago")
        .WithSummary("Registra un nuevo pago")
        .WithDescription("Registra un pago de un alumno asociado a una suscripción.")
        .WithTags("Pago")
        .Produces<PagoDto>(201)
        .Produces(400)
        .Produces(500);

        group.MapPut("/{id:int}", async (int id, PagoCreateDto dto, IPagoLogica logica) =>
        {
            var modificado = await logica.ActualizarAsync(id, dto);
            if (!modificado)
                return Results.Json(new { status = 404, message = "Pago no encontrado" }, statusCode: 404);

            return Results.Ok(new { status = 200, message = "Pago actualizado correctamente" });
        })
        .WithName("UpdatePago")
        .WithSummary("Actualiza un pago existente")
        .WithDescription("Modifica los datos de un pago ya registrado.")
        .WithTags("Pago")
        .Produces(200)
        .Produces(404)
        .Produces(400)
        .Produces(500);

        group.MapDelete("/{id:int}", async (int id, IPagoLogica logica) =>
        {
            var eliminado = await logica.EliminarAsync(id);
            if (!eliminado)
                return Results.Json(new { status = 404, message = "Pago no encontrado" }, statusCode: 404);

            return Results.NoContent();
        })
        .WithName("DeletePago")
        .WithSummary("Elimina un pago")
        .WithDescription("Elimina un registro de pago del sistema.")
        .WithTags("Pago")
        .Produces(204)
        .Produces(404)
        .Produces(500);
    }
}
