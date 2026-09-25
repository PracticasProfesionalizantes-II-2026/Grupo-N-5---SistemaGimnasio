using System.ComponentModel.DataAnnotations;

namespace SistemaGYM.Logica.DTOs;

public record AlumnoDto(
    int Id, 
    int Dni, 
    string Nombre, 
    string Apellido,
    string Email,
    string Suscripcion,   // nombre del plan activo o "Sin suscripción"
    DateTime FechaAlta
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
    [param: Range(1000000, 99999999, ErrorMessage = "El DNI debe tener 7 u 8 dígitos")]
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

    // No es [Required] porque al modificar un cliente se puede dejar vacía para no cambiarla.
    // Al crear, AlumnoLogica controla que venga cargada.
    [param: MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    string Contrasenia
);
