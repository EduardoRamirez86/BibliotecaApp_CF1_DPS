using System;

namespace BibliotecaApp.DTOs
{
    public class CrearPrestamoDto
    {
        public string IdentificacionUsuario { get; set; } = string.Empty;
        public string IsbnLibro { get; set; } = string.Empty;
        public int DiasPrestamo { get; set; } = 15;
    }
}