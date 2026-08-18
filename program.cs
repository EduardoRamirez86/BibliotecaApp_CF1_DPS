// BibliotecaApp — Fase 1: Aplicacion de Consola | UDB — DPS
// Proyecto de Catedra: Sistema de Gestion de Biblioteca
// Arquitectura: POO + SOLID + Almacenamiento en memoria + UI Modular

using BibliotecaApp.Interfaces;
using BibliotecaApp.Models;
using BibliotecaApp.Services;
using BibliotecaApp.UI;

// --- Servicios ---
// Declarados con la interfaz para facilitar la inyeccion de dependencias en Fase 2.
ILibroService    libroService    = new LibroService();
IUsuarioService  usuarioService  = new UsuarioService();
IPrestamoService prestamoService = new PrestamoService();

// --- Seed Data ---
InicializarSeedData(libroService, usuarioService);

// --- Bucle principal ---
var ejecutando = true;
while (ejecutando)
{
    ConsoleViews.Banner();
    ConsoleViews.MenuPrincipal();

    var opcionRaw = Console.ReadLine()?.Trim() ?? "0";

    var accion = opcionRaw switch
    {
        "1" => (Action)MostrarCatalogo,
        "2" => MostrarUsuarios,
        "3" => RegistrarUsuario,
        "4" => PrestarLibro,
        "5" => DevolverLibro,
        "6" => DetalleLibro,
        "7" => Salir,
        _   => OpcionInvalida
    };

    accion.Invoke();
}

// ============================================================
//  HANDLERS
// ============================================================

/// <summary>Opcion 1: muestra el catalogo completo.</summary>
void MostrarCatalogo()
{
    try
    {
        LibroView.RenderizarCatalogo(libroService.ObtenerTodos());
    }
    catch (Exception ex)
    {
        ConsoleViews.MensajeError($"Error al cargar el catalogo: {ex.Message}");
    }
    finally { ConsoleViews.EsperarTecla(); }
}

/// <summary>Opcion 2: lista todos los usuarios.</summary>
void MostrarUsuarios()
{
    try
    {
        UsuarioView.RenderizarLista(usuarioService.ObtenerTodos());
    }
    catch (Exception ex)
    {
        ConsoleViews.MensajeError($"Error al cargar usuarios: {ex.Message}");
    }
    finally { ConsoleViews.EsperarTecla(); }
}

/// <summary>Opcion 3: formulario para registrar un nuevo usuario.</summary>
void RegistrarUsuario()
{
    try
    {
        var nuevoUsuario = UsuarioView.FormularioRegistro();

        if (nuevoUsuario is null)
        {
            ConsoleViews.MensajeAdvertencia("Operacion cancelada.");
            return;
        }

        usuarioService.Registrar(nuevoUsuario);
        ConsoleViews.MensajeExito($"'{nuevoUsuario.Nombre}' registrado como {nuevoUsuario.ObtenerRol()}.");
    }
    catch (InvalidOperationException ex) { ConsoleViews.MensajeError(ex.Message); }
    catch (ArgumentException ex)         { ConsoleViews.MensajeError($"Dato invalido: {ex.Message}"); }
    catch (Exception ex)                 { ConsoleViews.MensajeError($"Error inesperado: {ex.Message}"); }
    finally { ConsoleViews.EsperarTecla(); }
}

/// <summary>Opcion 4: selecciona usuario y libro para crear un prestamo.</summary>
void PrestarLibro()
{
    try
    {
        var usuarioSeleccionado = UsuarioView.FormularioSeleccionUsuario(usuarioService.ObtenerTodos());
        if (usuarioSeleccionado is null) return;

        var libroSeleccionado = LibroView.FormularioSeleccionLibro(libroService.ObtenerTodos());
        if (libroSeleccionado is null) return;

        var prestamo = prestamoService.CrearPrestamo(usuarioSeleccionado, libroSeleccionado);
        PrestamoView.ConfirmarPrestamo(prestamo);
        ConsoleViews.MensajeExito("Prestamo registrado exitosamente.");
    }
    catch (InvalidOperationException ex) { ConsoleViews.MensajeError(ex.Message); }
    catch (ArgumentNullException ex)     { ConsoleViews.MensajeError($"Argumento nulo: {ex.ParamName}"); }
    catch (Exception ex)                 { ConsoleViews.MensajeError($"Error inesperado: {ex.Message}"); }
    finally { ConsoleViews.EsperarTecla(); }
}

