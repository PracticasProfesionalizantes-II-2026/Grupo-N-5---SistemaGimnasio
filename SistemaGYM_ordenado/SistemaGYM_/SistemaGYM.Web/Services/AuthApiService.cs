using System.Net.Http.Json;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Models;

namespace SistemaGYM.Web.Services;

public interface IAuthApiService
{
    Task<LoginResultDto?> LoginAsync(string email, string contrasenia);
    Task<(bool ok, string? error)> ResetearContraseniaAsync(string email, int dni, string nuevaContrasenia);
}

public class AuthApiService : IAuthApiService
{
    private readonly HttpClient _http;

    public AuthApiService(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("GymApi");
    }

    public async Task<LoginResultDto?> LoginAsync(string email, string contrasenia)
    {
        var response = await _http.PostAsJsonAsync("auth/login", new LoginDto(email, contrasenia));
        if (!response.IsSuccessStatusCode) return null;

        var resultado = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResultDto>>();
        return resultado?.Data;
    }

    public async Task<(bool ok, string? error)> ResetearContraseniaAsync(string email, int dni, string nuevaContrasenia)
    {
        var response = await _http.PostAsJsonAsync("auth/resetear-contrasenia", new ResetPasswordDto(email, dni, nuevaContrasenia));
        if (response.IsSuccessStatusCode) return (true, null);

        return (false, await ApiError.LeerMensajeAsync(response, "No se pudo restablecer la contraseña"));
    }
}
