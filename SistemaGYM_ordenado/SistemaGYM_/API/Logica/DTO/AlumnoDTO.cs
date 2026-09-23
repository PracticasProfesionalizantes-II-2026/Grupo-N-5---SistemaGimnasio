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
    [param: Required(ErrorMessage = "El email es obligatorio")]
    [param: EmailAddress(ErrorMessage = "Ingresá un email válido")]
    string Email, 
    [param: MaxLength(14, ErrorMessage = "El teléfono debe tener como máximo 14 dígitos")]
    [param: RegularExpression("^[0-9]+$", ErrorMessage = "El teléfono solo puede contener números")]
    string Telefono, 
    bool EstaActivo,

    [ Required(ErrorMessage = "La contraseña es obligatoria")]
    [ MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    string Contrasenia
);
