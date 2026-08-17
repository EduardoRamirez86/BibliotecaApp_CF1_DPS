using BibliotecaApp.Interfaces;
using BibliotecaApp.Models;

namespace BibliotecaApp.Services;

/// <summary>
/// Gestiona el catálogo de libros en memoria.
/// Implementa ILibroService e IBuscable&lt;Libro&gt;.
/// </summary>
public class LibroService : ILibroService
{
    private readonly List<Libro> _libros = new();

    public IReadOnlyList<Libro> ObtenerTodos() => _libros.AsReadOnly();

    public void Agregar(Libro libro)
    {
        ArgumentNullException.ThrowIfNull(libro);

        if (_libros.Any(l => l.ISBN == libro.ISBN))
            throw new InvalidOperationException(
                $"Ya existe un libro con ISBN '{libro.ISBN}' en el catálogo.");

        _libros.Add(libro);
    }

    public IEnumerable<Libro> BuscarPorTitulo(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo)) return _libros;

        return _libros.Where(l =>
            l.Titulo.Contains(titulo, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<Libro> BuscarPorTexto(string termino)
    {
        if (string.IsNullOrWhiteSpace(termino)) return _libros;

        return _libros.Where(l =>
            l.Titulo.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
            l.Autor.Contains(termino, StringComparison.OrdinalIgnoreCase)  ||
            l.ISBN.Contains(termino, StringComparison.OrdinalIgnoreCase)   ||
            l.Genero.Contains(termino, StringComparison.OrdinalIgnoreCase));
    }

    public Libro? ObtenerPorId(Guid id) => _libros.FirstOrDefault(l => l.Id == id);
}
