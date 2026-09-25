using System.Net.Http.Json;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Models;

namespace SistemaGYM.Web.Services;

public interface IRutinaApiService
{
    Task<List<RutinaDto>> ObtenerTodasAsync();
    Task<(bool ok, string? error)> CrearAsync(RutinaCreateDto dto);
    Task<(bool ok, string? error)> ActualizarAsync(int id, RutinaCreateDto dto);
    Task<bool> EliminarAsync(int id);
    Task<List<RutinaDto>> ObtenerDeAlumnoAsync(int alumnoId);
    Task<List<RutinaDto>> ObtenerDeProfesorAsync(int profesorId);
}

public class RutinaApiService : IRutinaApiService
{
    private readonly HttpClient _http;

    public RutinaApiService(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("GymApi");
    }

    public async Task<List<RutinaDto>> ObtenerTodasAsync()
    {
        var response = await _http.GetAsync("rutinas");
        if (!response.IsSuccessStatusCode) return new List<RutinaDto>();
        var resultado = await response.Content.ReadFromJsonAsync<ApiResponse<List<RutinaDto>>>();
        return resultado?.Data ?? new List<RutinaDto>();
    }

    public async Task<(bool ok, string? error)> CrearAsync(RutinaCreateDto dto)
    {
        var response = await _http.PostAsJsonAsync("rutinas", dto);
        if (response.IsSuccessStatusCode) return (true, null);
        return (false, await ApiError.LeerMensajeAsync(response, "No se pudo registrar la rutina"));
    }

    public async Task<(bool ok, string? error)> ActualizarAsync(int id, RutinaCreateDto dto)
    {
        var response = await _http.PutAsJsonAsync($"rutinas/{id}", dto);
        if (response.IsSuccessStatusCode) return (true, null);
        return (false, await ApiError.LeerMensajeAsync(response, "No se pudo modificar la rutina."));
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var response = await _http.DeleteAsync($"rutinas/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<RutinaDto>> ObtenerDeAlumnoAsync(int alumnoId)
    {
        var todas = await ObtenerTodasAsync();
        return todas.Where(r => r.AlumnoId == alumnoId).ToList();
    }

    public async Task<List<RutinaDto>> ObtenerDeProfesorAsync(int profesorId)
    {
        var todas = await ObtenerTodasAsync();
        return todas.Where(r => r.ProfesorId == profesorId).ToList();
    }
}
