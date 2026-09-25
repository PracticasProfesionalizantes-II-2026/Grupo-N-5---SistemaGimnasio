using System.Net.Http.Json;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Models;

namespace SistemaGYM.Web.Services;

public interface IProfesorApiService
{
    Task<List<ProfesorDto>> ObtenerTodosAsync();
    Task<ProfesorDetalleDto?> ObtenerDetalleAsync(int id);
    Task<(bool ok, string? error)> CrearAsync(ProfesorCreateDto dto);
    Task<(bool ok, string? error)> ActualizarAsync(int id, ProfesorCreateDto dto);
    Task<bool> EliminarAsync(int id);
}

public class ProfesorApiService : IProfesorApiService
{
    private readonly HttpClient _http;

    public ProfesorApiService(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("GymApi");
    }

    public async Task<List<ProfesorDto>> ObtenerTodosAsync()
    {
        var response = await _http.GetAsync("profesores");
        if (!response.IsSuccessStatusCode) return new List<ProfesorDto>();
        var resultado = await response.Content.ReadFromJsonAsync<ApiResponse<List<ProfesorDto>>>();
        return resultado?.Data ?? new List<ProfesorDto>();
    }

    public async Task<ProfesorDetalleDto?> ObtenerDetalleAsync(int id)
    {
        var response = await _http.GetAsync($"profesores/{id}/detalle");
        if (!response.IsSuccessStatusCode) return null;
        var resultado = await response.Content.ReadFromJsonAsync<ApiResponse<ProfesorDetalleDto>>();
        return resultado?.Data;
    }

    public async Task<(bool ok, string? error)> CrearAsync(ProfesorCreateDto dto)
    {
        var response = await _http.PostAsJsonAsync("profesores", dto);
        if (response.IsSuccessStatusCode) return (true, null);
        return (false, await ApiError.LeerMensajeAsync(response, "No se pudo registrar el profesor"));
    }

    public async Task<(bool ok, string? error)> ActualizarAsync(int id, ProfesorCreateDto dto)
    {
        var response = await _http.PutAsJsonAsync($"profesores/{id}", dto);
        if (response.IsSuccessStatusCode) return (true, null);
        return (false, await ApiError.LeerMensajeAsync(response, "No se pudo modificar el profesor."));
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var response = await _http.DeleteAsync($"profesores/{id}");
        return response.IsSuccessStatusCode;
    }
}
