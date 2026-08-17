using BibliotecaApp.Models;

namespace BibliotecaApp.Interfaces;

/// <summary>
/// Contrato del servicio de gestión de libros.
/// Principio SOLID: Inversión de Dependencias (DIP) — la UI depende de esta abstracción,
/// no de la implementación concreta LibroService.
/// Esto facilita la migración a Fase 2 donde se puede inyectar un repositorio EF Core
/// sin cambiar ninguna línea de código en Program.cs o las vistas.
/// </summary>
public interface ILibroService : IBuscable<Libro>
{
    /// <summary>Devuelve todos los libros registrados en el sistema.</summary>
    IReadOnlyList<Libro> ObtenerTodos();

    /// <summary>Agrega un nuevo libro al catálogo.</summary>
    void Agregar(Libro libro);

    /// <summary>Busca libros cuyo título contenga el texto dado.</summary>
    IEnumerable<Libro> BuscarPorTitulo(string titulo);
}
