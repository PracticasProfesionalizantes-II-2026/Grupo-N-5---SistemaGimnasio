using Microsoft.EntityFrameworkCore;
using SistemaGYM.Datos;
using SistemaGYM.Entidades;

namespace SistemaGYM.Repositorios;

public interface IActividadAlumnoRepository
{
    Task<ActividadAlumno?> ObtenerAsync(int alumnoId, int actividadId);
    Task<IEnumerable<ActividadAlumno>> ObtenerPorActividadAsync(int actividadId);
    Task AgregarAsync(ActividadAlumno entidad);
    Task ActualizarAsync(ActividadAlumno entidad);
}

public class ActividadAlumnoRepository : IActividadAlumnoRepository
{
    private readonly GimnasioContext _db;

    public ActividadAlumnoRepository(GimnasioContext db)
    {
        _db = db;
    }

    public async Task<ActividadAlumno?> ObtenerAsync(int alumnoId, int actividadId) =>
        await _db.ActividadesAlumno
            .FirstOrDefaultAsync(x => x.AlumnoId == alumnoId && x.ActividadId == actividadId);

    public async Task<IEnumerable<ActividadAlumno>> ObtenerPorActividadAsync(int actividadId) =>
        await _db.ActividadesAlumno
            .Include(x => x.Alumno)
            .Where(x => x.ActividadId == actividadId && x.Activa)
            .ToListAsync();

    public async Task AgregarAsync(ActividadAlumno entidad)
    {
        await _db.ActividadesAlumno.AddAsync(entidad);
        await _db.SaveChangesAsync();
    }

    public async Task ActualizarAsync(ActividadAlumno entidad)
    {
        _db.ActividadesAlumno.Update(entidad);
        await _db.SaveChangesAsync();
    }
}