using BibliotecaApp.Models;

namespace BibliotecaApp.UI;

/// <summary>
/// Renderiza en consola las vistas relacionadas con Usuario, Estudiante y Docente.
/// </summary>
public static class UsuarioView
{
    public static void RenderizarLista(IEnumerable<Usuario> usuarios)
    {
        var lista = usuarios.ToList();

        ConsoleViews.Header("USUARIOS REGISTRADOS");

        if (lista.Count == 0)
        {
            ConsoleViews.MensajeAdvertencia("No hay usuarios registrados en el sistema.");
            return;
        }

        for (int i = 0; i < lista.Count; i++)
        {
            var u = lista[i];

            // Número + nombre completo + badge de rol
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write($"  [{i + 1}]");
            Console.ResetColor();
            Console.Write($"  {u.Nombre}   ");

            var colorRol = u is Estudiante ? ConsoleColor.Cyan : ConsoleColor.Yellow;
            ConsoleViews.Badge(u.ObtenerRol(), colorRol);
            Console.WriteLine();

            // Segunda línea: detalle específico del tipo
            Console.ForegroundColor = ConsoleColor.DarkGray;
            var detalle = u switch
            {
                Estudiante e => $"Carne: {e.Carne}  |  Carrera: {e.Carrera}",
                Docente d    => $"Empleado: {d.NumeroEmpleado}  |  Depto: {d.Departamento}",
                _            => ""
            };
            Console.WriteLine($"       ID: {u.Identificacion}  |  {detalle}");
            Console.ResetColor();
            Console.WriteLine();
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"  Total: {lista.Count} usuario(s) registrado(s).");
        Console.ResetColor();
    }

    /// <summary>
    /// Formulario interactivo para registrar un nuevo usuario (Estudiante o Docente).
    /// Retorna null si el usuario cancela o ingresa datos inválidos.
    /// </summary>
    public static Usuario? FormularioRegistro()
    {
        ConsoleViews.Header("REGISTRAR NUEVO USUARIO");

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("  [1]  Estudiante");
        Console.WriteLine("  [2]  Docente");
        Console.ResetColor();

        var rolInput = ConsoleViews.LeerTexto("Seleccione el tipo de usuario");

        var esRolValido = rolInput switch
        {
            "1" or "2" => true,
            _           => false
        };

        if (!esRolValido)
        {
            ConsoleViews.MensajeError("Opcion no valida. Debe ingresar 1 o 2.");
            return null;
        }

        var nombre = ConsoleViews.LeerTexto("Nombre completo");
        if (string.IsNullOrWhiteSpace(nombre))
        {
            ConsoleViews.MensajeError("El nombre no puede estar vacio.");
            return null;
        }

        var identificacion = ConsoleViews.LeerTexto("Numero de identificacion (DUI/otro)");
        if (string.IsNullOrWhiteSpace(identificacion))
        {
            ConsoleViews.MensajeError("La identificacion no puede estar vacia.");
            return null;
        }

        try
        {
            return rolInput switch
            {
                "1" => CrearEstudiante(nombre, identificacion),
                "2" => CrearDocente(nombre, identificacion),
                _   => null
            };
        }
        catch (ArgumentException ex)
        {
            ConsoleViews.MensajeError($"Datos invalidos: {ex.Message}");
            return null;
        }
    }

    private static Estudiante CrearEstudiante(string nombre, string identificacion)
    {
        ConsoleViews.Header("DATOS DE ESTUDIANTE");

        var carne = ConsoleViews.LeerTexto("Numero de carne");
        if (string.IsNullOrWhiteSpace(carne))
            throw new ArgumentException("El carne no puede estar vacio.", nameof(carne));

        var carrera = ConsoleViews.LeerTexto("Carrera o programa academico");
        if (string.IsNullOrWhiteSpace(carrera))
            throw new ArgumentException("La carrera no puede estar vacia.", nameof(carrera));

        return new Estudiante(nombre, identificacion, carne, carrera);
    }

    private static Docente CrearDocente(string nombre, string identificacion)
    {
        ConsoleViews.Header("DATOS DE DOCENTE");

        var numEmpleado = ConsoleViews.LeerTexto("Numero de empleado");
        if (string.IsNullOrWhiteSpace(numEmpleado))
            throw new ArgumentException("El numero de empleado no puede estar vacio.", nameof(numEmpleado));

        var departamento = ConsoleViews.LeerTexto("Departamento academico");
        if (string.IsNullOrWhiteSpace(departamento))
            throw new ArgumentException("El departamento no puede estar vacio.", nameof(departamento));

        return new Docente(nombre, identificacion, numEmpleado, departamento);
    }

    /// <summary>
    /// Muestra la lista numerada de usuarios y retorna el que el usuario seleccione.
    /// Retorna null si la selección es inválida.
    /// </summary>
    public static Usuario? FormularioSeleccionUsuario(IReadOnlyList<Usuario> usuarios)
    {
        ConsoleViews.Header("SELECCIONAR USUARIO");

        if (usuarios.Count == 0)
        {
            ConsoleViews.MensajeAdvertencia("No hay usuarios registrados.");
            return null;
        }

        for (int i = 0; i < usuarios.Count; i++)
        {
            var u = usuarios[i];

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write($"  [{i + 1}]");
            Console.ResetColor();
            Console.Write($"  {u.Nombre}   ");

            var colorRol = u is Estudiante ? ConsoleColor.Cyan : ConsoleColor.Yellow;
            ConsoleViews.Badge(u.ObtenerRol(), colorRol);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"       ID: {u.Identificacion}");
            Console.ResetColor();
            Console.WriteLine();
        }

        var numero = ConsoleViews.LeerEntero("Ingrese el numero del usuario");
        if (numero is null || numero < 1 || numero > usuarios.Count)
        {
            ConsoleViews.MensajeError("Numero fuera de rango.");
            return null;
        }

        return usuarios[numero.Value - 1];
    }
}
