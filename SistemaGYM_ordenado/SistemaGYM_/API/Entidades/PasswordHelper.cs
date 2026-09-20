using System.Security.Cryptography;
using System.Text;

namespace SistemaGYM.Entidades;

public static class PasswordHelper
{
    private const int TamanioSalt = 16;   // bytes
    private const int TamanioHash = 32;   // bytes
    private const int Iteraciones = 100_000;

    public static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(TamanioSalt);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iteraciones,
            HashAlgorithmName.SHA256,
            TamanioHash);

        // Se guarda "salt.hash" en base64, todo en un único string
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public static bool VerificarPassword(string password, string hashGuardado)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashGuardado)) return false;

        try
        {
            var partes = hashGuardado.Split('.');
            if (partes.Length != 2) return false;

            var salt = Convert.FromBase64String(partes[0]);
            var hashEsperado = Convert.FromBase64String(partes[1]);
            if (salt.Length != TamanioSalt || hashEsperado.Length != TamanioHash) return false;

            var hashCalculado = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password), salt, Iteraciones,
                HashAlgorithmName.SHA256, TamanioHash);

            return CryptographicOperations.FixedTimeEquals(hashCalculado, hashEsperado);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
