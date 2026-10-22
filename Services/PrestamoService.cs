using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BibliotecaApp.Data;
using BibliotecaApp.Models;
using BibliotecaApp.Interfaces;

namespace BibliotecaApp.Services;

public class PrestamoService : IPrestamoService
{
    private readonly BibliotecaDbContext _context;

    public PrestamoService(BibliotecaDbContext context)
    {
        _context = context;
    }

    public IReadOnlyList<Prestamo> ObtenerTodos()
        => _context.Prestamos.Include(p => p.Usuario).Include(p => p.Libro).ToList().AsReadOnly();

    public IEnumerable<Prestamo> ObtenerActivos()
        => _context.Prestamos.Include(p => p.Usuario).Include(p => p.Libro)
            .AsEnumerable()
            .Where(p => p.EstaActivo)
            .ToList();

    public IEnumerable<Prestamo> ObtenerPrestamosPorUsuario(Guid usuarioId)
        => _context.Prestamos.Include(p => p.Usuario).Include(p => p.Libro).Where(p => p.Usuario.Id == usuarioId).ToList();

    public Prestamo CrearPrestamo(Usuario usuario, Libro libro)
    {
        ArgumentNullException.ThrowIfNull(usuario);
        ArgumentNullException.ThrowIfNull(libro);

        var yaLoPrestado = _context.Prestamos
            .Where(p => p.Usuario.Id == usuario.Id && p.Libro.Id == libro.Id)
            .AsEnumerable()
            .Any(p => p.EstaActivo);

        if (yaLoPrestado)
            throw new InvalidOperationException($"'{usuario.Nombre}' ya tiene activo un préstamo de '{libro.Titulo}'.");

        if (libro is LibroFisico lf)
        {
            lf.RealizarPrestamo();
            _context.Entry(lf).State = EntityState.Modified;
        }
        else
        {
            throw new InvalidOperationException($"Tipo de libro no soportado: '{libro.GetType().Name}'.");
        }

        var prestamo = new Prestamo(usuario, libro);
        _context.Prestamos.Add(prestamo);
        _context.SaveChanges();

        return prestamo;
    }

    public Prestamo RegistrarDevolucion(Guid prestamoId)
    {
        var prestamo = _context.Prestamos
            .Include(p => p.Libro)
            .FirstOrDefault(p => p.Id == prestamoId)
            ?? throw new KeyNotFoundException($"No se encontró el préstamo con Id '{prestamoId}'.");

        prestamo.RegistrarDevolucion();
        _context.Prestamos.Update(prestamo);

        if (prestamo.Libro is LibroFisico lf)
        {
            lf.RegistrarDevolucion();
            _context.Entry(lf).State = EntityState.Modified;
        }

        _context.SaveChanges();

        return prestamo;
    }
}