namespace SistemaGYM.Logica.DTOs;

public record RutinaDto(
    int Id, 
    string Nombre, 
    string Descripcion, 
    int ProfesorId, 
    int AlumnoId,
    int? ActividadId,
    string? ActividadNombre,
    string? AlumnoNombre
);

public record RutinaCreateDto(
    string Nombre, 
    string Descripcion, 
    int ProfesorId, 
    int AlumnoId,
    int? ActividadId
);
