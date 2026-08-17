using BibliotecaApp.Models;

namespace BibliotecaApp.Interfaces;

/// <summary>
/// Contrato del servicio de préstamos.
/// Principio SOLID: DIP — desacopla la lógica de préstamos de su consumidor (la UI).
/// </summary>
public interface IPrestamoService
{
    /// <summary>Devuelve todos los préstamos (activos e históricos).</summary>
    IReadOnlyList<Prestamo> ObtenerTodos();

    /// <summary>Devuelve solo los préstamos activos (no devueltos aún).</summary>
    IEnumerable<Prestamo> ObtenerActivos();

    /// <summary>
    /// Crea un nuevo préstamo, validando disponibilidad del libro.
    /// </summary>
    /// <param name="usuario">Usuario que realiza el préstamo.</param>
    /// <param name="libro">Libro que se desea prestar.</param>
    /// <returns>El objeto Prestamo recién creado.</returns>
    Prestamo CrearPrestamo(Usuario usuario, Libro libro);

    /// <summary>
    /// Registra la devolución del préstamo con el Id dado.
    /// </summary>
    /// <param name="prestamoId">Id del préstamo a devolver.</param>
    /// <returns>El objeto Prestamo actualizado.</returns>
    Prestamo RegistrarDevolucion(Guid prestamoId);

    /// <summary>
    /// Obtiene los préstamos activos de un usuario específico.
    /// </summary>
    IEnumerable<Prestamo> ObtenerPrestamosPorUsuario(Guid usuarioId);
}
