namespace BibliotecaApp.Models;

/// <summary>
/// Clase abstracta que representa cualquier tipo de libro.
/// HayDisponibilidad() es abstracto porque cada formato define su propia regla.
/// </summary>
public abstract class Libro : EntidadBase
{
    public string Titulo { get; protected set; }
    public string Autor { get; protected set; }
    public string ISBN { get; protected set; }
    public int AnioPublicacion { get; protected set; }
    public string Genero { get; protected set; }

    protected Libro(string titulo, string autor, string isbn, int anioPublicacion, string genero)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("El título no puede estar vacío.", nameof(titulo));
        if (string.IsNullOrWhiteSpace(autor))
            throw new ArgumentException("El autor no puede estar vacío.", nameof(autor));
        if (string.IsNullOrWhiteSpace(isbn))
            throw new ArgumentException("El ISBN no puede estar vacío.", nameof(isbn));
        if (anioPublicacion < 1000 || anioPublicacion > DateTime.Now.Year)
            throw new ArgumentOutOfRangeException(nameof(anioPublicacion), "Año de publicación no válido.");

        Titulo = titulo;
        Autor = autor;
        ISBN = isbn;
        AnioPublicacion = anioPublicacion;
        Genero = string.IsNullOrWhiteSpace(genero) ? "Sin clasificar" : genero;
    }

    public abstract bool HayDisponibilidad();
    public abstract string ObtenerTipo();

    public override string ToString() => $"[{ObtenerTipo()}] {Titulo} - {Autor} ({AnioPublicacion})";
}
