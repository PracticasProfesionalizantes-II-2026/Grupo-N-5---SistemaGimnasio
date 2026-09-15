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
    [MaxLength(70)]
    string Titulo,

    [Required(ErrorMessage = "La descripción es obligatoria")]
    [MaxLength(500)]
    string Descripcion,

    DateTime FechaPublicacion,

    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un profesor válido")]
    int ProfesorId
);