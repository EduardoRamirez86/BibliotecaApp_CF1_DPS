using System;
using Microsoft.AspNetCore.Mvc;
using BibliotecaApp.DTOs.Libros;
using BibliotecaApp.Interfaces;

namespace BibliotecaApp.Controllers
{
    /// <summary>
    /// Controlador Web API delgado para la gestión RESTful de libros.
    /// Delega el 100% de la lógica y persistencia a ILibroService sin inyectar DbContext.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LibrosApiController : ControllerBase
    {
        private readonly ILibroService _libroService;

        public LibrosApiController(ILibroService libroService)
        {
            _libroService = libroService;
        }

        // GET: /api/LibrosApi/{isbn}/disponibilidad
        [HttpGet("{isbn}/disponibilidad")]
        public IActionResult VerificarDisponibilidad(string isbn)
        {
            try
            {
                var disponibilidad = _libroService.VerificarDisponibilidad(isbn);
                return Ok(disponibilidad);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }

        // GET: /api/LibrosApi
        [HttpGet]
        public IActionResult ObtenerCatalogo()
        {
            var libros = _libroService.ObtenerTodos();
            return Ok(libros);
        }

        // POST: /api/LibrosApi
        [HttpPost]
        public IActionResult RegistrarLibro([FromBody] LibroFormDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var nuevoLibro = _libroService.Crear(dto);

                return CreatedAtAction(
                        
                    nameof(VerificarDisponibilidad),
                    new { isbn = nuevoLibro.ISBN },
                    nuevoLibro
                );
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // PUT: /api/LibrosApi/{id}
        [HttpPut("{id}")]
        public IActionResult ActualizarLibro(Guid id, [FromBody] LibroFormDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _libroService.Actualizar(id, dto);
                return Ok(new { mensaje = "Libro actualizado exitosamente.", id = id });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // DELETE: /api/LibrosApi/{id}
        [HttpDelete("{id}")]
        public IActionResult EliminarLibro(Guid id)
        {
            try
            {
                _libroService.Eliminar(id);
                return Ok(new { mensaje = "Libro eliminado del catálogo exitosamente." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno al eliminar el libro: " + ex.Message });
            }
        }
    }
}