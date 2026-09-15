using System.Net.Http.Json;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Models;

namespace SistemaGYM.Web.Services;

public interface IAlimentacionApiService
{
    Task<List<AlimentacionDto>> ObtenerTodasAsync();
    Task<AlimentacionDto?> ObtenerPorIdAsync(int id);
    Task<(bool ok, string? error)> CrearAsync(AlimentacionCreateDto dto);
    Task<bool> ActualizarAsync(int id, AlimentacionCreateDto dto);
    Task<bool> EliminarAsync(int id);
}

public class AlimentacionApiService : IAlimentacionApiService
{
    private readonly HttpClient _http;

    public AlimentacionApiService(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("GymApi");
    }

    public async Task<List<AlimentacionDto>> ObtenerTodasAsync()
    {
        var response = await _http.GetAsync("alimentacion");
        if (!response.IsSuccessStatusCode) return new List<AlimentacionDto>();
        var resultado = await response.Content.ReadFromJsonAsync<ApiResponse<List<AlimentacionDto>>>();
        return resultado?.Data ?? new List<AlimentacionDto>();
    }

    public async Task<AlimentacionDto?> ObtenerPorIdAsync(int id)
    {
        var response = await _http.GetAsync($"alimentacion/{id}");
        if (!response.IsSuccessStatusCode) return null;
        var resultado = await response.Content.ReadFromJsonAsync<ApiResponse<AlimentacionDto>>();
        return resultado?.Data;
    }

    public async Task<(bool ok, string? error)> CrearAsync(AlimentacionCreateDto dto)
    {
        var response = await _http.PostAsJsonAsync("alimentacion", dto);
        if (response.IsSuccessStatusCode) return (true, null);
        var resultado = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        return (false, resultado?.Message ?? "No se pudo registrar el plan");
    }

    public async Task<bool> ActualizarAsync(int id, AlimentacionCreateDto dto)
    {
        var response = await _http.PutAsJsonAsync($"alimentacion/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var response = await _http.DeleteAsync($"alimentacion/{id}");
        return response.IsSuccessStatusCode;
    }
}
