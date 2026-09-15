using System.Net.Http.Json;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Models;

namespace SistemaGYM.Web.Services;

public interface IAnuncioApiService
{
    Task<List<AnuncioDto>> ObtenerTodosAsync();
    Task<(bool ok, string? error)> CrearAsync(AnuncioCreateDto dto);
    Task<bool> ActualizarAsync(int id, AnuncioCreateDto dto);
    Task<bool> EliminarAsync(int id);
}

public class AnuncioApiService : IAnuncioApiService
{
    private readonly HttpClient _http;

    public AnuncioApiService(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("GymApi");
    }

    public async Task<List<AnuncioDto>> ObtenerTodosAsync()
    {
        var response = await _http.GetAsync("anuncios");
        if (!response.IsSuccessStatusCode) return new List<AnuncioDto>();
        var resultado = await response.Content.ReadFromJsonAsync<ApiResponse<List<AnuncioDto>>>();
        return resultado?.Data ?? new List<AnuncioDto>();
    }

    public async Task<(bool ok, string? error)> CrearAsync(AnuncioCreateDto dto)
    {
        var response = await _http.PostAsJsonAsync("anuncios", dto);
        if (response.IsSuccessStatusCode) return (true, null);
        var resultado = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        return (false, resultado?.Message ?? "No se pudo registrar el anuncio");
    }

    public async Task<bool> ActualizarAsync(int id, AnuncioCreateDto dto)
    {
        var response = await _http.PutAsJsonAsync($"anuncios/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var response = await _http.DeleteAsync($"anuncios/{id}");
        return response.IsSuccessStatusCode;
    }
}
