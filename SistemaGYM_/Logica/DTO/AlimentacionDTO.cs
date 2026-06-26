namespace SistemaGYM.Logica.DTOs;

public record AlimentacionDto(
    int Id, 
    string TipoAlimentacion, 
    string Descripcion, 
    int ProfesorId
);

public record AlimentacionCreateDto(
    string TipoAlimentacion, 
    string Descripcion, 
    int ProfesorId
);