namespace SistemaGYM.Logica.DTOs;

public record AlumnoSuscripcionDto(
    int Id,
    int SuscripcionId,
    string NombrePlan,
    decimal Precio,
    DateTime FechaInicio,
    DateTime? FechaFin,
    bool Activa
);

// Body del POST: solo se necesita indicar que plan se esta asignando
public record AsignarSuscripcionDto(
    int SuscripcionId
);