using BibliotecaApp.Interfaces;
using BibliotecaApp.Models;

namespace BibliotecaApp.Services;

/// <summary>
/// Orquesta la lógica de préstamos: crea registros, valida disponibilidad
/// y delega la actualización de stock a los propios modelos.
/// </summary>
public class PrestamoService : IPrestamoService
{
    private readonly List<Prestamo> _prestamos = new();

    public IReadOnlyList<Prestamo> ObtenerTodos() => _prestamos.AsReadOnly();

    public IEnumerable<Prestamo> ObtenerActivos() => _prestamos.Where(p => p.EstaActivo);

    public IEnumerable<Prestamo> ObtenerPrestamosPorUsuario(Guid usuarioId)
        => _prestamos.Where(p => p.Usuario.Id == usuarioId);

    /// <summary>
    /// Crea un préstamo validando que el usuario no tenga ya ese libro activo
    /// y que haya stock disponible.
    /// </summary>
    public Prestamo CrearPrestamo(Usuario usuario, Libro libro)
    {
        ArgumentNullException.ThrowIfNull(usuario);
        ArgumentNullException.ThrowIfNull(libro);

        // Un usuario no puede tener el mismo libro prestado dos veces simultáneamente
        var yaLoPrestado = _prestamos.Any(p =>
            p.EstaActivo &&
            p.Usuario.Id == usuario.Id &&
            p.Libro.Id == libro.Id);

        if (yaLoPrestado)
            throw new InvalidOperationException(
                $"'{usuario.Nombre}' ya tiene activo un préstamo de '{libro.Titulo}'.");

        if (libro is LibroFisico lf)
            lf.RealizarPrestamo();
        else
            throw new InvalidOperationException($"Tipo de libro no soportado: '{libro.GetType().Name}'.");

        var prestamo = new Prestamo(usuario, libro);
        _prestamos.Add(prestamo);
        return prestamo;
    }

    /// <summary>
    /// Registra la devolución y libera el stock del libro.
    /// Lanza KeyNotFoundException si el Id no existe, InvalidOperationException si ya fue devuelto.
    /// </summary>
    public Prestamo RegistrarDevolucion(Guid prestamoId)
    {
        var prestamo = _prestamos.FirstOrDefault(p => p.Id == prestamoId)
            ?? throw new KeyNotFoundException($"No se encontró el préstamo con Id '{prestamoId}'.");

        prestamo.RegistrarDevolucion();

        if (prestamo.Libro is LibroFisico lf)
            lf.RegistrarDevolucion();

        return prestamo;
    }
}
