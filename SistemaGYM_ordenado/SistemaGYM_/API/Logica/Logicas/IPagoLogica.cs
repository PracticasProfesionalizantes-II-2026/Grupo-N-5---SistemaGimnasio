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
    private readonly IAlumnoRepository _alumnoRepo;
    private readonly IAlumnoSuscripcionRepository _alumnoSuscripcionRepo;

    public PagoLogica(
        IPagoRepository repo,
        IAlumnoRepository alumnoRepo,
        IAlumnoSuscripcionRepository alumnoSuscripcionRepo)
    {
        _repo = repo;
        _alumnoRepo = alumnoRepo;
        _alumnoSuscripcionRepo = alumnoSuscripcionRepo;
    }

    public async Task<IEnumerable<PagoDto>> ObtenerTodosAsync()
    {
        var lista = await _repo.ObtenerTodosAsync();
        return lista.Select(p => new PagoDto(
            p.Id, 
            p.Monto, 
            p.FechaPago, 
            p.MetodoPago, 
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
            p.FechaPago, 
            p.MetodoPago, 
            p.AlumnoId, 
            p.AlumnoSuscripcionId
        );
    }

    public async Task<PagoDto> CrearAsync(PagoCreateDto dto)
    {
        await ValidarReferenciasAsync(dto);
        var nuevo = new Pago
        {
            Monto = dto.Monto,
            FechaPago = dto.FechaPago,
            MetodoPago = dto.MetodoPago, 
            AlumnoId = dto.AlumnoId,
            AlumnoSuscripcionId = dto.AlumnoSuscripcionId
        };
        
        await _repo.AgregarAsync(nuevo);
        
        return new PagoDto(
            nuevo.Id, 
            nuevo.Monto, 
            nuevo.FechaPago, 
            nuevo.MetodoPago, 
            nuevo.AlumnoId, 
            nuevo.AlumnoSuscripcionId
        );
    }

    public async Task<bool> ActualizarAsync(int id, PagoCreateDto dto)
    {
        var p = await _repo.ObtenerPorIdAsync(id);
        if (p == null) return false;

        await ValidarReferenciasAsync(dto);

        p.Monto = dto.Monto;
        p.FechaPago = dto.FechaPago;
        p.MetodoPago = dto.MetodoPago;
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

    private async Task ValidarReferenciasAsync(PagoCreateDto dto)
    {
        if (dto.FechaPago == default || dto.FechaPago.Date > DateTime.UtcNow.Date)
            throw new ReglaDeNegocioException("La fecha del pago debe ser una fecha válida que no esté en el futuro.");

        var alumno = await _alumnoRepo.ObtenerPorIdAsync(dto.AlumnoId);
        if (alumno is null || !alumno.EstaActivo)
            throw new ReglaDeNegocioException("El alumno indicado no existe o se encuentra inactivo.");

        var suscripcion = await _alumnoSuscripcionRepo.ObtenerPorIdAsync(dto.AlumnoSuscripcionId);
        if (suscripcion is null || suscripcion.AlumnoId != dto.AlumnoId)
            throw new ReglaDeNegocioException("La suscripción indicada no pertenece al alumno seleccionado.");

        if (!suscripcion.Activa || (suscripcion.FechaFin.HasValue && suscripcion.FechaFin.Value.Date < DateTime.UtcNow.Date))
            throw new ReglaDeNegocioException("Solo se pueden registrar pagos para una suscripción activa y vigente.");
    }
}
