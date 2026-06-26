namespace SistemaGYM.Logica.DTOs;

public record AlumnoDto(
    int Id, 
    int Dni, 
    string Nombre, 
    string Apellido
);

public record AlumnoDetalleDto(
    int Id, 
    int Dni, 
    string Nombre, 
    string Apellido, 
    string Direccion, 
    string Email, 
    string Telefono, 
    string Suscripcion,
    string EstaActivo
);

public record AlumnoCreateDto(
    int Dni, 
    string Nombre, 
    string Apellido, 
    string Direccion, 
    string Email, 
    string Telefono, 
    bool EstaActivo
);