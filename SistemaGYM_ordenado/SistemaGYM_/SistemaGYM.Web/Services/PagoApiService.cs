using System.Net.Http.Json;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Models;

namespace SistemaGYM.Web.Services;

public interface IPagoApiService
{
    Task<List<PagoDto>> ObtenerTodosAsync();
    Task<(bool ok, string? error)> CrearAsync(PagoCreateDto dto);
    Task<bool> EliminarAsync(int id);
}

public class PagoApiService : IPagoApiService
{
    private readonly HttpClient _http;

    public PagoApiService(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("GymApi");
    }

    public async Task<List<PagoDto>> ObtenerTodosAsync()
    {
        var response = await _http.GetAsync("pagos");
        if (!response.IsSuccessStatusCode) return new List<PagoDto>();
        var resultado = await response.Content.ReadFromJsonAsync<ApiResponse<List<PagoDto>>>(JsonOptionsWeb.Default);
        return resultado?.Data ?? new List<PagoDto>();
    }

    public async Task<(bool ok, string? error)> CrearAsync(PagoCreateDto dto)
    {
        var response = await _http.PostAsJsonAsync("pagos", dto);
        if (response.IsSuccessStatusCode) return (true, null);
        var resultado = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        return (false, resultado?.Message ?? "No se pudo registrar el pago");
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var response = await _http.DeleteAsync($"pagos/{id}");
        return response.IsSuccessStatusCode;
    }
}
