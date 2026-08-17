using BibliotecaApp.Models;

namespace BibliotecaApp.Interfaces;

/// <summary>
/// Contrato del servicio de gestión de usuarios.
/// Principio SOLID: DIP — la UI programa contra esta interfaz, no la clase concreta.
/// </summary>
public interface IUsuarioService : IBuscable<Usuario>
{
    /// <summary>Devuelve todos los usuarios registrados.</summary>
    IReadOnlyList<Usuario> ObtenerTodos();

    /// <summary>Registra un nuevo usuario en el sistema.</summary>
    void Registrar(Usuario usuario);

    /// <summary>Verifica si ya existe un usuario con la misma identificación.</summary>
    bool ExisteIdentificacion(string identificacion);
}
