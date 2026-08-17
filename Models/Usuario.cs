namespace BibliotecaApp.Models;

/// <summary>
/// Clase abstracta que representa a cualquier usuario del sistema.
/// ObtenerRol() es abstracto: cada subclase define su propio rol.
/// </summary>
public abstract class Usuario : EntidadBase
{
    public string Nombre { get; protected set; }
    public string Identificacion { get; protected set; }

    protected Usuario(string nombre, string identificacion)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre no puede estar vacío.", nameof(nombre));
        if (string.IsNullOrWhiteSpace(identificacion))
            throw new ArgumentException("La identificación no puede estar vacía.", nameof(identificacion));

        Nombre = nombre;
        Identificacion = identificacion;
    }

    public abstract string ObtenerRol();

    public override string ToString() => $"[{ObtenerRol()}] {Nombre} (ID: {Identificacion})";
}
