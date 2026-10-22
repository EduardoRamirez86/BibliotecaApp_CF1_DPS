using System;

namespace BibliotecaApp.Models;

/// <summary>
/// Libro con ejemplares físicos en estante.
/// Controla disponibilidad mediante StockTotal y StockPrestado.
/// </summary>
public sealed class LibroFisico : Libro
{
    public int StockTotal { get; private set; } = 0;
    public int StockPrestado { get; private set; } = 0;
    public string UbicacionEstante { get; private set; } = null!;
    public int StockDisponible => StockTotal - StockPrestado;

    // --- Constructor vacío requerido por Entity Framework Core ---
    private LibroFisico() { }

    public LibroFisico(
        string titulo, string autor, string isbn,
        int anioPublicacion, string genero,
        int stockTotal, string ubicacionEstante)
        : base(titulo, autor, isbn, anioPublicacion, genero)
    {
        if (stockTotal <= 0)
            throw new ArgumentOutOfRangeException(nameof(stockTotal), "El stock debe ser mayor a 0.");
        if (string.IsNullOrWhiteSpace(ubicacionEstante))
            throw new ArgumentException("La ubicación del estante no puede estar vacía.", nameof(ubicacionEstante));

        StockTotal = stockTotal;
        StockPrestado = 0;
        UbicacionEstante = ubicacionEstante;
    }

    public override bool HayDisponibilidad() => StockDisponible > 0;

    /// <summary>Lanza InvalidOperationException si no hay stock disponible.</summary>
    public void RealizarPrestamo()
    {
        if (!HayDisponibilidad())
            throw new InvalidOperationException(
                $"No hay ejemplares disponibles de '{Titulo}'. Stock prestado: {StockPrestado}/{StockTotal}.");

        StockPrestado++;
    }

    /// <summary>Lanza InvalidOperationException si no hay préstamos activos.</summary>
    public void RegistrarDevolucion()
    {
        if (StockPrestado <= 0)
            throw new InvalidOperationException(
                $"No hay préstamos activos para devolver de '{Titulo}'.");

        StockPrestado--;
    }

    /// <summary>Actualiza las propiedades del libro físico respetando invariantes de stock.</summary>
    public void ActualizarDatos(
        string titulo, string autor, string isbn,
        int anioPublicacion, string genero,
        int stockTotal, string ubicacionEstante)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("El título no puede estar vacío.", nameof(titulo));
        if (string.IsNullOrWhiteSpace(autor))
            throw new ArgumentException("El autor no puede estar vacío.", nameof(autor));
        if (string.IsNullOrWhiteSpace(isbn))
            throw new ArgumentException("El ISBN no puede estar vacío.", nameof(isbn));
        if (anioPublicacion < 1000 || anioPublicacion > DateTime.Now.Year)
            throw new ArgumentOutOfRangeException(nameof(anioPublicacion), "Año de publicación no válido.");
        if (stockTotal <= 0)
            throw new ArgumentOutOfRangeException(nameof(stockTotal), "El stock total debe ser mayor a 0.");
        if (stockTotal < StockPrestado)
            throw new InvalidOperationException($"El stock total no puede ser menor a los ejemplares actualmente prestados ({StockPrestado}).");
        if (string.IsNullOrWhiteSpace(ubicacionEstante))
            throw new ArgumentException("La ubicación del estante no puede estar vacía.", nameof(ubicacionEstante));

        Titulo = titulo;
        Autor = autor;
        ISBN = isbn;
        AnioPublicacion = anioPublicacion;
        Genero = string.IsNullOrWhiteSpace(genero) ? "Sin clasificar" : genero;
        StockTotal = stockTotal;
        UbicacionEstante = ubicacionEstante;
    }

    public override string ObtenerTipo() => "FISICO";
}