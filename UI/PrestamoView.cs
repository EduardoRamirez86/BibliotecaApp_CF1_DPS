using BibliotecaApp.Models;

namespace BibliotecaApp.UI;

/// <summary>
/// Renderiza en consola las vistas relacionadas con la entidad Prestamo.
/// </summary>
public static class PrestamoView
{
    public static void RenderizarLista(IEnumerable<Prestamo> prestamos)
    {
        var lista = prestamos.ToList();

        ConsoleViews.Header("REGISTRO DE PRESTAMOS");

        if (lista.Count == 0)
        {
            ConsoleViews.MensajeAdvertencia("No hay prestamos registrados.");
            return;
        }

        for (int i = 0; i < lista.Count; i++)
        {
            var p = lista[i];

            // Número + usuario -> libro + badge de estado
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write($"  [{i + 1}]");
            Console.ResetColor();
            Console.Write($"  {p.Usuario.Nombre}  ->  {p.Libro.Titulo}   ");

            var (estadoTexto, estadoColor) = (p.EstaActivo, p.EstaVencido) switch
            {
                (false, _)   => ("DEVUELTO", ConsoleColor.DarkGray),
                (true, true) => ("VENCIDO",  ConsoleColor.Red),
                (true, _)    => ("ACTIVO",   ConsoleColor.Green)
            };
            ConsoleViews.Badge(estadoTexto, estadoColor);
            Console.WriteLine();

            // Segunda línea: fechas
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"       Prestamo: {p.FechaPrestamo:dd/MM/yyyy}  |  Devol. esperada: {p.FechaDevolucionEsperada:dd/MM/yyyy}");
            Console.ResetColor();
            Console.WriteLine();
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"  Total: {lista.Count} prestamo(s) registrado(s).");
        Console.ResetColor();
    }

    /// <summary>
    /// Muestra los préstamos activos numerados y retorna el Id del que el usuario seleccione.
    /// Retorna null si no hay activos o la selección es inválida.
    /// </summary>
    public static Guid? FormularioDevolucion(IEnumerable<Prestamo> prestamosActivos)
    {
        var lista = prestamosActivos.ToList();

        ConsoleViews.Header("REGISTRAR DEVOLUCION");

        if (lista.Count == 0)
        {
            ConsoleViews.MensajeAdvertencia("No hay prestamos activos para devolver.");
            return null;
        }

        for (int i = 0; i < lista.Count; i++)
        {
            var p = lista[i];

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write($"  [{i + 1}]");
            Console.ResetColor();
            Console.Write($"  {p.Usuario.Nombre}  ->  '{p.Libro.Titulo}'");

            if (p.EstaVencido)
            {
                Console.Write("   ");
                ConsoleViews.Badge("VENCIDO", ConsoleColor.Red);
            }

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"       Prestamo: {p.FechaPrestamo:dd/MM/yyyy}  |  Devol. esperada: {p.FechaDevolucionEsperada:dd/MM/yyyy}");
            Console.ResetColor();
            Console.WriteLine();
        }

        var numero = ConsoleViews.LeerEntero("Ingrese el numero del prestamo a devolver");
        if (numero is null || numero < 1 || numero > lista.Count)
        {
            ConsoleViews.MensajeError("Numero fuera de rango.");
            return null;
        }

        return lista[numero.Value - 1].Id;
    }

    public static void ConfirmarPrestamo(Prestamo prestamo)
    {
        ConsoleViews.Header("PRESTAMO REGISTRADO");

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("  ID Prestamo:");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"  {prestamo.Id}");
        Console.ResetColor();

        ImprimirCampo("Usuario", $"{prestamo.Usuario.Nombre} ({prestamo.Usuario.ObtenerRol()})");
        ImprimirCampo("Libro", $"{prestamo.Libro.Titulo} - {prestamo.Libro.Autor}");
        ImprimirCampo("Fecha de prestamo", prestamo.FechaPrestamo.ToString("dd/MM/yyyy HH:mm"));
        ImprimirCampo("Devolucion esperada", prestamo.FechaDevolucionEsperada.ToString("dd/MM/yyyy"));
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
