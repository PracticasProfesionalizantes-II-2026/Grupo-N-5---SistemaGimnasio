using System.ComponentModel.DataAnnotations;


public class Profesor : Usuario
{
    [MaxLength(100)]
    public string? Titulo {get; set;}
    [MaxLength(500)]
    public string? Descripcion { get; set; }
    public ICollection<Actividad> Actividades { get; set; } = new List<Actividad>();
    public ICollection<Alimentacion> PlanesAlimentacion { get; set; } = new List<Alimentacion>();
    public ICollection<Anuncio> Anuncios { get; set; } = new List<Anuncio>();
    public ICollection<Rutina> Rutinas { get; set; } = new List<Rutina>();
}