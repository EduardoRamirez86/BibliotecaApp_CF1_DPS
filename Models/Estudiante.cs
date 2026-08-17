namespace BibliotecaApp.Models;

/// <summary>Estudiante de la UDB. Extiende Usuario con Carné y Carrera.</summary>
public sealed class Estudiante : Usuario
{
    public string Carne { get; private set; }
    public string Carrera { get; private set; }

    public Estudiante(string nombre, string identificacion, string carne, string carrera)
        : base(nombre, identificacion)
    {
        if (string.IsNullOrWhiteSpace(carne))
            throw new ArgumentException("El carné no puede estar vacío.", nameof(carne));
        if (string.IsNullOrWhiteSpace(carrera))
            throw new ArgumentException("La carrera no puede estar vacía.", nameof(carrera));

        Carne = carne;
        Carrera = carrera;
    }

    public override string ObtenerRol() => "ESTUDIANTE";
}
