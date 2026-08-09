using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGYM.Entidades;
public class Pago
{
    [Key]
    public int Id { get; set; }
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Monto { get; set; }
    [Required]
    public DateTime FechaPago { get; set; } = DateTime.Now;
    [Required]
    public MetodoPago MetodoPago { get; set; }
    public int AlumnoId { get; set; }
    [ForeignKey("AlumnoId")]
    public Alumno Alumno { get; set; } = null!;
    public int AlumnoSuscripcionId { get; set; }
    [ForeignKey("AlumnoSuscripcionId")]
    public AlumnoSuscripcion AlumnoSuscripcion { get; set; } = null!;
}