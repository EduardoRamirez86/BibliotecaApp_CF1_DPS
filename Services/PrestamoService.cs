using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BibliotecaApp.Data;
using BibliotecaApp.DTOs.Prestamos;
using BibliotecaApp.Interfaces;
using BibliotecaApp.Models;
using BibliotecaApp.Interfaces;

namespace BibliotecaApp.Services;

/// <summary>
/// Implementación del servicio de préstamos. Centraliza la persistencia, deducción de existencias y reglas de integridad.
/// </summary>
public class PrestamoService : IPrestamoService
{
    private readonly BibliotecaDbContext _context;

    public PrestamoService(BibliotecaDbContext context)
    {
        _context = context;
    }

    public IReadOnlyList<Prestamo> ObtenerTodos()
        => _context.Prestamos
            .Include(p => p.Usuario)
            .Include(p => p.Libro)
            .AsNoTracking()
            .ToList()
            .AsReadOnly();

    public IReadOnlyList<PrestamoDetalleDto> ObtenerResumenTodos()
        => _context.Prestamos
            .Include(p => p.Usuario)
            .Include(p => p.Libro)
            .AsNoTracking()
            .Select(p => new PrestamoDetalleDto
            {
                Id = p.Id,
                FechaPrestamo = p.FechaPrestamo,
                FechaDevolucionEsperada = p.FechaDevolucionEsperada,
                FechaDevolucionReal = p.FechaDevolucionReal,
                EstaActivo = p.EstaActivo,
                EstaVencido = p.EstaVencido,
                UsuarioNombre = p.Usuario.Nombre,
                IdentificacionUsuario = p.Usuario.Identificacion,
                LibroTitulo = p.Libro.Titulo,
                IsbnLibro = p.Libro.ISBN
            })
            .ToList()
            .AsReadOnly();

    public IEnumerable<Prestamo> ObtenerActivos()
        => _context.Prestamos
            .Include(p => p.Usuario)
            .Include(p => p.Libro)
            .AsNoTracking()
            .Where(p => p.FechaDevolucionReal == null)
            .ToList();

    public IEnumerable<Prestamo> ObtenerPrestamosPorUsuario(Guid usuarioId)
        => _context.Prestamos
            .Include(p => p.Usuario)
            .Include(p => p.Libro)
            .AsNoTracking()
            .Where(p => p.Usuario.Id == usuarioId)
            .OrderByDescending(p => p.FechaPrestamo)
            .ToList();

    public IEnumerable<Prestamo> ObtenerPrestamosPorEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return Enumerable.Empty<Prestamo>();

        var emailNormalizado = email.Trim().ToLowerInvariant();
        return _context.Prestamos
            .Include(p => p.Usuario)
            .Include(p => p.Libro)
            .AsNoTracking()
            .Where(p => p.Usuario.Email == emailNormalizado)
            .OrderByDescending(p => p.FechaPrestamo)
            .ToList();
    }

    public Prestamo CrearPrestamoPorEmail(string email, Guid idLibro)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El correo del usuario no puede estar vacío.", nameof(email));

        var emailNormalizado = email.Trim().ToLowerInvariant();
        var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == emailNormalizado)
            ?? throw new KeyNotFoundException($"No se encontró ningún usuario con el correo '{email}'.");

        var libro = _context.LibrosFisicos.FirstOrDefault(l => l.Id == idLibro)
            ?? throw new KeyNotFoundException($"No se encontró ningún libro con el Id '{idLibro}'.");

        return CrearPrestamo(usuario, libro);
    }

    public Prestamo CrearPrestamoPorIdentificacion(string identificacionUsuario, string isbnLibro, int diasPrestamo = 15)
    {
        if (string.IsNullOrWhiteSpace(identificacionUsuario))
            throw new ArgumentException("La identificación del usuario es obligatoria.", nameof(identificacionUsuario));
        if (string.IsNullOrWhiteSpace(isbnLibro))
            throw new ArgumentException("El código ISBN del libro es obligatorio.", nameof(isbnLibro));

        var idNormalizada = identificacionUsuario.Trim();
        var isbnNormalizado = isbnLibro.Trim();

        var usuario = _context.Usuarios.FirstOrDefault(u => u.Identificacion == idNormalizada)
            ?? throw new KeyNotFoundException($"No se encontró ningún usuario con la identificación '{identificacionUsuario}'.");

        var libro = _context.LibrosFisicos.FirstOrDefault(l => l.ISBN == isbnNormalizado)
            ?? throw new KeyNotFoundException($"No se encontró ningún libro físico con el ISBN '{isbnLibro}'.");

        return CrearPrestamoInterno(usuario, libro, diasPrestamo);
    }

    public Prestamo CrearPrestamo(Usuario usuario, Libro libro)
    {
        return CrearPrestamoInterno(usuario, libro, diasPrestamo: 15);
    }

    private Prestamo CrearPrestamoInterno(Usuario usuario, Libro libro, int diasPrestamo)
    {
        ArgumentNullException.ThrowIfNull(usuario);
        ArgumentNullException.ThrowIfNull(libro);

        var yaLoPrestado = _context.Prestamos
            .Any(p => p.Usuario.Id == usuario.Id && p.Libro.Id == libro.Id && p.FechaDevolucionReal == null);

        if (yaLoPrestado)
            throw new InvalidOperationException($"'{usuario.Nombre}' ya tiene activo un préstamo del libro '{libro.Titulo}'.");

        if (libro is LibroFisico lf)
        {
            lf.RealizarPrestamo();
            _context.Entry(lf).State = EntityState.Modified;
        }
        else
        {
            throw new InvalidOperationException($"Tipo de libro no soportado para préstamo: '{libro.GetType().Name}'.");
        }

        var prestamo = new Prestamo(usuario, libro, diasPrestamo);
        _context.Prestamos.Add(prestamo);
        _context.SaveChanges();

        return prestamo;
    }

    public Prestamo RegistrarDevolucion(Guid prestamoId)
    {
        var prestamo = _context.Prestamos
            .Include(p => p.Libro)
            .FirstOrDefault(p => p.Id == prestamoId)
            ?? throw new KeyNotFoundException($"No se encontró ningún préstamo con Id '{prestamoId}'.");

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