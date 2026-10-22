using System;
using System.Collections.Generic;
using BibliotecaApp.DTOs.Usuarios;
using BibliotecaApp.Models;

namespace BibliotecaApp.Interfaces;

/// <summary>
/// Contrato del servicio de gestión de usuarios.
/// Desacopla la consulta y manipulación de cuentas de los controladores.
/// </summary>
public interface IUsuarioService : IBuscable<Usuario>
{
    /// <summary>Devuelve todos los usuarios registrados.</summary>
    IReadOnlyList<Usuario> ObtenerTodos();

    /// <summary>Devuelve el resumen seguro de todos los usuarios (sin contraseñas) para APIs y vistas.</summary>
    IReadOnlyList<UsuarioResumenDto> ObtenerResumenTodos();

    /// <summary>Obtiene un usuario por su correo electrónico institucional.</summary>
    Usuario? ObtenerPorEmail(string email);

    /// <summary>Registra un nuevo usuario en el sistema.</summary>
    void Registrar(Usuario usuario);

    /// <summary>Verifica si ya existe un usuario con la misma identificación.</summary>
    bool ExisteIdentificacion(string identificacion);
}
