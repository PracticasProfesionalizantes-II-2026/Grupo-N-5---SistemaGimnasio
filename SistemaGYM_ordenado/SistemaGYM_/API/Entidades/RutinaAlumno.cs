using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGYM.Entidades;

// Tabla intermedia: a qué alumnos está asignada cada rutina.
// Una rutina sin filas en esta tabla es una rutina general (la ven todos los alumnos).
public class RutinaAlumno
{
    public int RutinaId { get; set; }
    [ForeignKey("RutinaId")]
    public Rutina Rutina { get; set; } = null!;
    public int AlumnoId { get; set; }
    [ForeignKey("AlumnoId")]
    public Alumno Alumno { get; set; } = null!;
}
