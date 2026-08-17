using System;

namespace GestionBiblioteca
{

    public class Prestamo
    {
        public int Id { get; set; }
        public Libro Libro { get; set; }
        public Usuario Usuario { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaDevolucionEsperada { get; set; }
        public DateTime? FechaDevolucionReal { get; set; }
        public bool Devuelto { get; set; }

        public Prestamo(int id, Libro libro, Usuario usuario, int diasPlazo = 7)
        {
            Id = id;
            Libro = libro;
            Usuario = usuario;
            FechaPrestamo = DateTime.Now;
            FechaDevolucionEsperada = FechaPrestamo.AddDays(diasPlazo);
            Devuelto = false;
        }

        public void MarcarDevuelto()
        {
            Devuelto = true;
            FechaDevolucionReal = DateTime.Now;
        }

        public bool EstaAtrasado()
        {
            if (Devuelto) return false;
            return DateTime.Now > FechaDevolucionEsperada;
        }

        public override string ToString()
        {
            string estado = Devuelto ? "Devuelto" : (EstaAtrasado() ? "Atrasado" : "En curso");
            return $"#{Id} - \"{Libro.Titulo}\" prestado a {Usuario.Nombre} | Vence: {FechaDevolucionEsperada:dd/MM/yyyy} | Estado: {estado}";
        }
    }
}
