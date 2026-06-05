using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Suscripcion
{
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;
    [Column(TypeName = "decimal(10,2)")]
    public decimal Precio { get; set; }
    public int DuracionDias { get; set; }
    public ICollection<AlumnoSuscripcion> AlumnoSuscripciones { get; set; } = new List<AlumnoSuscripcion>();
}
