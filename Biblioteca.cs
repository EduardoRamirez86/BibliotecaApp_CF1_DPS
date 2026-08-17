using System;
using System.Collections.Generic;
using System.Linq;

namespace GestionBiblioteca
{
    public class Biblioteca
    {
        public List<Libro> Libros { get; set; }
        public List<Usuario> Usuarios { get; set; }
        public List<Prestamo> Prestamos { get; set; }

        private int siguienteIdLibro = 1;
        private int siguienteIdUsuario = 1;
        private int siguienteIdPrestamo = 1;

        public Biblioteca()
        {
            Libros = new List<Libro>();
            Usuarios = new List<Usuario>();
            Prestamos = new List<Prestamo>();
        }

        public Libro AgregarLibro(string titulo, string autor, string isbn, int cantidad)
        {
            Libro libro = new Libro(siguienteIdLibro++, titulo, autor, isbn, cantidad);
            Libros.Add(libro);
            return libro;
        }

        public Usuario RegistrarUsuario(string nombre, string email)
        {
            Usuario usuario = new Usuario(siguienteIdUsuario++, nombre, email);
            Usuarios.Add(usuario);
            return usuario;
        }

        public Prestamo PrestarLibro(int idLibro, int idUsuario)
        {
            Libro libro = Libros.FirstOrDefault(l => l.Id == idLibro);
            Usuario usuario = Usuarios.FirstOrDefault(u => u.Id == idUsuario);

            if (libro == null || usuario == null || !libro.HayDisponibilidad())
                return null;

            Prestamo prestamo = new Prestamo(siguienteIdPrestamo++, libro, usuario);
            Prestamos.Add(prestamo);
            libro.CantidadDisponible--;

            return prestamo;
        }

        public bool DevolverLibro(int idPrestamo)
        {
            Prestamo prestamo = Prestamos.FirstOrDefault(p => p.Id == idPrestamo && !p.Devuelto);

            if (prestamo == null)
                return false;

            prestamo.MarcarDevuelto();
            prestamo.Libro.CantidadDisponible++;

            return true;
        }

        public List<Prestamo> ListarPrestamosActivos()
        {
            return Prestamos.Where(p => !p.Devuelto).ToList();
        }
    }
}
