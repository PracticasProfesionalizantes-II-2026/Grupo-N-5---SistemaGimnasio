namespace SistemaGYM.Logica.DTOs; 

public record ProfesorDto(int Id, int Dni, string Nombre, string Apellido, string Email, string Descripcion);

public record ProfesorDetalleDto(
    int Id, 
    int Dni, 
    string Nombre, 
    string Apellido, 
    string Direccion, 
    string Email, 
    long Telefono, 
    string Descripcion
    
);


public record ProfesorCreateDto(
    int Dni, 
    string Nombre, 
    string Apellido, 
    string Direccion, 
    string Email, 
    long Telefono, 
    string Descripcion
);