namespace SistemaGYM.Logica.DTOs;

public record RutinaDto(
    int Id, 
    string Nombre, 
    string Descripcion, 
    int ProfesorId, 
    int AlumnoId
);

public record RutinaCreateDto(
    string Nombre, 
    string Descripcion, 
    int ProfesorId, 
    int AlumnoId
);