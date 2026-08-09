using Microsoft.EntityFrameworkCore;
using SistemaGYM.Datos; 
using SistemaGYM.Entidades;

namespace SistemaGYM.Repositorios;

public interface IPagoRepository
{
    Task<IEnumerable<Pago>> ObtenerTodosAsync();
    Task<Pago?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(Pago entidad);
    Task ActualizarAsync(Pago entidad);
    Task EliminarAsync(Pago entidad);
}

public class PagoRepository : IPagoRepository
{
    private readonly GimnasioContext _db;

    public PagoRepository(GimnasioContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Pago>> ObtenerTodosAsync() => await _db.Pagos.ToListAsync();
    public async Task<Pago?> ObtenerPorIdAsync(int id) => await _db.Pagos.FindAsync(id);
    public async Task AgregarAsync(Pago entidad) { await _db.Pagos.AddAsync(entidad); await _db.SaveChangesAsync(); }
    public async Task ActualizarAsync(Pago entidad) { _db.Pagos.Update(entidad); await _db.SaveChangesAsync(); }
    public async Task EliminarAsync(Pago entidad) { _db.Pagos.Remove(entidad); await _db.SaveChangesAsync(); }
}