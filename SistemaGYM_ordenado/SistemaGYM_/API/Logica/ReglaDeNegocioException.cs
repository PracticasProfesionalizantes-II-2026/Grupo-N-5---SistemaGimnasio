namespace SistemaGYM.Logica;

/// <summary>Indica que una solicitud es válida en formato pero viola una regla del dominio.</summary>
public sealed class ReglaDeNegocioException : Exception
{
    public ReglaDeNegocioException(string message) : base(message) { }
}
