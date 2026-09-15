using Microsoft.EntityFrameworkCore;
using SistemaGYM.Datos;
using SistemaGYM.Entidades;

namespace SistemaGYM.Repositorios;

public interface IAdministradorRepository
{
    Task<Administrador?> ObtenerPorUsuarioAsync(string usuario);
}

public class AdministradorRepository : IAdministradorRepository
{
    private readonly GimnasioContext _db;

    public AdministradorRepository(GimnasioContext db)
    {
        _db = db;
    }

    public async Task<Administrador?> ObtenerPorUsuarioAsync(string usuario) =>
        await _db.Administradores.FirstOrDefaultAsync(a => a.Usuario == usuario);
}
