using SistemaGYM.Logica.DTOs;
using SistemaGYM.Entidades;
using SistemaGYM.Repositorios;

namespace SistemaGYM.Logica;


public interface IAlumnoLogica
{
    Task<IEnumerable<AlumnoDto>> ObtenerTodosAsync();
    Task<AlumnoDto?> ObtenerPorIdAsync(int id);
    Task<AlumnoDetalleDto?> ObtenerDetallePorIdAsync(int id);
    Task<AlumnoDto> CrearAsync(AlumnoCreateDto dto);
    Task<bool> ActualizarAsync(int id, AlumnoCreateDto dto);
    Task<bool> EliminarAsync(int id);
}
