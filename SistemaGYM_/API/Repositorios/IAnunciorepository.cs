using Microsoft.EntityFrameworkCore;
using SistemaGYM.Datos; 
using SistemaGYM.Entidades;

namespace SistemaGYM.Repositorios;

public interface IAnuncioRepository
{
    Task<IEnumerable<Anuncio>> ObtenerTodosAsync();
    Task<Anuncio?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(Anuncio entidad);
    Task ActualizarAsync(Anuncio entidad);
    Task EliminarAsync(Anuncio entidad);
}

public class AnuncioRepository : IAnuncioRepository
{
    private readonly GimnasioContext _db;

    public AnuncioRepository(GimnasioContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Anuncio>> ObtenerTodosAsync() => await _db.Anuncios.ToListAsync();
    public async Task<Anuncio?> ObtenerPorIdAsync(int id) => await _db.Anuncios.FindAsync(id);
    public async Task AgregarAsync(Anuncio entidad) { await _db.Anuncios.AddAsync(entidad); await _db.SaveChangesAsync(); }
    public async Task ActualizarAsync(Anuncio entidad) { _db.Anuncios.Update(entidad); await _db.SaveChangesAsync(); }
    public async Task EliminarAsync(Anuncio entidad) { _db.Anuncios.Remove(entidad); await _db.SaveChangesAsync(); }
}