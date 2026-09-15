using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGYM.Entidades;
public class ActividadAlumno
{
    [Key]
    public int Id { get; set; }
    public int AlumnoId { get; set; }
    [ForeignKey("AlumnoId")]
    public Alumno Alumno { get; set; } = null!;
    public int ActividadId { get; set; }
    [ForeignKey("ActividadId")]
    public Actividad Actividad { get; set; } = null!;
    public DateTime FechaInscripcion { get; set; } = DateTime.Now;
    public bool Activa { get; set; } = true;
    
}