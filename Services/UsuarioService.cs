using BibliotecaApp.Interfaces;
using BibliotecaApp.Models;

namespace BibliotecaApp.Services;

/// <summary>
/// Gestiona el registro y consulta de usuarios en memoria.
/// Implementa IUsuarioService e IBuscable&lt;Usuario&gt;.
/// </summary>
public class UsuarioService : IUsuarioService
{
    private readonly List<Usuario> _usuarios = new();

    public IReadOnlyList<Usuario> ObtenerTodos() => _usuarios.AsReadOnly();

    public void Registrar(Usuario usuario)
    {
        ArgumentNullException.ThrowIfNull(usuario);

        if (ExisteIdentificacion(usuario.Identificacion))
            throw new InvalidOperationException(
                $"Ya existe un usuario con la identificación '{usuario.Identificacion}'.");

        _usuarios.Add(usuario);
    }

    public bool ExisteIdentificacion(string identificacion)
        => _usuarios.Any(u =>
            u.Identificacion.Equals(identificacion, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<Usuario> BuscarPorTexto(string termino)
    {
        if (string.IsNullOrWhiteSpace(termino)) return _usuarios;

        return _usuarios.Where(u =>
        {
            var coincideBase =
                u.Nombre.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
                u.Identificacion.Contains(termino, StringComparison.OrdinalIgnoreCase);

            var coincideDetalle = u switch
            {
                Estudiante e =>
                    e.Carne.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
                    e.Carrera.Contains(termino, StringComparison.OrdinalIgnoreCase),
                Docente d =>
                    d.NumeroEmpleado.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
                    d.Departamento.Contains(termino, StringComparison.OrdinalIgnoreCase),
                _ => false
            };

            return coincideBase || coincideDetalle;
        });
    }

    public Usuario? ObtenerPorId(Guid id) => _usuarios.FirstOrDefault(u => u.Id == id);
}
