using Microsoft.EntityFrameworkCore;
using SistemaGYM.Datos;
using SistemaGYM.Entidades;

namespace SistemaGYM.Repositorios;

public interface IAlumnoRepository
{
    Task<IEnumerable<Alumno>> ObtenerTodosAsync();
    Task<Alumno?> ObtenerPorIdAsync(int id);
    Task<Alumno?> ObtenerDetallePorIdAsync(int id);
    Task AgregarAsync(Alumno alumno);
    Task ActualizarAsync(Alumno alumno);
    Task EliminarAsync(Alumno alumno);
}

public class AlumnoRepository : IAlumnoRepository
{
    private readonly GimnasioContext _db;

    public AlumnoRepository(GimnasioContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Alumno>> ObtenerTodosAsync() =>
        await _db.Alumnos.ToListAsync();

    public async Task<Alumno?> ObtenerPorIdAsync(int id) =>
        await _db.Alumnos.FindAsync(id);

    // Trae la suscripción activa del alumno para el endpoint /detalle
    public async Task<Alumno?> ObtenerDetallePorIdAsync(int id) =>
        await _db.Alumnos
            .Include(a => a.AlumnoSuscripciones.Where(s => s.Activa))
                .ThenInclude(s => s.Suscripcion)
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task AgregarAsync(Alumno alumno)
    {
        await _db.Alumnos.AddAsync(alumno);
        await _db.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Alumno alumno)
    {
        _db.Alumnos.Update(alumno);
        await _db.SaveChangesAsync();
    }

    public async Task EliminarAsync(Alumno alumno)
    {
        _db.Alumnos.Remove(alumno);
        await _db.SaveChangesAsync();
    }
}
