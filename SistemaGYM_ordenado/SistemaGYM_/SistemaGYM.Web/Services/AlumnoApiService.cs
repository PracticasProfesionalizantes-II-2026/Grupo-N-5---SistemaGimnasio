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

    // Devuelve también el alumno creado, así se le puede asignar la suscripción con su Id
    public async Task<(bool ok, string? error, AlumnoDto? creado)> CrearAsync(AlumnoCreateDto dto)
    {
        var response = await _http.PostAsJsonAsync("alumnos", dto);
        if (!response.IsSuccessStatusCode)
            return (false, await ApiError.LeerMensajeAsync(response, "No se pudo registrar el cliente."), null);

        var resultado = await response.Content.ReadFromJsonAsync<ApiResponse<AlumnoDto>>();
        return (true, null, resultado?.Data);
    }

    public async Task<(bool ok, string? error)> ActualizarAsync(int id, AlumnoCreateDto dto)
    {
        var response = await _http.PutAsJsonAsync($"alumnos/{id}", dto);
        if (response.IsSuccessStatusCode) return (true, null);
        return (false, await ApiError.LeerMensajeAsync(response, "No se pudo modificar el cliente."));
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var response = await _http.DeleteAsync($"alumnos/{id}");
        return response.IsSuccessStatusCode;
    }
}
