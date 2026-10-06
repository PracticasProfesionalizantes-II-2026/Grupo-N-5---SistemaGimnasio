using System.ComponentModel.DataAnnotations;

namespace SistemaGYM.Logica.DTOs;

public record AnuncioDto(
    int Id, 
    string Titulo, 
    string Descripcion, 
    DateTime FechaPublicacion, 
    int ProfesorId
);

public record AnuncioCreateDto(
    [Required(ErrorMessage = "El título es obligatorio")]
    [MaxLength(70, ErrorMessage = "El título puede tener como máximo 70 caracteres")]
    string Titulo,

    [Required(ErrorMessage = "La descripción es obligatoria")]
    [MaxLength(500, ErrorMessage = "La descripción puede tener como máximo 500 caracteres")]
    string Descripcion,

    DateTime FechaPublicacion,

    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un profesor válido")]
    int ProfesorId
);