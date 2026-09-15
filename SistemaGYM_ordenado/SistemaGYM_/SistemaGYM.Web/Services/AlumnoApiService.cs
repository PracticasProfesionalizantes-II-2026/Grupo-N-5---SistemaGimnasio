using System.Net.Http.Json;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Models;

namespace SistemaGYM.Web.Services;

public class AlumnoApiService : IAlumnoApiService
{
    private readonly HttpClient _http;

    public AlumnoApiService(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("GymApi");
    }

    public async Task<List<AlumnoDto>> ObtenerTodosAsync()
    {
        var response = await _http.GetAsync("alumnos");
        if (!response.IsSuccessStatusCode) return new List<AlumnoDto>();

        var resultado = await response.Content.ReadFromJsonAsync<ApiResponse<List<AlumnoDto>>>();
        return resultado?.Data ?? new List<AlumnoDto>();
    }

    public async Task<AlumnoDetalleDto?> ObtenerDetalleAsync(int id)
    {
        var response = await _http.GetAsync($"alumnos/{id}/detalle");
        if (!response.IsSuccessStatusCode) return null;

        var resultado = await response.Content.ReadFromJsonAsync<ApiResponse<AlumnoDetalleDto>>();
        return resultado?.Data;
    }

    public async Task<bool> CrearAsync(AlumnoCreateDto dto)
    {
        var response = await _http.PostAsJsonAsync("alumnos", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ActualizarAsync(int id, AlumnoCreateDto dto)
    {
        var response = await _http.PutAsJsonAsync($"alumnos/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var response = await _http.DeleteAsync($"alumnos/{id}");
        return response.IsSuccessStatusCode;
    }
}
