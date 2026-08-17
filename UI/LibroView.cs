using BibliotecaApp.Models;

namespace BibliotecaApp.UI;

/// <summary>
/// Renderiza en consola las vistas relacionadas con Libro y LibroFisico.
/// </summary>
public static class LibroView
{
    public static void RenderizarCatalogo(IEnumerable<Libro> libros)
    {
        var lista = libros.ToList();

        ConsoleViews.Header("CATALOGO DE LIBROS");

        if (lista.Count == 0)
        {
            ConsoleViews.MensajeAdvertencia("No hay libros registrados en el catalogo.");
            return;
        }

        for (int i = 0; i < lista.Count; i++)
        {
            var libro = lista[i];

            // Número + título completo
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write($"  [{i + 1}]");
            Console.ResetColor();
            Console.WriteLine($"  {libro.Titulo}");

            // Autor + disponibilidad en la segunda línea
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"       {libro.Autor}   ");
            Console.ResetColor();

            if (libro.HayDisponibilidad())
                ConsoleViews.Badge("DISPONIBLE", ConsoleColor.Green);
            else
                ConsoleViews.Badge("AGOTADO", ConsoleColor.Red);

            Console.WriteLine();
            Console.WriteLine();
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"  Total: {lista.Count} libro(s) en catalogo.");
        Console.ResetColor();
    }

    public static void RenderizarDetalle(Libro libro)
    {
        ConsoleViews.Header($"DETALLE - {libro.Titulo}");

        ImprimirCampo("ID", libro.Id.ToString());
        ImprimirCampo("Titulo", libro.Titulo);
        ImprimirCampo("Autor", libro.Autor);
        ImprimirCampo("ISBN", libro.ISBN);
        ImprimirCampo("Año publicacion", libro.AnioPublicacion.ToString());
        ImprimirCampo("Genero", libro.Genero);
        ImprimirCampo("Fecha de registro", libro.FechaCreacion.ToString("dd/MM/yyyy HH:mm"));

        Console.Write("  Disponibilidad:         ");
        if (libro.HayDisponibilidad())
            ConsoleViews.Badge("DISPONIBLE", ConsoleColor.Green);
        else
            ConsoleViews.Badge("AGOTADO", ConsoleColor.Red);
        Console.WriteLine();

        if (libro is LibroFisico lf)
        {
            ConsoleViews.Separador();
            ConsoleViews.Header("INFORMACION FISICA");
            ImprimirCampo("Stock total", lf.StockTotal.ToString());
            ImprimirCampo("Prestados", lf.StockPrestado.ToString());
            ImprimirCampo("Disponibles", lf.StockDisponible.ToString());
            ImprimirCampo("Ubicacion estante", lf.UbicacionEstante);
        }
    }

    /// <summary>
    /// Muestra la lista de libros numerada y retorna el libro que el usuario seleccione.
    /// Retorna null si la selección es inválida.
    /// </summary>
    public static Libro? FormularioSeleccionLibro(IReadOnlyList<Libro> libros)
    {
        ConsoleViews.Header("SELECCIONAR LIBRO");

        var disponibles = libros.Select((libro, idx) => (libro, idx: idx + 1)).ToList();

        if (disponibles.Count == 0)
        {
            ConsoleViews.MensajeAdvertencia("No hay libros en el catalogo.");
            return null;
        }

        foreach (var (libro, idx) in disponibles)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write($"  [{idx}]");
            Console.ResetColor();
            Console.Write($"  {libro.Titulo}");

            Console.Write("  ");
            if (libro.HayDisponibilidad())
                ConsoleViews.Badge("DISPONIBLE", ConsoleColor.Green);
            else
                ConsoleViews.Badge("AGOTADO", ConsoleColor.Red);

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"       {libro.Autor}");
            Console.ResetColor();
            Console.WriteLine();
        }

        var numero = ConsoleViews.LeerEntero("Ingrese el numero del libro");
        if (numero is null || numero < 1 || numero > disponibles.Count)
        {
            ConsoleViews.MensajeError("Numero fuera de rango.");
            return null;
        }

        return disponibles[numero.Value - 1].libro;
    }

    private static void ImprimirCampo(string etiqueta, string valor)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"  {etiqueta + ":",-25}");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(valor);
        Console.ResetColor();
    }
}
