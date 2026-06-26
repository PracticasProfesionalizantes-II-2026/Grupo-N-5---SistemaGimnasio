namespace SistemaGYM.Logica.DTOs;

public record SuscripcionDto(
    int Id, 
    string Descripcion
);

public record SuscripcionCreateDto(
    string Descripcion
);