using Microsoft.EntityFrameworkCore;
using SistemaGYM.Datos; 
using SistemaGYM.Entidades;

namespace SistemaGYM.Repositorios;

public interface IActividadRepository
{
    Task<IEnumerable<Actividad>> ObtenerTodosAsync();
    Task<Actividad?> ObtenerPorIdAsync(int id);
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

    public async Task<IEnumerable<Actividad>> ObtenerTodosAsync() => await _db.Actividades.ToListAsync();
    public async Task<Actividad?> ObtenerPorIdAsync(int id) => await _db.Actividades.FindAsync(id);
    public async Task AgregarAsync(Actividad entidad) { await _db.Actividades.AddAsync(entidad); await _db.SaveChangesAsync(); }
    public async Task ActualizarAsync(Actividad entidad) { _db.Actividades.Update(entidad); await _db.SaveChangesAsync(); }
    public async Task EliminarAsync(Actividad entidad) { _db.Actividades.Remove(entidad); await _db.SaveChangesAsync(); }
}