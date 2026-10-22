using System;

namespace BibliotecaApp.DTOs.Libros;

/// <summary>
/// DTO con la información de disponibilidad de un ejemplar para clientes Web y API.
/// </summary>
public class DisponibilidadLibroDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public int StockTotal { get; set; }
    public int StockPrestado { get; set; }
    public int StockDisponible { get; set; }
    public bool Disponible { get; set; }
    public string Mensaje { get; set; } = string.Empty;
}
