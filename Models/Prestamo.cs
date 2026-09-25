using System;

namespace BibliotecaApp.Models;

/// <summary>
/// Registra la relación entre un Usuario y un Libro durante un período de tiempo.
/// </summary>
public sealed class Prestamo : EntidadBase
{
    public Usuario Usuario { get; private set; } = null!;
    public Libro Libro { get; private set; } = null!;
    public DateTime FechaPrestamo { get; private set; }
    public DateTime FechaDevolucionEsperada { get; private set; }

    // null mientras el préstamo está activo
    public DateTime? FechaDevolucionReal { get; private set; }

    public bool EstaActivo => FechaDevolucionReal is null;
    public bool EstaVencido => EstaActivo && DateTime.Now > FechaDevolucionEsperada;

    // --- Constructor vacío para EF Core ---
    private Prestamo() { }

    public Prestamo(Usuario usuario, Libro libro, int diasPrestamo = 15)
    {
        Usuario = usuario ?? throw new ArgumentNullException(nameof(usuario));
        Libro = libro ?? throw new ArgumentNullException(nameof(libro));

        if (diasPrestamo <= 0)
            throw new ArgumentOutOfRangeException(nameof(diasPrestamo),
                "La duración del préstamo debe ser mayor a 0 días.");

        FechaPrestamo = DateTime.Now;
        FechaDevolucionEsperada = FechaPrestamo.AddDays(diasPrestamo);
        FechaDevolucionReal = null;
    }

    public void RegistrarDevolucion()
    {
        if (!EstaActivo)
            throw new InvalidOperationException(
                $"El préstamo de '{Libro.Titulo}' ya fue devuelto el {FechaDevolucionReal:dd/MM/yyyy}.");

        FechaDevolucionReal = DateTime.Now;
    }
}