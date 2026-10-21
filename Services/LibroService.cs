using BibliotecaApp.Interfaces;
using BibliotecaApp.Models;
using BibliotecaApp.Data;

namespace BibliotecaApp.Services;

public class LibroService : ILibroService
{
    private readonly BibliotecaDbContext _context;

    // Inyectamos la base de datos en lugar de usar una lista en memoria
    public LibroService(BibliotecaDbContext context)
    {
        _context = context;
    }

    public IReadOnlyList<Libro> ObtenerTodos() => _context.Libros.ToList().AsReadOnly();

    public void Agregar(Libro libro)
    {
        ArgumentNullException.ThrowIfNull(libro);

        if (_context.Libros.Any(l => l.ISBN == libro.ISBN))
            throw new InvalidOperationException($"Ya existe un libro con ISBN '{libro.ISBN}' en el catálogo.");

        _context.Libros.Add(libro);
        _context.SaveChanges();
    }

    public IEnumerable<Libro> BuscarPorTitulo(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo)) return ObtenerTodos();

        return _context.Libros
            .Where(l => l.Titulo.Contains(titulo))
            .ToList();
    }

    public IEnumerable<Libro> BuscarPorTexto(string termino)
    {
        if (string.IsNullOrWhiteSpace(termino)) return ObtenerTodos();

        return _context.Libros
            .Where(l => l.Titulo.Contains(termino) ||
                        l.Autor.Contains(termino) ||
                        l.ISBN.Contains(termino) ||
                        l.Genero.Contains(termino))
            .ToList();
    }

    public Libro? ObtenerPorId(Guid id) => _context.Libros.FirstOrDefault(l => l.Id == id);
}