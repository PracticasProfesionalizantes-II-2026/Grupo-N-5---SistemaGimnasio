/*using System.Security.Cryptography;

public static class PasswordHelper
{
    public static void CrearHash(string password, out byte[] hash, out byte[] salt)
    {
        using var hmac = new HMACSHA512();
        salt = hmac.Key;
        hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    }

    public static bool VerificarHash(string password, byte[] hashGuardado, byte[] saltGuardado)
    {
        using var hmac = new HMACSHA512(saltGuardado);
        var hashCalculado = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return hashCalculado.SequenceEqual(hashGuardado);
    }
}
*/