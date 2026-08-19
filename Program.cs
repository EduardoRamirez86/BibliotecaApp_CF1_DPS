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

            // ============================================================
            //                         SEED DATA INICIAL
            // ============================================================
            InicializarSeedData(biblioteca);

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
                        case "1": // Catálogo de Libros
                            LibroView.RenderizarCatalogo(biblioteca.Libros);
                            break;

                        case "2": // Ver Usuarios
                            UsuarioView.RenderizarLista(biblioteca.Usuarios);
                            break;

                        case "3": // Registrar Usuario
                            var nuevoUsuario = UsuarioView.FormularioRegistro();
                            if (nuevoUsuario != null)
                            {
                                biblioteca.RegistrarUsuario(nuevoUsuario);
                                ConsoleViews.MensajeExito($"Usuario '{nuevoUsuario.Nombre}' registrado con éxito.");
                            }
                            break;

                        case "4": // Prestar Libro
                            ConsoleViews.Header("NUEVO PRÉSTAMO");

                            // 1. Seleccionar Usuario
                            var usuarioPrestar = UsuarioView.FormularioSeleccionUsuario(biblioteca.Usuarios);
                            if (usuarioPrestar == null) break;

                            Console.WriteLine();

                            // 2. Seleccionar Libro
                            var libroPrestar = LibroView.FormularioSeleccionLibro(biblioteca.Libros);
                            if (libroPrestar == null) break;

                            // 3. Procesar préstamo
                            var prestamo = biblioteca.PrestarLibro(libroPrestar.Id, usuarioPrestar.Id);
                            if (prestamo != null)
                            {
                                Console.WriteLine();
                                PrestamoView.ConfirmarPrestamo(prestamo);
                                ConsoleViews.MensajeExito("Préstamo realizado correctamente.");
                            }
                            else
                            {
                                ConsoleViews.MensajeError("No se pudo realizar el préstamo. Verifique disponibilidad.");
                            }
                            break;

                        case "5": // Devolver Libro
                            var prestamosActivos = biblioteca.ListarPrestamosActivos();
                            var idPrestamoDevolver = PrestamoView.FormularioDevolucion(prestamosActivos);

                            if (idPrestamoDevolver != null)
                            {
                                bool exito = biblioteca.DevolverLibro(idPrestamoDevolver.Value);
                                if (exito)
                                    ConsoleViews.MensajeExito("El libro ha sido devuelto correctamente.");
                                else
                                    ConsoleViews.MensajeError("Error al procesar la devolución.");
                            }
                            break;

                        case "6": // Detalle de Libro
                            var libroDetalle = LibroView.FormularioSeleccionLibro(biblioteca.Libros);
                            if (libroDetalle != null)
                            {
                                Console.Clear();
                                ConsoleViews.Banner();
                                LibroView.RenderizarDetalle(libroDetalle);
                            }
                            break;

                        case "7": // Salir
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
                    ConsoleViews.MensajeError($"Ocurrió un error inesperado: {ex.Message}");
                }

                if (!salir)
                {
                    ConsoleViews.EsperarTecla();
                }
            }
        }

        /// <summary>
        /// Carga de datos iniciales(2 Estudiantes, 1 Docente y 4 Libros).
        /// </summary>

        static void InicializarSeedData(Biblioteca biblioteca)
        {
            // Usuarios
            biblioteca.RegistrarUsuario(new Estudiante(
                nombre: "Maria Gonzalez Lopez",
                identificacion: "04567890-1",
                carne: "UDB-2022-001",
                carrera: "Ingenieria en Sistemas Informaticos"
            ));

            biblioteca.RegistrarUsuario(new Estudiante(
                nombre: "Carlos Martinez Rivas",
                identificacion: "07891234-5",
                carne: "UDB-2021-087",
                carrera: "Licenciatura en Administracion de Empresas"
            ));

            biblioteca.RegistrarUsuario(new Docente(
                nombre: "Dr. Roberto Mejia Fuentes",
                identificacion: "01234567-8",
                numeroEmpleado: "EMP-5023",
                departamento: "Departamento de Ingenieria Informatica"
            ));

            // Libros
            biblioteca.AgregarLibro(new LibroFisico(
                titulo: "Introduccion a los Algoritmos",
                autor: "Cormen, Leiserson, Rivest, Stein",
                isbn: "978-0-262-03384-8",
                anioPublicacion: 2009,
                genero: "Ciencias de la Computacion",
                stockTotal: 3,
                ubicacionEstante: "Estante A-12"
            ));

            biblioteca.AgregarLibro(new LibroFisico(
                titulo: "Clean Code",
                autor: "Robert C. Martin",
                isbn: "978-0-13-235088-4",
                anioPublicacion: 2008,
                genero: "Ingenieria de Software",
                stockTotal: 2,
                ubicacionEstante: "Estante B-07"
            ));

            biblioteca.AgregarLibro(new LibroFisico(
                titulo: "Designing Data-Intensive Applications",
                autor: "Martin Kleppmann",
                isbn: "978-1-491-90308-1",
                anioPublicacion: 2017,
                genero: "Bases de Datos",
                stockTotal: 4,
                ubicacionEstante: "Estante C-03"
            ));

            biblioteca.AgregarLibro(new LibroFisico(
                titulo: "Fundamentos de Bases de Datos",
                autor: "Silberschatz, Korth, Sudarshan",
                isbn: "978-0-07-352332-3",
                anioPublicacion: 2019,
                genero: "Bases de Datos",
                stockTotal: 5,
                ubicacionEstante: "Estante C-01"
            ));
        }
    }
}