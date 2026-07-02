using System.ComponentModel.DataAnnotations;

namespace SistemaGYM.Logica.DTOs;

public record ActividadDto(
    int ActividadId, 
    string Nombre, 
    string Descripcion, 
    TimeOnly HoraInicio, 
    TimeOnly HoraFin, 
    int ProfesorId, 
    DiasSemana Dias
);

public record ActividadCreateDto(
    [property: Required(ErrorMessage = "El nombre es obligatorio")]
    [property: MaxLength(100)]
    string Nombre,

    [property: Required(ErrorMessage = "La descripción es obligatoria")]
    [property: MaxLength(500)]
    string Descripcion,

    TimeOnly HoraInicio,
    TimeOnly HoraFin,

    [property: Range(1, int.MaxValue, ErrorMessage = "Debe indicar un profesor válido")]
    int ProfesorId,

    DiasSemana Dias
);