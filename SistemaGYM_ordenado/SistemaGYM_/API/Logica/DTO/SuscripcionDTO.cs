using System.ComponentModel.DataAnnotations;

namespace SistemaGYM.Logica.DTOs;

public record SuscripcionDto(
    int Id,
    string Nombre,
    decimal Precio,
    int DuracionDias
);

public record SuscripcionCreateDto(
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(100)]
    string Nombre,

    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
    decimal Precio,

    [Range(1, int.MaxValue, ErrorMessage = "La duración debe ser de al menos 1 día")]
    int DuracionDias
);