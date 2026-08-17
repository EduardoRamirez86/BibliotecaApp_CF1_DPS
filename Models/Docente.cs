namespace BibliotecaApp.Models;

/// <summary>Docente de la UDB. Extiende Usuario con NumeroEmpleado y Departamento.</summary>
public sealed class Docente : Usuario
{
    public string NumeroEmpleado { get; private set; }
    public string Departamento { get; private set; }

    public Docente(string nombre, string identificacion, string numeroEmpleado, string departamento)
        : base(nombre, identificacion)
    {
        if (string.IsNullOrWhiteSpace(numeroEmpleado))
            throw new ArgumentException("El número de empleado no puede estar vacío.", nameof(numeroEmpleado));
        if (string.IsNullOrWhiteSpace(departamento))
            throw new ArgumentException("El departamento no puede estar vacío.", nameof(departamento));

        NumeroEmpleado = numeroEmpleado;
        Departamento = departamento;
    }

    public override string ObtenerRol() => "DOCENTE";
}
