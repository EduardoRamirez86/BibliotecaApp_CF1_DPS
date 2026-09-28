using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BibliotecaApp.Data;
using BibliotecaApp.DTOs.Auth;
using BibliotecaApp.Enums;
using BibliotecaApp.Interfaces;
using BibliotecaApp.Models;

namespace BibliotecaApp.Services;

/// <summary>
/// Servicio de autenticación, registro y hashing criptográfico seguro (PBKDF2/SHA256).
/// </summary>
public class AuthService : IAuthService
{
    private readonly BibliotecaDbContext _context;
    private readonly ILogger<AuthService> _logger;
    private const int SaltSize = 16; // 128 bits
    private const int KeySize = 32;  // 256 bits
    private const int Iterations = 100_000;

    public AuthService(BibliotecaDbContext context, ILogger<AuthService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AuthResult> IniciarSesionAsync(LoginDto loginDto, HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(loginDto);
        ArgumentNullException.ThrowIfNull(httpContext);

        var emailNormalizado = loginDto.Email.Trim().ToLowerInvariant();
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == emailNormalizado);

        if (usuario == null || !VerificarPassword(loginDto.Password, usuario.PasswordHash))
        {
            _logger.LogWarning("Intento fallido de inicio de sesión para el correo: {Email}", emailNormalizado);
            return AuthResult.Falla("Correo electrónico o contraseña incorrectos.");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nombre),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Role, usuario.ObtenerRol())
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = loginDto.Recordarme,
            ExpiresUtc = loginDto.Recordarme
                ? DateTimeOffset.UtcNow.AddDays(7)
                : DateTimeOffset.UtcNow.AddHours(4),
            AllowRefresh = true
        };

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        _logger.LogInformation("Usuario {Email} con rol {Rol} inició sesión exitosamente.", usuario.Email, usuario.ObtenerRol());
        return AuthResult.Ok(usuario, "Inicio de sesión exitoso.");
    }

    public async Task<AuthResult> RegistrarUsuarioAsync(RegisterDto registerDto)
    {
        ArgumentNullException.ThrowIfNull(registerDto);

        var emailNormalizado = registerDto.Email.Trim().ToLowerInvariant();
        var existeEmail = await _context.Usuarios
            .AnyAsync(u => u.Email == emailNormalizado);

        if (existeEmail)
        {
            return AuthResult.Falla("El correo electrónico ya se encuentra registrado en el sistema.");
        }

        var passwordHash = HashearPassword(registerDto.Password);
        var identificacionAuto = $"UDB-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";

        Usuario nuevoUsuario = registerDto.Rol switch
        {
            RolUsuario.Estudiante => new Estudiante(
                nombre: registerDto.NombreCompleto.Trim(),
                identificacion: identificacionAuto,
                email: emailNormalizado,
                passwordHash: passwordHash),

            RolUsuario.Docente => new Docente(
                nombre: registerDto.NombreCompleto.Trim(),
                identificacion: identificacionAuto,
                email: emailNormalizado,
                passwordHash: passwordHash),

            RolUsuario.Bibliotecario => new Bibliotecario(
                nombre: registerDto.NombreCompleto.Trim(),
                identificacion: identificacionAuto,
                email: emailNormalizado,
                passwordHash: passwordHash),

            _ => throw new InvalidOperationException($"Rol no soportado: {registerDto.Rol}")
        };

        await _context.Usuarios.AddAsync(nuevoUsuario);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Nuevo usuario registrado: {Email} con rol {Rol}.", nuevoUsuario.Email, nuevoUsuario.ObtenerRol());
        return AuthResult.Ok(nuevoUsuario, "Usuario registrado exitosamente.");
    }

    public async Task CerrarSesionAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    public string HashearPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("La contraseña no puede estar vacía.", nameof(password));

        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password: Encoding.UTF8.GetBytes(password),
            salt: salt,
            iterations: Iterations,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: KeySize);

        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    public bool VerificarPassword(string password, string hashAlmacenado)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashAlmacenado))
            return false;

        var partes = hashAlmacenado.Split(':');
        if (partes.Length != 2)
            return false;

        try
        {
            byte[] salt = Convert.FromBase64String(partes[0]);
            byte[] hashEsperado = Convert.FromBase64String(partes[1]);

            byte[] hashActual = Rfc2898DeriveBytes.Pbkdf2(
                password: Encoding.UTF8.GetBytes(password),
                salt: salt,
                iterations: Iterations,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: KeySize);

            return CryptographicOperations.FixedTimeEquals(hashEsperado, hashActual);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
