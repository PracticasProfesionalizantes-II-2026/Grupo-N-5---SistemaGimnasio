using Microsoft.EntityFrameworkCore;
using SistemaGYM.Datos; 
using SistemaGYM.Entidades;

namespace SistemaGYM.Repositorios;

public interface ISuscripcionRepository
{
    Task<IEnumerable<Suscripcion>> ObtenerTodosAsync();
    Task<Suscripcion?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(Suscripcion entidad);
    Task ActualizarAsync(Suscripcion entidad);
    Task EliminarAsync(Suscripcion entidad);
}

public class SuscripcionRepository : ISuscripcionRepository
{
    private readonly GimnasioContext _db;

    public SuscripcionRepository(GimnasioContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Suscripcion>> ObtenerTodosAsync() => await _db.Suscripciones.ToListAsync();
    public async Task<Suscripcion?> ObtenerPorIdAsync(int id) => await _db.Suscripciones.FindAsync(id);
    public async Task AgregarAsync(Suscripcion entidad) { await _db.Suscripciones.AddAsync(entidad); await _db.SaveChangesAsync(); }
    public async Task ActualizarAsync(Suscripcion entidad) { _db.Suscripciones.Update(entidad); await _db.SaveChangesAsync(); }
    public async Task EliminarAsync(Suscripcion entidad) { _db.Suscripciones.Remove(entidad); await _db.SaveChangesAsync(); }
}