using Microsoft.EntityFrameworkCore;
using SistemaGYM.Datos; 
using SistemaGYM.Entidades;

namespace SistemaGYM.Repositorios;

public interface IAlimentacionRepository
{
    Task<IEnumerable<Alimentacion>> ObtenerTodosAsync();
    Task<Alimentacion?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(Alimentacion entidad);
    Task ActualizarAsync(Alimentacion entidad);
    Task EliminarAsync(Alimentacion entidad);
}

public class AlimentacionRepository : IAlimentacionRepository
{
    private readonly GimnasioContext _db;

    public AlimentacionRepository(GimnasioContext db)
    {
        _db = db;
    }

    // Se incluyen los alumnos asignados (tabla AlimentacionAlumnos)
    public async Task<IEnumerable<Alimentacion>> ObtenerTodosAsync() =>
        await _db.Alimentaciones
            .Include(a => a.AlimentacionAlumnos).ThenInclude(aa => aa.Alumno)
            .ToListAsync();
    public async Task<Alimentacion?> ObtenerPorIdAsync(int id) =>
        await _db.Alimentaciones
            .Include(a => a.AlimentacionAlumnos).ThenInclude(aa => aa.Alumno)
            .FirstOrDefaultAsync(a => a.Id == id);
    public async Task AgregarAsync(Alimentacion entidad) { await _db.Alimentaciones.AddAsync(entidad); await _db.SaveChangesAsync(); }
    public async Task ActualizarAsync(Alimentacion entidad) { _db.Alimentaciones.Update(entidad); await _db.SaveChangesAsync(); }
    public async Task EliminarAsync(Alimentacion entidad) { _db.Alimentaciones.Remove(entidad); await _db.SaveChangesAsync(); }
}
