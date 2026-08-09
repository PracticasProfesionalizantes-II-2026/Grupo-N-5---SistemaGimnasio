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
    [Range(1, int.MaxValue, ErrorMessage = "La duración debe ser de al menos 1 día")]
    public int DuracionDias { get; set; }
    public ICollection<AlumnoSuscripcion> AlumnoSuscripciones { get; set; } = new List<AlumnoSuscripcion>();
}