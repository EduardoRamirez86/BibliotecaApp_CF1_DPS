using System;

namespace GestionBiblioteca
{

    public class Libro
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public string ISBN { get; set; }
        public int CantidadTotal { get; set; }
        public int CantidadDisponible { get; set; }

        public Libro(int id, string titulo, string autor, string isbn, int cantidadTotal)
        {
            Id = id;
            Titulo = titulo;
            Autor = autor;
            ISBN = isbn;
            CantidadTotal = cantidadTotal;
            CantidadDisponible = cantidadTotal;
        }

        public bool HayDisponibilidad()
        {
            return CantidadDisponible > 0;
        }

        public override string ToString()
        {
            return $"#{Id} - \"{Titulo}\" de {Autor} (ISBN: {ISBN}) | Disponibles: {CantidadDisponible}/{CantidadTotal}";
        }
    }
}
