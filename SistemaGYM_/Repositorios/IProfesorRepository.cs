using Microsoft.EntityFrameworkCore;
using SistemaGYM.Datos;
using SistemaGYM.Entidades;

namespace SistemaGYM.Repositorios;

public interface IProfesorRepository
{
    Task<IEnumerable<Profesor>> ObtenerTodosAsync();
    Task<Profesor?> ObtenerPorIdAsync(int id);
    Task<Profesor?> ObtenerDetallePorIdAsync(int id);
    Task AgregarAsync(Profesor profesor);
    Task ActualizarAsync(Profesor profesor);
    Task EliminarAsync(Profesor profesor);
}

public class ProfesorRepository : IProfesorRepository
{
    private readonly GimnasioContext _db;

    public ProfesorRepository(GimnasioContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Profesor>> ObtenerTodosAsync() =>
        await _db.Profesores.ToListAsync();

    public async Task<Profesor?> ObtenerPorIdAsync(int id) =>
        await _db.Profesores.FindAsync(id);

    // Trae las actividades del profesor para el endpoint /detalle
    public async Task<Profesor?> ObtenerDetallePorIdAsync(int id) =>
        await _db.Profesores
            .Include(p => p.Actividades)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task AgregarAsync(Profesor profesor)
    {
        await _db.Profesores.AddAsync(profesor);
        await _db.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Profesor profesor)
    {
        _db.Profesores.Update(profesor);
        await _db.SaveChangesAsync();
    }

    public async Task EliminarAsync(Profesor profesor)
    {
        _db.Profesores.Remove(profesor);
        await _db.SaveChangesAsync();
    }
}
