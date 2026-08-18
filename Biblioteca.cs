using System;
using System.Collections.Generic;
using System.Linq;
using BibliotecaApp.Models;

namespace BibliotecaApp
{
    public class Biblioteca
    {
        public List<Libro> Libros { get; set; }
        public List<Usuario> Usuarios { get; set; }
        public List<Prestamo> Prestamos { get; set; }

        public Biblioteca()
        {
            Libros = new List<Libro>();
            Usuarios = new List<Usuario>();
            Prestamos = new List<Prestamo>();
        }

        public void AgregarLibro(Libro libro)
        {
            if (libro != null)
                Libros.Add(libro);
        }

        public void RegistrarUsuario(Usuario usuario)
        {
            if (usuario != null)
                Usuarios.Add(usuario);
        }

        public Prestamo PrestarLibro(Guid idLibro, Guid idUsuario)
        {
            Libro libro = Libros.FirstOrDefault(l => l.Id == idLibro);
            Usuario usuario = Usuarios.FirstOrDefault(u => u.Id == idUsuario);

            if (libro == null || usuario == null || !libro.HayDisponibilidad())
                return null;

            if (libro is LibroFisico libroFisico)
            {
                libroFisico.RealizarPrestamo();
            }

            Prestamo prestamo = new Prestamo(usuario, libro);
            Prestamos.Add(prestamo);

            return prestamo;
        }

        public bool DevolverLibro(Guid idPrestamo)
        {
            Prestamo prestamo = Prestamos.FirstOrDefault(p => p.EstaActivo && p.Id == idPrestamo);

            if (prestamo == null)
                return false;

            prestamo.RegistrarDevolucion();

            if (prestamo.Libro is LibroFisico libroFisico)
            {
                libroFisico.RegistrarDevolucion();
            }

            return true;
        }

        public List<Prestamo> ListarPrestamosActivos()
        {
            return Prestamos.Where(p => p.EstaActivo).ToList();
        }
    }
}