using System;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaApp.DTOs.Libros;

/// <summary>
/// DTO para la creación y edición de libros físicos en el catálogo.
/// </summary>
public class LibroFormDto
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "El título del libro es obligatorio.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "El título debe tener entre 2 y 150 caracteres.")]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El autor es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El autor debe tener entre 2 y 100 caracteres.")]
    [Display(Name = "Autor")]
    public string Autor { get; set; } = string.Empty;

    [Required(ErrorMessage = "El código ISBN es obligatorio.")]
    [StringLength(25, MinimumLength = 5, ErrorMessage = "El ISBN debe tener entre 5 y 25 caracteres.")]
    [Display(Name = "Código ISBN")]
    public string ISBN { get; set; } = string.Empty;

    [Required(ErrorMessage = "El año de publicación es obligatorio.")]
    [Range(1000, 2100, ErrorMessage = "Ingrese un año de publicación válido.")]
    [Display(Name = "Año de Publicación")]
    public int AnioPublicacion { get; set; } = DateTime.Now.Year;

    [Required(ErrorMessage = "El género literario es obligatorio.")]
    [StringLength(50, ErrorMessage = "El género no puede superar los 50 caracteres.")]
    [Display(Name = "Género Literario")]
    public string Genero { get; set; } = string.Empty;

    [Required(ErrorMessage = "El stock inicial es obligatorio.")]
    [Range(1, 500, ErrorMessage = "El stock total debe ser al menos 1 unidad y máximo 500.")]
    [Display(Name = "Stock Total")]
    public int StockTotal { get; set; } = 1;

    [Required(ErrorMessage = "La ubicación en estante es obligatoria.")]
    [StringLength(50, ErrorMessage = "La ubicación no puede superar los 50 caracteres.")]
    [Display(Name = "Ubicación en Estante")]
    public string UbicacionEstante { get; set; } = string.Empty;
}
