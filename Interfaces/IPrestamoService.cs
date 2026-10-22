using System;
using System.Collections.Generic;
using BibliotecaApp.DTOs.Prestamos;
using BibliotecaApp.Models;

namespace BibliotecaApp.Interfaces;

/// <summary>
/// Contrato del servicio de préstamos y devoluciones.
/// Centraliza la orquestación de usuarios, inventario de libros y reglas de negocio.
/// </summary>
public interface IPrestamoService
{
    /// <summary>Devuelve todos los préstamos (activos e históricos).</summary>
    IReadOnlyList<Prestamo> ObtenerTodos();

    /// <summary>Devuelve la lista completa de préstamos proyectada a DTO seguro.</summary>
    IReadOnlyList<PrestamoDetalleDto> ObtenerResumenTodos();

    /// <summary>Devuelve solo los préstamos activos (no devueltos aún).</summary>
    IEnumerable<Prestamo> ObtenerActivos();

    /// <summary>Obtiene los préstamos de un usuario específico por su Id.</summary>
    IEnumerable<Prestamo> ObtenerPrestamosPorUsuario(Guid usuarioId);

    /// <summary>Obtiene los préstamos de un usuario específico por su correo institucional.</summary>
    IEnumerable<Prestamo> ObtenerPrestamosPorEmail(string email);

    /// <summary>Crea un nuevo préstamo entre un usuario y libro ya instanciados.</summary>
    Prestamo CrearPrestamo(Usuario usuario, Libro libro);

    /// <summary>Crea un préstamo resolviendo internamente el usuario autenticado y el libro por Id.</summary>
    Prestamo CrearPrestamoPorEmail(string email, Guid idLibro);

    /// <summary>Crea un préstamo resolviendo internamente por identificación de usuario y código ISBN.</summary>
    Prestamo CrearPrestamoPorIdentificacion(string identificacionUsuario, string isbnLibro, int diasPrestamo = 15);

    /// <summary>Registra la devolución del préstamo restaurando el inventario disponible.</summary>
    Prestamo RegistrarDevolucion(Guid prestamoId);
}
