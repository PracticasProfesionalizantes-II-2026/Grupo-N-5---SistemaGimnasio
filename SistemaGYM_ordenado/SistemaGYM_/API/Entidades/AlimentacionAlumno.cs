using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGYM.Entidades;

// Tabla intermedia: a qué alumnos está asignado cada plan de alimentación.
// Un plan sin filas en esta tabla es un plan general (lo ven todos los alumnos).
public class AlimentacionAlumno
{
    public int AlimentacionId { get; set; }
    [ForeignKey("AlimentacionId")]
    public Alimentacion Alimentacion { get; set; } = null!;
    public int AlumnoId { get; set; }
    [ForeignKey("AlumnoId")]
    public Alumno Alumno { get; set; } = null!;
}
