namespace SistemaGYM.Logica.DTOs;

public record PagoDto(
    int Id, 
    decimal Monto, 
    string FechaPago, 
    string MetodoPago, 
    int AlumnoId, 
    int AlumnoSuscripcionId
);

public record PagoCreateDto(
    decimal Monto, 
    string FechaPago, 
    string MetodoPago, 
    int AlumnoId, 
    int AlumnoSuscripcionId
);