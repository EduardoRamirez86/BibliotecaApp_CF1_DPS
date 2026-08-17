namespace BibliotecaApp.Models;

/// <summary>
/// Clase abstracta base para todas las entidades del dominio.
/// Genera automáticamente el Id (GUID) y la FechaCreacion al construirse.
/// </summary>
public abstract class EntidadBase
{
    public Guid Id { get; init; }
    public DateTime FechaCreacion { get; init; }

    protected EntidadBase()
    {
        Id = Guid.NewGuid();
        FechaCreacion = DateTime.Now;
    }
}
