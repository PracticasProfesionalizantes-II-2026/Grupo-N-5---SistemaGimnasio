using System.ComponentModel.DataAnnotations;

namespace SistemaGYM.Logica.DTOs;

public record ActividadDto(
    int ActividadId, 
    string Nombre, 
    string Descripcion, 
    TimeOnly HoraInicio, 
    TimeOnly HoraFin, 
    int ProfesorId, 
    DiasSemana Dias,
    int Cupo,
    int Inscriptos   // alumnos con inscripción activa (para mostrar "5 / 20")
);

public record ActividadCreateDto(
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(100)]
    string Nombre,

    [Required(ErrorMessage = "La descripción es obligatoria")]
    [MaxLength(500)]
    string Descripcion,

    TimeOnly HoraInicio,
    TimeOnly HoraFin,

    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un profesor válido")]
    int ProfesorId,

    DiasSemana Dias,

    [Range(1, 500, ErrorMessage = "El cupo debe estar entre 1 y 500 alumnos")]
    int Cupo
);