namespace SistemaGYM.Web.Models;

// Tu API siempre responde con esta forma: { "status": 200, "message": "...", "data": ... }
// Esta clase permite deserializar esa respuesta sin importar qué tipo tenga "data".
public class ApiResponse<T>
{
    public int Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}
