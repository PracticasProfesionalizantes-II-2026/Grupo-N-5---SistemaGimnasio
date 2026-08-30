using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
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

    [HttpGet]
    public async Task<List<AlumnoDto>> ObtenerTodosAsync()
    {
        // GET /api/alumnos
        var response = await _http.GetAsync("alumnos");
        if (!response.IsSuccessStatusCode) return new List<AlumnoDto>();

        var resultado = await response.Content.ReadFromJsonAsync<ApiResponse<List<AlumnoDto>>>();
        return resultado?.Data ?? new List<AlumnoDto>();
    }
    [HttpGet("{id}/detalle")]
    public async Task<AlumnoDetalleDto?> ObtenerDetalleAsync(int id)
    {
        // GET /api/alumnos/{id}/detalle
        var response = await _http.GetAsync($"alumnos/{id}/detalle");
        if (!response.IsSuccessStatusCode) return null;

        var resultado = await response.Content.ReadFromJsonAsync<ApiResponse<AlumnoDetalleDto>>();
        return resultado?.Data;
    }

    [HttpPost]
    public async Task<bool> CrearAsync(AlumnoCreateDto dto)
    {
        // POST /api/alumnos
        var response = await _http.PostAsJsonAsync("alumnos", dto);
        return response.IsSuccessStatusCode;
    }
}
