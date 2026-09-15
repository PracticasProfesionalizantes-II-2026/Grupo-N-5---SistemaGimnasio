using SistemaGYM.Logica.DTOs;

namespace SistemaGYM.Web.Services;

public interface IAlumnoApiService
{
    Task<List<AlumnoDto>> ObtenerTodosAsync();
    Task<AlumnoDetalleDto?> ObtenerDetalleAsync(int id);
    Task<bool> CrearAsync(AlumnoCreateDto dto);
    Task<bool> ActualizarAsync(int id, AlumnoCreateDto dto);
    Task<bool> EliminarAsync(int id);
}
