namespace SistemaGYM.Logica.DTOs;

public record AlumnoInscriptoDto(
    int AlumnoId,
    string Nombre,
    string Apellido,
    DateTime FechaInscripcion,
    bool Activa
);

public record InscripcionDto(
    int Id,
    int AlumnoId,
    int ActividadId,
    DateTime FechaInscripcion,
    bool Activa
);