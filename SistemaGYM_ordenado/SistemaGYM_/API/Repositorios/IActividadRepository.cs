using Microsoft.EntityFrameworkCore;
using SistemaGYM.Datos; 
using SistemaGYM.Entidades;

namespace SistemaGYM.Repositorios;

public interface IActividadRepository
{
    Task<IEnumerable<Actividad>> ObtenerTodosAsync();
    Task<Actividad?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteNombreAsync(string nombre, int? excluirActividadId = null);
    Task AgregarAsync(Actividad entidad);
    Task ActualizarAsync(Actividad entidad);
    Task EliminarAsync(Actividad entidad);
}

public class ActividadRepository : IActividadRepository
{
    private readonly GimnasioContext _db;

    public ActividadRepository(GimnasioContext db)
    {
        _db = db;
    }

    // Se incluyen las inscripciones para poder contar cuántos alumnos activos tiene cada actividad
    public async Task<IEnumerable<Actividad>> ObtenerTodosAsync() =>
        await _db.Actividades.Include(a => a.ActividadesAlumno).ToListAsync();

    public async Task<Actividad?> ObtenerPorIdAsync(int id) =>
        await _db.Actividades.Include(a => a.ActividadesAlumno).FirstOrDefaultAsync(a => a.ActividadId == id);

    // No puede haber dos actividades con el mismo nombre (sin importar mayúsculas)
    public Task<bool> ExisteNombreAsync(string nombre, int? excluirActividadId = null) =>
        _db.Actividades.AnyAsync(a =>
            (!excluirActividadId.HasValue || a.ActividadId != excluirActividadId.Value) &&
            a.Nombre.ToLower() == nombre.Trim().ToLower());

    public async Task AgregarAsync(Actividad entidad) { await _db.Actividades.AddAsync(entidad); await _db.SaveChangesAsync(); }
    public async Task ActualizarAsync(Actividad entidad) { _db.Actividades.Update(entidad); await _db.SaveChangesAsync(); }
    public async Task EliminarAsync(Actividad entidad) { _db.Actividades.Remove(entidad); await _db.SaveChangesAsync(); }
}
