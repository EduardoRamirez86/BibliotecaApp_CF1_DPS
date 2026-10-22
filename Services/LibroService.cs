using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BibliotecaApp.Data;
using BibliotecaApp.DTOs.Libros;
using BibliotecaApp.Interfaces;
using BibliotecaApp.Models;

namespace BibliotecaApp.Services;

/// <summary>
/// Servicio de catálogo de libros. Encapsula todas las operaciones con la base de datos y reglas de negocio.
/// </summary>
public class LibroService : ILibroService
{
    private readonly BibliotecaDbContext _context;

    public LibroService(BibliotecaDbContext context)
    {
        _context = context;
    }

    public IReadOnlyList<Libro> ObtenerTodos() => _context.Libros.AsNoTracking().ToList().AsReadOnly();

    public Libro? ObtenerPorId(Guid id) => _context.Libros.FirstOrDefault(l => l.Id == id);

    public Libro? ObtenerPorIsbn(string isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn)) return null;
        var isbnLimpio = isbn.Trim();
        return _context.Libros.FirstOrDefault(l => l.ISBN == isbnLimpio);
    }

    public DisponibilidadLibroDto VerificarDisponibilidad(string isbn)
    {
        var libro = ObtenerPorIsbn(isbn);
        if (libro == null)
        {
            throw new KeyNotFoundException($"No se encontró ningún libro con el ISBN '{isbn}'.");
        }

        if (libro is LibroFisico lf)
        {
            bool estaDisponible = lf.HayDisponibilidad();
            return new DisponibilidadLibroDto
            {
                Id = lf.Id,
                Titulo = lf.Titulo,
                ISBN = lf.ISBN,
                StockTotal = lf.StockTotal,
                StockPrestado = lf.StockPrestado,
                StockDisponible = lf.StockDisponible,
                Disponible = estaDisponible,
                Mensaje = estaDisponible
                    ? "El libro se encuentra disponible para préstamo."
                    : "Lo sentimos, todos los ejemplares están actualmente prestados."
            };
        }

        return new DisponibilidadLibroDto
        {
            Id = libro.Id,
            Titulo = libro.Titulo,
            ISBN = libro.ISBN,
            StockTotal = 1,
            StockPrestado = 0,
            StockDisponible = 1,
            Disponible = true,
            Mensaje = "Libro digital disponible para lectura inmediata."
        };
    }

    public Libro Crear(LibroFormDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var nuevoLibro = new LibroFisico(
            dto.Titulo.Trim(),
            dto.Autor.Trim(),
            dto.ISBN.Trim(),
            dto.AnioPublicacion,
            dto.Genero.Trim(),
            dto.StockTotal,
            dto.UbicacionEstante.Trim()
        );

        Agregar(nuevoLibro);
        return nuevoLibro;
    }

    public void Actualizar(Guid id, LibroFormDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var existente = _context.Libros.FirstOrDefault(l => l.Id == id)
            ?? throw new KeyNotFoundException($"No se encontró ningún libro con ID '{id}'.");

        var isbnTrimmed = dto.ISBN.Trim();
        if (_context.Libros.Any(l => l.ISBN == isbnTrimmed && l.Id != id))
            throw new InvalidOperationException($"Ya existe otro libro con el ISBN '{dto.ISBN}'.");

        if (existente is LibroFisico lfExistente)
        {
            lfExistente.ActualizarDatos(
                dto.Titulo.Trim(),
                dto.Autor.Trim(),
                isbnTrimmed,
                dto.AnioPublicacion,
                dto.Genero.Trim(),
                dto.StockTotal,
                dto.UbicacionEstante.Trim()
            );
            _context.Entry(lfExistente).State = EntityState.Modified;
            _context.SaveChanges();
        }
        else
        {
            throw new NotSupportedException($"Tipo de libro no soportado para actualización: '{existente.GetType().Name}'.");
        }
    }

    public void Agregar(Libro libro)
    {
        ArgumentNullException.ThrowIfNull(libro);

        if (_context.Libros.Any(l => l.ISBN == libro.ISBN))
            throw new InvalidOperationException($"Ya existe un libro con ISBN '{libro.ISBN}' en el catálogo.");

        _context.Libros.Add(libro);
        _context.SaveChanges();
    }

    public void Actualizar(Libro libro)
    {
        ArgumentNullException.ThrowIfNull(libro);

        var existente = _context.Libros.FirstOrDefault(l => l.Id == libro.Id)
            ?? throw new KeyNotFoundException($"No se encontró ningún libro con ID '{libro.Id}'.");

        if (_context.Libros.Any(l => l.ISBN == libro.ISBN && l.Id != libro.Id))
            throw new InvalidOperationException($"Ya existe otro libro con el ISBN '{libro.ISBN}'.");

        if (existente is LibroFisico lfExistente && libro is LibroFisico lfNuevo)
        {
            lfExistente.ActualizarDatos(
                lfNuevo.Titulo,
                lfNuevo.Autor,
                lfNuevo.ISBN,
                lfNuevo.AnioPublicacion,
                lfNuevo.Genero,
                lfNuevo.StockTotal,
                lfNuevo.UbicacionEstante
            );
            _context.Entry(lfExistente).State = EntityState.Modified;
        }
        else
        {
            throw new NotSupportedException($"Tipo de libro no soportado para actualización: '{libro.GetType().Name}'.");
        }

        _context.SaveChanges();
    }

    public void Eliminar(Guid id)
    {
        var libro = _context.Libros.FirstOrDefault(l => l.Id == id)
            ?? throw new KeyNotFoundException($"No se encontró ningún libro con ID '{id}'.");

        var tienePrestamosActivos = _context.Prestamos
            .Any(p => p.Libro.Id == id && p.FechaDevolucionReal == null);

        if (tienePrestamosActivos)
            throw new InvalidOperationException(
                $"No se puede eliminar el libro '{libro.Titulo}' porque cuenta con préstamos activos pendientes de devolución.");

        _context.Libros.Remove(libro);
        _context.SaveChanges();
    }

    public IEnumerable<Libro> BuscarPorTitulo(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo)) return ObtenerTodos();

        return _context.Libros
            .AsNoTracking()
            .Where(l => l.Titulo.Contains(titulo))
            .ToList();
    }

    public IEnumerable<Libro> BuscarPorTexto(string termino)
    {
        if (string.IsNullOrWhiteSpace(termino)) return ObtenerTodos();

        return _context.Libros
            .AsNoTracking()
            .Where(l => l.Titulo.Contains(termino) ||
                        l.Autor.Contains(termino) ||
                        l.ISBN.Contains(termino) ||
                        l.Genero.Contains(termino))
            .ToList();
    }
}