using System.Net.Http.Json;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Models;

namespace SistemaGYM.Web.Services;

public interface ISuscripcionApiService
{
    Task<List<SuscripcionDto>> ObtenerTodasAsync();
    Task<SuscripcionDto?> ObtenerPorIdAsync(int id);
    Task<(bool ok, string? error)> CrearAsync(SuscripcionCreateDto dto);
    Task<bool> ActualizarAsync(int id, SuscripcionCreateDto dto);
    Task<bool> EliminarAsync(int id);
}

public class SuscripcionApiService : ISuscripcionApiService
{
    private readonly HttpClient _http;

    public SuscripcionApiService(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("GymApi");
    }

    public async Task<List<SuscripcionDto>> ObtenerTodasAsync()
    {
        var response = await _http.GetAsync("suscripciones");
        if (!response.IsSuccessStatusCode) return new List<SuscripcionDto>();
        var resultado = await response.Content.ReadFromJsonAsync<ApiResponse<List<SuscripcionDto>>>();
        return resultado?.Data ?? new List<SuscripcionDto>();
    }

    public async Task<SuscripcionDto?> ObtenerPorIdAsync(int id)
    {
        var response = await _http.GetAsync($"suscripciones/{id}");
        if (!response.IsSuccessStatusCode) return null;
        var resultado = await response.Content.ReadFromJsonAsync<ApiResponse<SuscripcionDto>>();
        return resultado?.Data;
    }

    public async Task<(bool ok, string? error)> CrearAsync(SuscripcionCreateDto dto)
    {
        var response = await _http.PostAsJsonAsync("suscripciones", dto);
        if (response.IsSuccessStatusCode) return (true, null);
        return (false, await ApiError.LeerMensajeAsync(response, "No se pudo registrar la suscripción"));
    }

    public async Task<bool> ActualizarAsync(int id, SuscripcionCreateDto dto)
    {
        var response = await _http.PutAsJsonAsync($"suscripciones/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var response = await _http.DeleteAsync($"suscripciones/{id}");
        return response.IsSuccessStatusCode;
    }
}
