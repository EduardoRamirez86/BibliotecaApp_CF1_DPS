using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using BibliotecaApp.Models;

namespace BibliotecaApp.Data
{
    public class BibliotecaDbContext : DbContext
    {
        public BibliotecaDbContext(DbContextOptions<BibliotecaDbContext> options)
            : base(options)
        {
        }

        // Propiedades que se convertirán en las tablas de la DB
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<Docente> Docentes { get; set; }
        public DbSet<Libro> Libros { get; set; }
        public DbSet<LibroFisico> LibrosFisicos { get; set; }
        public DbSet<Prestamo> Prestamos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aquí configuramos la herencia para que Estudiantes y Docentes convivan en la misma tabla "Usuarios" pero se diferencien por una columna.
            modelBuilder.Entity<Usuario>()
                .HasDiscriminator<string>("TipoUsuario")
                .HasValue<Estudiante>("Estudiante")
                .HasValue<Docente>("Docente");

            modelBuilder.Entity<Libro>()
                .HasDiscriminator<string>("TipoLibro")
                .HasValue<LibroFisico>("Fisico");
        }
    }
}