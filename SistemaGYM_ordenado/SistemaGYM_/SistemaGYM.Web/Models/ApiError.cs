using System.Text.Json;

namespace SistemaGYM.Web.Models;

// Lee el mensaje de error que devuelve la API para mostrárselo al usuario.
// La API puede responder de dos formas cuando algo sale mal:
//   - Errores de negocio:      { "status": 400, "message": "Ya existe un cliente con ese DNI." }
//   - Validaciones de los DTO: { "errors": { "Dni": ["El DNI debe tener 7 u 8 dígitos"] } }
public static class ApiError
{
    public static async Task<string> LeerMensajeAsync(HttpResponseMessage response, string mensajePorDefecto)
    {
        try
        {
            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var raiz = json.RootElement;

            if (raiz.TryGetProperty("message", out var message) && message.ValueKind == JsonValueKind.String)
                return message.GetString()!;

            if (raiz.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
            {
                var mensajes = errors.EnumerateObject()
                    .Where(campo => campo.Value.ValueKind == JsonValueKind.Array)
                    .SelectMany(campo => campo.Value.EnumerateArray())
                    .Select(m => m.GetString())
                    .Where(m => !string.IsNullOrWhiteSpace(m));

                var texto = string.Join(" ", mensajes);
                if (texto.Length > 0) return texto;
            }
        }
        catch (JsonException)
        {
            // La respuesta no era JSON: usamos el mensaje por defecto
        }

        return mensajePorDefecto;
    }
}
