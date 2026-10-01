using System.Net.Http.Json;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Models;

namespace SistemaGYM.Web.Services;

public interface IActividadApiService
{
    Task<List<ActividadDto>> ObtenerTodasAsync();
    Task<ActividadDto?> ObtenerPorIdAsync(int id);
    Task<(bool ok, string? error, ActividadDto? creada)> CrearAsync(ActividadCreateDto dto);
    Task<(bool ok, string? error)> ActualizarAsync(int id, ActividadCreateDto dto);
    Task<(bool ok, string? error)> EliminarAsync(int id);
    Task<List<AlumnoInscriptoDto>> ObtenerAlumnosInscriptosAsync(int actividadId);
    Task<(bool ok, string? error)> InscribirAlumnoAsync(int alumnoId, int actividadId);
    Task<(bool ok, string? error)> DarDeBajaAlumnoAsync(int alumnoId, int actividadId);

    // Inscribe a varios alumnos a la vez. Devuelve cuántos se inscribieron y los errores de los que no.
    Task<(int inscriptos, List<string> errores)> InscribirAlumnosAsync(int actividadId, List<int> alumnoIds);

    // No hay endpoint directo "actividades de un alumno": se deriva revisando
    // la lista de inscriptos de cada actividad. Aceptable para el volumen de este sistema.
    Task<List<ActividadDto>> ObtenerActividadesDeAlumnoAsync(int alumnoId);
}

public class ActividadApiService : IActividadApiService
{
    private readonly HttpClient _http;

    public ActividadApiService(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("GymApi");
    }

    public async Task<List<ActividadDto>> ObtenerTodasAsync()
    {
        var response = await _http.GetAsync("actividades");
        if (!response.IsSuccessStatusCode) return new List<ActividadDto>();
        var resultado = await response.Content.ReadFromJsonAsync<ApiResponse<List<ActividadDto>>>(JsonOptionsWeb.Default);
        return resultado?.Data ?? new List<ActividadDto>();
    }

    public async Task<ActividadDto?> ObtenerPorIdAsync(int id)
    {
        var response = await _http.GetAsync($"actividades/{id}");
        if (!response.IsSuccessStatusCode) return null;
        var resultado = await response.Content.ReadFromJsonAsync<ApiResponse<ActividadDto>>(JsonOptionsWeb.Default);
        return resultado?.Data;
    }

    // Devuelve también la actividad creada, así se le pueden inscribir alumnos con su Id
    public async Task<(bool ok, string? error, ActividadDto? creada)> CrearAsync(ActividadCreateDto dto)
    {
        var response = await _http.PostAsJsonAsync("actividades", dto);
        if (!response.IsSuccessStatusCode)
            return (false, await ApiError.LeerMensajeAsync(response, "No se pudo registrar la actividad"), null);

        var resultado = await response.Content.ReadFromJsonAsync<ApiResponse<ActividadDto>>(JsonOptionsWeb.Default);
        return (true, null, resultado?.Data);
    }

    public async Task<(bool ok, string? error)> ActualizarAsync(int id, ActividadCreateDto dto)
    {
        var response = await _http.PutAsJsonAsync($"actividades/{id}", dto);
        if (response.IsSuccessStatusCode) return (true, null);
        return (false, await ApiError.LeerMensajeAsync(response, "No se pudo modificar la actividad."));
    }

    public async Task<(bool ok, string? error)> EliminarAsync(int id)
    {
        var response = await _http.DeleteAsync($"actividades/{id}");
        if (response.IsSuccessStatusCode) return (true, null);
        return (false, await ApiError.LeerMensajeAsync(response, "No se pudo eliminar la actividad."));
    }

    public async Task<List<AlumnoInscriptoDto>> ObtenerAlumnosInscriptosAsync(int actividadId)
    {
        var response = await _http.GetAsync($"actividades/{actividadId}/alumnos");
        if (!response.IsSuccessStatusCode) return new List<AlumnoInscriptoDto>();
        var resultado = await response.Content.ReadFromJsonAsync<ApiResponse<List<AlumnoInscriptoDto>>>();
        return resultado?.Data ?? new List<AlumnoInscriptoDto>();
    }

    public async Task<(bool ok, string? error)> InscribirAlumnoAsync(int alumnoId, int actividadId)
    {
        var response = await _http.PostAsync($"alumnos/{alumnoId}/actividades/{actividadId}", null);
        if (response.IsSuccessStatusCode) return (true, null);
        return (false, await ApiError.LeerMensajeAsync(response, "No se pudo inscribir al alumno"));
    }

    public async Task<(bool ok, string? error)> DarDeBajaAlumnoAsync(int alumnoId, int actividadId)
    {
        var response = await _http.DeleteAsync($"alumnos/{alumnoId}/actividades/{actividadId}");
        if (response.IsSuccessStatusCode) return (true, null);
        return (false, await ApiError.LeerMensajeAsync(response, "No se pudo dar de baja al alumno de la actividad."));
    }

    public async Task<(int inscriptos, List<string> errores)> InscribirAlumnosAsync(int actividadId, List<int> alumnoIds)
    {
        var inscriptos = 0;
        var errores = new List<string>();

        foreach (var alumnoId in alumnoIds)
        {
            var (ok, error) = await InscribirAlumnoAsync(alumnoId, actividadId);
            if (ok) inscriptos++;
            else if (error != null && !errores.Contains(error)) errores.Add(error);
        }

        return (inscriptos, errores);
    }

    public async Task<List<ActividadDto>> ObtenerActividadesDeAlumnoAsync(int alumnoId)
    {
        var todas = await ObtenerTodasAsync();
        var resultado = new List<ActividadDto>();

        foreach (var actividad in todas)
        {
            var inscriptos = await ObtenerAlumnosInscriptosAsync(actividad.ActividadId);
            if (inscriptos.Any(i => i.AlumnoId == alumnoId && i.Activa))
                resultado.Add(actividad);
        }

        return resultado;
    }
}
