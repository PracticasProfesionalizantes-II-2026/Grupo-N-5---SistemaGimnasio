using System.Net.Http.Json;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Models;

namespace SistemaGYM.Web.Services;

public interface IAlumnoSuscripcionApiService
{
    Task<List<AlumnoSuscripcionDto>> ObtenerHistorialAsync(int alumnoId);
    Task<(bool ok, string? error)> AsignarAsync(int alumnoId, int suscripcionId);
    Task<bool> CancelarAsync(int alumnoId, int alumnoSuscripcionId);
}

public class AlumnoSuscripcionApiService : IAlumnoSuscripcionApiService
{
    private readonly HttpClient _http;

    public AlumnoSuscripcionApiService(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("GymApi");
    }

    public async Task<List<AlumnoSuscripcionDto>> ObtenerHistorialAsync(int alumnoId)
    {
        var response = await _http.GetAsync($"alumnos/{alumnoId}/suscripciones");
        if (!response.IsSuccessStatusCode) return new List<AlumnoSuscripcionDto>();
        var resultado = await response.Content.ReadFromJsonAsync<ApiResponse<List<AlumnoSuscripcionDto>>>();
        return resultado?.Data ?? new List<AlumnoSuscripcionDto>();
    }

    public async Task<(bool ok, string? error)> AsignarAsync(int alumnoId, int suscripcionId)
    {
        var response = await _http.PostAsJsonAsync($"alumnos/{alumnoId}/suscripciones", new AsignarSuscripcionDto(suscripcionId));
        if (response.IsSuccessStatusCode) return (true, null);
        return (false, await ApiError.LeerMensajeAsync(response, "No se pudo asignar la suscripción"));
    }

    public async Task<bool> CancelarAsync(int alumnoId, int alumnoSuscripcionId)
    {
        var response = await _http.DeleteAsync($"alumnos/{alumnoId}/suscripciones/{alumnoSuscripcionId}");
        return response.IsSuccessStatusCode;
    }
}
