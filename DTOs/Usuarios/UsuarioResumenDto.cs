using System;

namespace BibliotecaApp.DTOs.Usuarios;

/// <summary>
/// DTO con información de resumen de usuario para el consumo de endpoints y listas sin exponer datos sensibles.
/// </summary>
public class UsuarioResumenDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Identificacion { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
}
