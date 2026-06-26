using SistemaGYM.Logica.DTOs;
using SistemaGYM.Entidades;
using SistemaGYM.Repositorios;

namespace SistemaGYM.Logica;

public interface IPagoLogica
{
    Task<IEnumerable<PagoDto>> ObtenerTodosAsync();
    Task<PagoDto?> ObtenerPorIdAsync(int id);
    Task<PagoDto> CrearAsync(PagoCreateDto dto);
    Task<bool> ActualizarAsync(int id, PagoCreateDto dto);
    Task<bool> EliminarAsync(int id);
}

public class PagoLogica : IPagoLogica
{
    private readonly IPagoRepository _repo;

    public PagoLogica(IPagoRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<PagoDto>> ObtenerTodosAsync()
    {
        var lista = await _repo.ObtenerTodosAsync();
        return lista.Select(p => new PagoDto(
            p.Id, 
            p.Monto, 
            p.FechaPago.ToString("yyyy-MM-dd"), 
            p.MetodoPago.ToString(), 
            p.AlumnoId, 
            p.AlumnoSuscripcionId
        ));
    }

    public async Task<PagoDto?> ObtenerPorIdAsync(int id)
    {
        var p = await _repo.ObtenerPorIdAsync(id);
        if (p == null) return null;
        
        return new PagoDto(
            p.Id, 
            p.Monto, 
            p.FechaPago.ToString("yyyy-MM-dd"), 
            p.MetodoPago.ToString(), 
            p.AlumnoId, 
            p.AlumnoSuscripcionId
        );
    }

    public async Task<PagoDto> CrearAsync(PagoCreateDto dto)
    {
        var nuevo = new Pago
        {
            Monto = dto.Monto,
            FechaPago = DateTime.Parse(dto.FechaPago),
            MetodoPago = Enum.Parse<MetodoPago>(dto.MetodoPago, true), 
            AlumnoId = dto.AlumnoId,
            AlumnoSuscripcionId = dto.AlumnoSuscripcionId
        };
        
        await _repo.AgregarAsync(nuevo);
        
        return new PagoDto(
            nuevo.Id, 
            nuevo.Monto, 
            nuevo.FechaPago.ToString("yyyy-MM-dd"), 
            nuevo.MetodoPago.ToString(), 
            nuevo.AlumnoId, 
            nuevo.AlumnoSuscripcionId
        );
    }

    public async Task<bool> ActualizarAsync(int id, PagoCreateDto dto)
    {
        var p = await _repo.ObtenerPorIdAsync(id);
        if (p == null) return false;

        p.Monto = dto.Monto;
        p.FechaPago = DateTime.Parse(dto.FechaPago);
        p.MetodoPago = Enum.Parse<MetodoPago>(dto.MetodoPago, true);
        p.AlumnoId = dto.AlumnoId;
        p.AlumnoSuscripcionId = dto.AlumnoSuscripcionId;

        await _repo.ActualizarAsync(p);
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var p = await _repo.ObtenerPorIdAsync(id);
        if (p == null) return false;

        await _repo.EliminarAsync(p);
        return true;
    }
}