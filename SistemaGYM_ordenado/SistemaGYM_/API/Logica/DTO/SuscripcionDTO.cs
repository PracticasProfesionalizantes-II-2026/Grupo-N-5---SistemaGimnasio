using System.ComponentModel.DataAnnotations;

namespace SistemaGYM.Logica.DTOs;

public record SuscripcionDto(
    int Id,
    string Nombre,
    decimal Precio,
    int DuracionDias,
    int DiasPorSemana
);

public record SuscripcionCreateDto(
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(100)]
    string Nombre,

    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
    decimal Precio,

    [Range(1, 7, ErrorMessage = "La frecuencia debe estar entre 1 y 7 días por semana")]
    int DiasPorSemana
);
