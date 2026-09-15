using Microsoft.EntityFrameworkCore;
using SistemaGYM.Datos; 
using SistemaGYM.Entidades;

namespace SistemaGYM.Repositorios;

public interface IRutinaRepository
{
    Task<IEnumerable<Rutina>> ObtenerTodosAsync();
    Task<Rutina?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(Rutina entidad);
    Task ActualizarAsync(Rutina entidad);
    Task EliminarAsync(Rutina entidad);
}

public class RutinaRepository : IRutinaRepository
{
    private readonly GimnasioContext _db;

    public RutinaRepository(GimnasioContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Rutina>> ObtenerTodosAsync() => await _db.Rutinas.ToListAsync();
    public async Task<Rutina?> ObtenerPorIdAsync(int id) => await _db.Rutinas.FindAsync(id);
    public async Task AgregarAsync(Rutina entidad) { await _db.Rutinas.AddAsync(entidad); await _db.SaveChangesAsync(); }
    public async Task ActualizarAsync(Rutina entidad) { _db.Rutinas.Update(entidad); await _db.SaveChangesAsync(); }
    public async Task EliminarAsync(Rutina entidad) { _db.Rutinas.Remove(entidad); await _db.SaveChangesAsync(); }
}