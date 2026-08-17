namespace BibliotecaApp.Interfaces;

/// <summary>
/// Interfaz que define el contrato de búsqueda genérica por texto.
/// Principio SOLID: Segregación de Interfaces (ISP) — define un contrato mínimo y enfocado.
/// Cualquier servicio que implemente esta interfaz puede ser buscado desde la UI
/// de forma polimórfica, sin conocer su implementación interna.
/// </summary>
public interface IBuscable<T>
{
    /// <summary>
    /// Busca elementos cuyo título, nombre u otro campo de texto contenga el término dado.
    /// </summary>
    /// <param name="termino">Texto a buscar (case-insensitive).</param>
    /// <returns>Colección de resultados coincidentes.</returns>
    IEnumerable<T> BuscarPorTexto(string termino);

    /// <summary>
    /// Obtiene un elemento por su identificador único.
    /// </summary>
    /// <param name="id">GUID del elemento.</param>
    /// <returns>El elemento encontrado, o null si no existe.</returns>
    T? ObtenerPorId(Guid id);
}
