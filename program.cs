using System;

namespace GestionBiblioteca
{
    class Program
    {
        static void Main(string[] args)
        {
            Biblioteca biblioteca = new Biblioteca();

            Libro libro1 = biblioteca.AgregarLibro("Cien años de soledad", "Gabriel García Márquez", "978-0307474728", 2);
            Usuario usuario1 = biblioteca.RegistrarUsuario("Ana Torres", "ana.torres@correo.com");

            Console.WriteLine("Libro agregado: " + libro1);
            Console.WriteLine("Usuario registrado: " + usuario1);

            var prestamo = biblioteca.PrestarLibro(libro1.Id, usuario1.Id);
            if (prestamo != null)
                Console.WriteLine("Préstamo creado: " + prestamo);
            else
                Console.WriteLine("Error: no se pudo crear el préstamo.");

            Console.WriteLine("\nPréstamos activos:");
            foreach (var p in biblioteca.ListarPrestamosActivos())
                Console.WriteLine(p);

            bool devuelto = biblioteca.DevolverLibro(prestamo.Id);
            Console.WriteLine("\n¿Devolución exitosa? " + devuelto);
            Console.WriteLine("Libro tras devolución: " + libro1);
        }
    }
}
