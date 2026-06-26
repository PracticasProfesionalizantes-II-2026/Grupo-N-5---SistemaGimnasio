namespace SistemaGYM.Logica.DTOs;

public record AnuncioDto(
    int Id, 
    string Titulo, 
    string Descripcion, 
    string FechaPublicacion, 
    int ProfesorId
);

public record AnuncioCreateDto(
    string Titulo, 
    string Descripcion, 
    string FechaPublicacion, 
    int ProfesorId
);