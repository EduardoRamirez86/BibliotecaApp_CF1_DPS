using BibliotecaApp.Models;

namespace BibliotecaApp.DTOs.Auth;

/// <summary>
/// Resultado estructurado para operaciones de autenticación y registro.
/// </summary>
public class AuthResult
{
    public bool Exitoso { get; init; }
    public string Mensaje { get; init; } = string.Empty;
    public Usuario? Usuario { get; init; }

    public static AuthResult Ok(Usuario usuario, string mensaje = "Operación exitosa.")
        => new() { Exitoso = true, Mensaje = mensaje, Usuario = usuario };

    public static AuthResult Falla(string mensaje)
        => new() { Exitoso = false, Mensaje = mensaje, Usuario = null };
}
