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
    [property: Required(ErrorMessage = "El título es obligatorio")]
    [property: MaxLength(70)]
    string Titulo,

    [property: Required(ErrorMessage = "La descripción es obligatoria")]
    [property: MaxLength(500)]
    string Descripcion,

    DateTime FechaPublicacion,

    [property: Range(1, int.MaxValue, ErrorMessage = "Debe indicar un profesor válido")]
    int ProfesorId
);