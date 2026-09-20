using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGYM.Entidades;
public class Suscripcion
{
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;
    [Column(TypeName = "decimal(10,2)")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
    public decimal Precio { get; set; }
    // La vigencia comercial es siempre mensual. Se conserva la columna por compatibilidad
    // con los planes ya existentes en la base de datos.
    public int DuracionDias { get; set; } = 30;
    [Range(1, 7, ErrorMessage = "La frecuencia debe estar entre 1 y 7 días por semana")]
    public int DiasPorSemana { get; set; }
    public ICollection<AlumnoSuscripcion> AlumnoSuscripciones { get; set; } = new List<AlumnoSuscripcion>();
}
