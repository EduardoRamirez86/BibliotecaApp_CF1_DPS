using System;

namespace BibliotecaApp.DTOs.Prestamos;

/// <summary>
/// DTO con el detalle seguro de un préstamo para visualización en APIs y reportes.
/// </summary>
public class PrestamoDetalleDto
{
    public Guid Id { get; set; }
    public DateTime FechaPrestamo { get; set; }
    public DateTime FechaDevolucionEsperada { get; set; }
    public DateTime? FechaDevolucionReal { get; set; }
    public bool EstaActivo { get; set; }
    public bool EstaVencido { get; set; }
    public string UsuarioNombre { get; set; } = string.Empty;
    public string IdentificacionUsuario { get; set; } = string.Empty;
    public string LibroTitulo { get; set; } = string.Empty;
    public string IsbnLibro { get; set; } = string.Empty;
}