/// <summary>Opcion 5: selecciona un prestamo activo y lo marca como devuelto.</summary>
void DevolverLibro()
{
    try
    {
        var prestamoId = PrestamoView.FormularioDevolucion(prestamoService.ObtenerActivos());
        if (prestamoId is null) return;

        var prestamo = prestamoService.RegistrarDevolucion(prestamoId.Value);
        ConsoleViews.MensajeExito($"'{prestamo.Libro.Titulo}' devuelto por {prestamo.Usuario.Nombre}.");
    }
    catch (KeyNotFoundException ex)      { ConsoleViews.MensajeError($"Prestamo no encontrado: {ex.Message}"); }
    catch (InvalidOperationException ex) { ConsoleViews.MensajeError(ex.Message); }
    catch (Exception ex)                 { ConsoleViews.MensajeError($"Error inesperado: {ex.Message}"); }
    finally { ConsoleViews.EsperarTecla(); }
}

/// <summary>Opcion 6: busca un libro por texto y muestra su ficha completa.</summary>
void DetalleLibro()
{
    try
    {
        ConsoleViews.Header("BUSCAR LIBRO");

        var termino = ConsoleViews.LeerTexto("Titulo o parte del titulo");
        if (string.IsNullOrWhiteSpace(termino))
        {
            ConsoleViews.MensajeAdvertencia("Ingrese un termino de busqueda.");
            return;
        }

        var resultados = libroService.BuscarPorTexto(termino).ToList();

        if (resultados.Count == 0)
        {
            ConsoleViews.MensajeAdvertencia($"No se encontraron libros con '{termino}'.");
            return;
        }

        if (resultados.Count > 1)
        {
            ConsoleViews.MensajeAdvertencia($"Se encontraron {resultados.Count} resultado(s). Seleccione uno:");
            var libroSeleccionado = LibroView.FormularioSeleccionLibro(resultados);
            if (libroSeleccionado is null) return;
            LibroView.RenderizarDetalle(libroSeleccionado);
        }
        else
        {
            LibroView.RenderizarDetalle(resultados[0]);
        }
    }
    catch (Exception ex) { ConsoleViews.MensajeError($"Error al buscar: {ex.Message}"); }
    finally { ConsoleViews.EsperarTecla(); }
}

/// <summary>Opcion 7: termina la ejecucion.</summary>
void Salir()
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("\n  Hasta pronto.");
    Console.ResetColor();
    ejecutando = false;
}

void OpcionInvalida()
{
    ConsoleViews.MensajeError("Opcion no valida. Ingrese un numero del 1 al 7.");
    ConsoleViews.EsperarTecla();
}

// ============================================================
//  SEED DATA
//  En Fase 2 se sustituye por migraciones de EF Core (HasData).
// ============================================================

/// <summary>Carga 2 estudiantes UDB, 1 docente y 4 libros fisicos.</summary>
void InicializarSeedData(ILibroService libros, IUsuarioService usuarios)
{
    usuarios.Registrar(new Estudiante(
        nombre:         "Maria Gonzalez Lopez",
        identificacion: "04567890-1",
        carne:          "UDB-2022-001",
        carrera:        "Ingenieria en Sistemas Informaticos"
    ));

    usuarios.Registrar(new Estudiante(
        nombre:         "Carlos Martinez Rivas",
        identificacion: "07891234-5",
        carne:          "UDB-2021-087",
        carrera:        "Licenciatura en Administracion de Empresas"
    ));

    usuarios.Registrar(new Docente(
        nombre:          "Dr. Roberto Mejia Fuentes",
        identificacion:  "01234567-8",
        numeroEmpleado:  "EMP-5023",
        departamento:    "Departamento de Ingenieria Informatica"
    ));

    libros.Agregar(new LibroFisico(
        titulo:           "Introduccion a los Algoritmos",
        autor:            "Cormen, Leiserson, Rivest, Stein",
        isbn:             "978-0-262-03384-8",
        anioPublicacion:  2009,
        genero:           "Ciencias de la Computacion",
        stockTotal:       3,
        ubicacionEstante: "Estante A-12"
    ));

    libros.Agregar(new LibroFisico(
        titulo:           "Clean Code",
        autor:            "Robert C. Martin",
        isbn:             "978-0-13-235088-4",
        anioPublicacion:  2008,
        genero:           "Ingenieria de Software",
        stockTotal:       2,
        ubicacionEstante: "Estante B-07"
    ));

    libros.Agregar(new LibroFisico(
        titulo:           "Designing Data-Intensive Applications",
        autor:            "Martin Kleppmann",
        isbn:             "978-1-491-90308-1",
        anioPublicacion:  2017,
        genero:           "Bases de Datos",
        stockTotal:       4,
        ubicacionEstante: "Estante C-03"
    ));

    libros.Agregar(new LibroFisico(
        titulo:           "Fundamentos de Bases de Datos",
        autor:            "Silberschatz, Korth, Sudarshan",
        isbn:             "978-0-07-352332-3",
        anioPublicacion:  2019,
        genero:           "Bases de Datos",
        stockTotal:       5,
        ubicacionEstante: "Estante C-01"
    ));
}
