using System;

namespace BibliotecaApp.DTOs
{
    public class CrearPrestamoDto
    {
        public string IdentificacionUsuario { get; set; }
        public string IsbnLibro { get; set; }
        public int DiasPrestamo { get; set; } = 15;
    }
}