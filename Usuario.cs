using System;

namespace GestionBiblioteca
{

    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }

        public Usuario(int id, string nombre, string email)
        {
            Id = id;
            Nombre = nombre;
            Email = email;
        }

        public override string ToString()
        {
            return $"#{Id} - {Nombre} ({Email})";
        }
    }
}
