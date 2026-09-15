using Microsoft.EntityFrameworkCore;
using SistemaGYM.Datos;
using SistemaGYM.Entidades;

namespace SistemaGYM.Repositorios;

public interface IAlumnoSuscripcionRepository
{
    Task<IEnumerable<AlumnoSuscripcion>> ObtenerPorAlumnoAsync(int alumnoId);
    Task<AlumnoSuscripcion?> ObtenerActivaPorAlumnoAsync(int alumnoId);
    Task<AlumnoSuscripcion?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(AlumnoSuscripcion entidad);
    Task ActualizarAsync(AlumnoSuscripcion entidad);
}

public class AlumnoSuscripcionRepository : IAlumnoSuscripcionRepository
{
    private readonly GimnasioContext _db;

    public AlumnoSuscripcionRepository(GimnasioContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<AlumnoSuscripcion>> ObtenerPorAlumnoAsync(int alumnoId) =>
        await _db.AlumnoSuscripciones
            .Include(x => x.Suscripcion)
            .Where(x => x.AlumnoId == alumnoId)
            .OrderByDescending(x => x.FechaInicio)
            .ToListAsync();

    public async Task<AlumnoSuscripcion?> ObtenerActivaPorAlumnoAsync(int alumnoId) =>
        await _db.AlumnoSuscripciones
            .Include(x => x.Suscripcion)
            .FirstOrDefaultAsync(x => x.AlumnoId == alumnoId && x.Activa);

    public async Task<AlumnoSuscripcion?> ObtenerPorIdAsync(int id) =>
        await _db.AlumnoSuscripciones
            .Include(x => x.Suscripcion)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task AgregarAsync(AlumnoSuscripcion entidad)
    {
        await _db.AlumnoSuscripciones.AddAsync(entidad);
        await _db.SaveChangesAsync();
    }

    public async Task ActualizarAsync(AlumnoSuscripcion entidad)
    {
        _db.AlumnoSuscripciones.Update(entidad);
        await _db.SaveChangesAsync();
    }
}