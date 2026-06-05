using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class AlumnoSuscripcion
{
    [Key]
    public int Id { get; set; }
    public int AlumnoId { get; set; }
    [ForeignKey("AlumnoId")]
    public Alumno Alumno { get; set; } = null!;
    public int SuscripcionId { get; set; }
    [ForeignKey("SuscripcionId")]
    public Suscripcion Suscripcion { get; set; } = null!;
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public bool Activa { get; set; } = true;
}