using System.ComponentModel.DataAnnotations;

namespace SistemaGYM.Logica.DTOs;

public record PagoDto(
    int Id, 
    decimal Monto, 
    DateTime FechaPago, 
    MetodoPago MetodoPago, 
    int AlumnoId, 
    int AlumnoSuscripcionId
);

public record PagoCreateDto(
    [property: Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
    decimal Monto,

    DateTime FechaPago,
    MetodoPago MetodoPago,

    [property: Range(1, int.MaxValue, ErrorMessage = "Debe indicar un alumno válido")]
    int AlumnoId,

    [property: Range(1, int.MaxValue, ErrorMessage = "Debe indicar una suscripción válida")]
    int AlumnoSuscripcionId
);