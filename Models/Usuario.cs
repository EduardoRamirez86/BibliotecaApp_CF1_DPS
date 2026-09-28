using System;
using BibliotecaApp.Enums;

namespace BibliotecaApp.Models;

/// <summary>
/// Clase abstracta que representa a cualquier usuario del sistema con credenciales de acceso.
/// </summary>
public abstract class Usuario : EntidadBase
{
    public string Nombre { get; protected set; } = null!;
    public string Identificacion { get; protected set; } = null!;
    public string Email { get; protected set; } = string.Empty;
    public string PasswordHash { get; protected set; } = string.Empty;
    public RolUsuario Rol { get; protected set; }

    protected Usuario() { }

    protected Usuario(string nombre, string identificacion)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre no puede estar vacío.", nameof(nombre));
        if (string.IsNullOrWhiteSpace(identificacion))
            throw new ArgumentException("La identificación no puede estar vacía.", nameof(identificacion));

        Nombre = nombre;
        Identificacion = identificacion;
    }

    protected Usuario(string nombre, string identificacion, string email, string passwordHash, RolUsuario rol)
        : this(nombre, identificacion)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El correo electrónico no puede estar vacío.", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("El hash de contraseña no puede estar vacío.", nameof(passwordHash));

        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        Rol = rol;
    }

    public abstract string ObtenerRol();

    public override string ToString() => $"[{ObtenerRol()}] {Nombre} (Email: {Email})";
}
