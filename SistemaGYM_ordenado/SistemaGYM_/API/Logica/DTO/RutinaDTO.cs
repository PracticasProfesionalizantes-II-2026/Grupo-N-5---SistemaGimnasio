using System.ComponentModel.DataAnnotations;

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
    [param: Required(ErrorMessage = "El nombre de la rutina es obligatorio")]
    [param: MaxLength(70, ErrorMessage = "El nombre de la rutina puede tener como máximo 70 caracteres")]
    string Nombre, 
    [param: Required(ErrorMessage = "La descripción de la rutina es obligatoria")]
    [param: MaxLength(700, ErrorMessage = "La descripción de la rutina puede tener como máximo 700 caracteres")]
    string Descripcion, 
    int ProfesorId, 
    int AlumnoId,
    int? ActividadId
);
