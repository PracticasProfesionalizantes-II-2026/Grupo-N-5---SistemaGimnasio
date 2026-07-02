using System.ComponentModel.DataAnnotations;

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
    bool EstaActivo
);

public record AlumnoCreateDto(
    int Dni, 
    string Nombre, 
    string Apellido, 
    string Direccion, 
    string Email, 
    string Telefono, 
    bool EstaActivo,

    [property: Required(ErrorMessage = "La contraseña es obligatoria")]
    [property: MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    string Contrasenia
);