using System;
using System.Collections.Generic;
using BibliotecaApp.DTOs.Libros;
using BibliotecaApp.Models;

namespace BibliotecaApp.Interfaces;

/// <summary>
/// Contrato del servicio de gestión de libros y catálogo.
/// Encapsula la lógica de negocio, validaciones de stock y transformaciones de DTOs.
/// </summary>
public interface ILibroService : IBuscable<Libro>
{
    /// <summary>Devuelve todos los libros registrados en el sistema.</summary>
    IReadOnlyList<Libro> ObtenerTodos();

    /// <summary>Busca un libro por su código ISBN único.</summary>
    Libro? ObtenerPorIsbn(string isbn);

    /// <summary>Verifica la disponibilidad de ejemplares y retorna un DTO descriptivo.</summary>
    DisponibilidadLibroDto VerificarDisponibilidad(string isbn);

    /// <summary>Crea un nuevo libro físico a partir de un DTO de formulario.</summary>
    Libro Crear(LibroFormDto dto);

    /// <summary>Actualiza los datos de un libro existente a partir de un DTO.</summary>
    void Actualizar(Guid id, LibroFormDto dto);

    /// <summary>Agrega una entidad de libro al catálogo con validación de ISBN.</summary>
    void Agregar(Libro libro);

    /// <summary>Actualiza una entidad de libro existente.</summary>
    void Actualizar(Libro libro);

    /// <summary>Elimina un libro del catálogo si no tiene préstamos activos pendientes.</summary>
    void Eliminar(Guid id);

    /// <summary>Busca libros cuyo título contenga el texto dado.</summary>
    IEnumerable<Libro> BuscarPorTitulo(string titulo);
}
