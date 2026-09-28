using System;
using BibliotecaApp.Enums;

namespace BibliotecaApp.Models;

/// <summary>
/// Encargado de la gestión de la biblioteca (Administrador del sistema de préstamos e inventario).
/// </summary>
public sealed class Bibliotecario : Usuario
{
    public string CodigoPersonal { get; private set; } = string.Empty;

    private Bibliotecario() { }

    public Bibliotecario(string nombre, string identificacion, string email, string passwordHash, string codigoPersonal = "")
        : base(nombre, identificacion, email, passwordHash, RolUsuario.Bibliotecario)
    {
        CodigoPersonal = codigoPersonal;
    }

    public override string ObtenerRol() => "BIBLIOTECARIO";
}
