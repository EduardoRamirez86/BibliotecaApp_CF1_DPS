using System;

namespace BibliotecaApp.Models;

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