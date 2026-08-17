namespace BibliotecaApp.UI;

/// <summary>
/// Métodos estáticos de presentación reutilizables: encabezados, badges,
/// mensajes de estado y lectura de inputs con formato consistente.
/// </summary>
public static class ConsoleViews
{
    private const int AnchoMarco = 60;

    public static void Banner()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine();
        Console.WriteLine("  ╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("  ║                                                          ║");
        Console.WriteLine("  ║         BIBLIOTECA UDB  (Fase 1)                         ║");
        Console.WriteLine("  ║         Sistema de Gestion de Prestamos                  ║");
        Console.WriteLine("  ║                                                          ║");
        Console.WriteLine("  ╚══════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"  {DateTime.Now:dddd, dd MMMM yyyy  HH:mm}");
        Console.ResetColor();
        Console.WriteLine();
    }

    /// <summary>
    /// Dibuja un encabezado de sección con línea separadora.
    /// </summary>
    public static void Header(string titulo)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  >> {titulo.ToUpper()}");
        Console.WriteLine("  " + new string('-', AnchoMarco));
        Console.ResetColor();
    }

    /// <summary>
    /// Imprime una etiqueta con corchetes y color. Ej: [DISPONIBLE] en verde.
    /// </summary>
    public static void Badge(string texto, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write($"[{texto}]");
        Console.ResetColor();
    }

    public static void MensajeExito(string mensaje)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n  OK  {mensaje}");
        Console.ResetColor();
    }

    public static void MensajeError(string mensaje)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n  ERROR: {mensaje}");
        Console.ResetColor();
    }

    public static void MensajeAdvertencia(string mensaje)
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine($"\n  AVISO: {mensaje}");
        Console.ResetColor();
    }

    public static void Separador()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("  " + new string('-', AnchoMarco));
        Console.ResetColor();
    }

    public static void EsperarTecla()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("  Presione ENTER para continuar...");
        Console.ResetColor();
        Console.ReadLine();
    }

    public static string LeerTexto(string etiqueta)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"  {etiqueta}: ");
        Console.ResetColor();
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Solicita un número entero. Retorna null si el input no es numérico,
    /// evitando que el programa lance una excepción no controlada.
    /// </summary>
    public static int? LeerEntero(string etiqueta)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"  {etiqueta}: ");
        Console.ResetColor();

        try
        {
            var input = Console.ReadLine()?.Trim();
            return int.Parse(input ?? string.Empty);
        }
        catch (FormatException)
        {
            MensajeError("Se esperaba un número entero. Intente de nuevo.");
            return null;
        }
    }

    public static void MenuPrincipal()
    {
        Header("MENU PRINCIPAL");

        var opciones = new[]
        {
            ("1", "Catalogo de Libros"),
            ("2", "Ver Usuarios"),
            ("3", "Registrar Usuario"),
            ("4", "Prestar Libro"),
            ("5", "Devolver Libro"),
            ("6", "Detalle de Libro"),
            ("7", "Salir")
        };

        foreach (var (num, desc) in opciones)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write($"  [{num}]");
            Console.ResetColor();
            Console.WriteLine($"  {desc}");
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("  Seleccione una opcion: ");
        Console.ResetColor();
    }
}
