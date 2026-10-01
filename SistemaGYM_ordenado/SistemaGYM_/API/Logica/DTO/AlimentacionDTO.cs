using System.ComponentModel.DataAnnotations;

namespace SistemaGYM.Logica.DTOs;

public record AlimentacionDto(
    int Id,
    string TipoAlimentacion,
    string Descripcion,
    int ProfesorId,
    List<AlumnoAsignadoDto> Alumnos   // vacía = plan general (lo ven todos los alumnos)
);

public record AlimentacionCreateDto(
    [param: Required(ErrorMessage = "El tipo de alimentación es obligatorio")]
    [param: MaxLength(50, ErrorMessage = "El tipo de alimentación puede tener como máximo 50 caracteres")]
    string TipoAlimentacion,
    [param: Required(ErrorMessage = "La descripción del plan es obligatoria")]
    [param: MaxLength(700, ErrorMessage = "La descripción del plan puede tener como máximo 700 caracteres")]
    string Descripcion,
    int ProfesorId,
    List<int>? AlumnoIds   // vacía o nula = plan general
);
