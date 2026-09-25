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
    // La columna Monto es decimal(10,2): admite hasta 99.999.999,99
    [param: Range(0.01, 99999999.99, ErrorMessage = "El monto debe ser mayor a 0 y menor a 100.000.000")]
    decimal Monto,

    DateTime FechaPago,
    MetodoPago MetodoPago,

    [param: Range(1, int.MaxValue, ErrorMessage = "Debe indicar un alumno válido")]
    int AlumnoId,

    [param: Range(1, int.MaxValue, ErrorMessage = "Debe indicar una suscripción válida")]
    int AlumnoSuscripcionId
);
