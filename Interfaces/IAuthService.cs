using Microsoft.AspNetCore.Http;
using BibliotecaApp.DTOs.Auth;

namespace BibliotecaApp.Interfaces;

/// <summary>
/// Contrato para el servicio de autenticación y gestión de credenciales.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Valida credenciales de usuario e inicia la sesión en el contexto HTTP (Cookie Auth).
    /// </summary>
    Task<AuthResult> IniciarSesionAsync(LoginDto loginDto, HttpContext httpContext);

    /// <summary>
    /// Registra un nuevo usuario con contraseña hasheada y asignación de rol.
    /// </summary>
    Task<AuthResult> RegistrarUsuarioAsync(RegisterDto registerDto);

    /// <summary>
    /// Cierra la sesión activa del usuario.
    /// </summary>
    Task CerrarSesionAsync(HttpContext httpContext);

    /// <summary>
    /// Genera un hash criptográfico PBKDF2 para la contraseña dada.
    /// </summary>
    string HashearPassword(string password);

    /// <summary>
    /// Verifica si una contraseña en texto plano coincide con el hash almacenado.
    /// </summary>
    bool VerificarPassword(string password, string hash);
}
