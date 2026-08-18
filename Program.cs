using System;
using System.Linq;
using BibliotecaApp.Models;
using BibliotecaApp.UI;

namespace BibliotecaApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Biblioteca biblioteca = new Biblioteca();

            // Datos iniciales de prueba
            LibroFisico libro1 = new LibroFisico("Cien años de soledad", "Gabriel García Márquez", "978-03", 1967, "Novela", 3, "Estante A1");
            LibroFisico libro2 = new LibroFisico("El Principito", "Antoine de Saint-Exupéry", "978-01", 1943, "Fábula", 5, "Estante B2");
            Estudiante usuario1 = new Estudiante("Ana Torres", "01234567-8", "AT202401", "Ingeniería en Computación");

            biblioteca.AgregarLibro(libro1);
            biblioteca.AgregarLibro(libro2);
            biblioteca.RegistrarUsuario(usuario1);

            bool salir = false;

            while (!salir)
            {
                ConsoleViews.Banner();
                ConsoleViews.MenuPrincipal();

                string opcion = Console.ReadLine()?.Trim() ?? string.Empty;

                try
                {
                    switch (opcion)
                    {
                        case "1":
                            ConsoleViews.Header("CATÁLOGO DE LIBROS");
                            if (biblioteca.Libros.Count == 0)
                                ConsoleViews.MensajeAdvertencia("No hay libros registrados.");
                            else
                            {
                                foreach (var lib in biblioteca.Libros)
                                {
                                    Console.WriteLine($"  Título: {lib.Titulo} - {lib.Autor}");
                                    Console.WriteLine($"  Disponible: {(lib.HayDisponibilidad() ? "SÍ" : "NO")}\n");
                                }
                            }
                            break;

                        case "2":
                            ConsoleViews.Header("LISTADO DE USUARIOS");
                            if (biblioteca.Usuarios.Count == 0)
                                ConsoleViews.MensajeAdvertencia("No hay usuarios registrados.");
                            else
                            {
                                foreach (var user in biblioteca.Usuarios)
                                {
                                    Console.WriteLine($"  Nombre: {user.Nombre}");
                                    // Mostramos el Carnet si es estudiante, si no, la Identificación general
                                    string idMostrar = user is Estudiante est ? est.Carne : user.Identificacion;
                                    Console.WriteLine($"  Carnet/ID: {idMostrar}\n");
                                }
                            }
                            break;

                        case "3":
                            ConsoleViews.Header("REGISTRAR NUEVO ESTUDIANTE");
                            string nombre = ConsoleViews.LeerTexto("Nombre del Estudiante");
                            string identificacion = ConsoleViews.LeerTexto("DUI / Identificación");
                            string carnet = ConsoleViews.LeerTexto("Carnet de la UDB");
                            string carrera = ConsoleViews.LeerTexto("Carrera");

                            Estudiante nuevoEstudiante = new Estudiante(nombre, identificacion, carnet, carrera);
                            biblioteca.RegistrarUsuario(nuevoEstudiante);
                            ConsoleViews.MensajeExito($"Estudiante '{nuevoEstudiante.Nombre}' registrado con éxito.");
                            break;

                        case "4":
                            ConsoleViews.Header("PRESTAR UN LIBRO");
                            // Búsqueda amigable
                            string tituloLibro = ConsoleViews.LeerTexto("Ingrese el Título del Libro (o parte de él)");
                            string carnetUsuario = ConsoleViews.LeerTexto("Ingrese el Carnet o Identificación del Usuario");

                            // Buscamos internamente en la memoria
                            var libroPrestar = biblioteca.Libros.FirstOrDefault(l => l.Titulo.ToLower().Contains(tituloLibro.ToLower()));
                            var userPrestar = biblioteca.Usuarios.FirstOrDefault(u => u.Identificacion == carnetUsuario || (u is Estudiante e && e.Carne == carnetUsuario));

                            if (libroPrestar != null && userPrestar != null)
                            {
                                // Si los encontramos, usamos sus IDs largos internamente sin que el usuario los vea
                                var prestamo = biblioteca.PrestarLibro(libroPrestar.Id, userPrestar.Id);
                                if (prestamo != null)
                                    ConsoleViews.MensajeExito($"Préstamo realizado. Se entregó '{libroPrestar.Titulo}' a '{userPrestar.Nombre}'.");
                                else
                                    ConsoleViews.MensajeError("Error: Verifique que el libro tenga stock disponible.");
                            }
                            else
                            {
                                ConsoleViews.MensajeError("No se encontró ningún libro o usuario con los datos ingresados.");
                            }
                            break;

                        case "5":
                            ConsoleViews.Header("DEVOLVER UN LIBRO");
                            var prestamosActivos = biblioteca.ListarPrestamosActivos();

                            if (prestamosActivos.Count == 0)
                            {
                                ConsoleViews.MensajeAdvertencia("Actualmente no hay préstamos activos para devolver.");
                            }
                            else
                            {
                                Console.WriteLine("  Préstamos Activos:");
                                // Mostramos una lista numerada del 1 en adelante
                                for (int i = 0; i < prestamosActivos.Count; i++)
                                {
                                    Console.WriteLine($"  [{i + 1}] Libro: {prestamosActivos[i].Libro.Titulo} | Usuario: {prestamosActivos[i].Usuario.Nombre}");
                                }

                                int? indice = ConsoleViews.LeerEntero("\n  Seleccione el NÚMERO del préstamo que desea devolver");

                                if (indice.HasValue && indice.Value > 0 && indice.Value <= prestamosActivos.Count)
                                {
                                    // Tomamos el préstamo correspondiente y usamos su ID internamente
                                    var prestamoSeleccionado = prestamosActivos[indice.Value - 1];
                                    bool exito = biblioteca.DevolverLibro(prestamoSeleccionado.Id);
                                    if (exito)
                                        ConsoleViews.MensajeExito($"El libro '{prestamoSeleccionado.Libro.Titulo}' ha sido devuelto correctamente.");
                                    else
                                        ConsoleViews.MensajeError("No se pudo procesar la devolución.");
                                }
                                else
                                {
                                    ConsoleViews.MensajeError("Opción no válida.");
                                }
                            }
                            break;

                        case "6":
                            ConsoleViews.Header("DETALLE DE LIBRO");
                            string tituloBusqueda = ConsoleViews.LeerTexto("Ingrese el Título del Libro a buscar");

                            var libroEncontrado = biblioteca.Libros.FirstOrDefault(l => l.Titulo.ToLower().Contains(tituloBusqueda.ToLower()));

                            if (libroEncontrado != null)
                            {
                                Console.WriteLine($"\n  Título: {libroEncontrado.Titulo}");
                                Console.WriteLine($"  Autor: {libroEncontrado.Autor}");
                                Console.WriteLine($"  ISBN: {libroEncontrado.ISBN}");
                                Console.WriteLine($"  Año: {libroEncontrado.AnioPublicacion}");
                                Console.WriteLine($"  Disponible: {(libroEncontrado.HayDisponibilidad() ? "SÍ" : "NO")}");
                            }
                            else
                                ConsoleViews.MensajeError("Libro no encontrado.");
                            break;

                        case "7":
                            salir = true;
                            ConsoleViews.MensajeExito("Saliendo del sistema. ¡Hasta pronto!");
                            break;

                        default:
                            ConsoleViews.MensajeError("Opción no válida. Ingrese un número entre 1 y 7.");
                            break;
                    }
                }
                catch (ArgumentException ex)
                {
                    ConsoleViews.MensajeError(ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    ConsoleViews.MensajeError(ex.Message);
                }
                catch (Exception ex)
                {
                    ConsoleViews.MensajeError($"Ocurrió un error en el sistema: {ex.Message}");
                }

                if (!salir)
                {
                    ConsoleViews.EsperarTecla();
                }
            }
        }
    }
}