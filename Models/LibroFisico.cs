namespace BibliotecaApp.Models;

/// <summary>
/// Libro con ejemplares físicos en estante.
/// Controla disponibilidad mediante StockTotal y StockPrestado.
/// </summary>
public sealed class LibroFisico : Libro
{
    public int StockTotal { get; private set; }
    public int StockPrestado { get; private set; }
    public string UbicacionEstante { get; private set; }
    public int StockDisponible => StockTotal - StockPrestado;

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

    public override string ObtenerTipo() => "FISICO";
}
