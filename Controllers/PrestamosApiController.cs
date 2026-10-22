using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using BibliotecaApp.DTOs;
using BibliotecaApp.DTOs.Prestamos;
using BibliotecaApp.Interfaces;

namespace BibliotecaApp.Controllers
{
    /// <summary>
    /// Controlador Web API delgado para el consumo REST de préstamos.
    /// Delega completamente en IPrestamoService sin acceder a la base de datos de forma directa.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PrestamosApiController : ControllerBase
    {
        private readonly IPrestamoService _prestamoService;

        public PrestamosApiController(IPrestamoService prestamoService)
        {
            _prestamoService = prestamoService;
        }

        // GET: /api/PrestamosApi
        [HttpGet]
        public IActionResult ObtenerTodosLosPrestamos()
        {
            var prestamos = _prestamoService.ObtenerResumenTodos();
            return Ok(prestamos);
        }

        // POST: /api/PrestamosApi
        [HttpPost]
        public IActionResult RegistrarPrestamo([FromBody] CrearPrestamoDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var nuevoPrestamo = _prestamoService.CrearPrestamoPorIdentificacion(
                    request.IdentificacionUsuario,
                    request.IsbnLibro,
                    request.DiasPrestamo
                );

                return Ok(new
                {
                    mensaje = "Préstamo registrado exitosamente y stock de inventario actualizado.",
                    prestamoId = nuevoPrestamo.Id,
                    fechaPrestamo = nuevoPrestamo.FechaPrestamo,
                    fechaDevolucionEsperada = nuevoPrestamo.FechaDevolucionEsperada
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
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

        // POST: /api/PrestamosApi/{id}/devolver
        [HttpPost("{id}/devolver")]
        public IActionResult RegistrarDevolucion(Guid id)
        {
            try
            {
                var prestamo = _prestamoService.RegistrarDevolucion(id);
                return Ok(new
                {
                    mensaje = "Devolución registrada exitosamente y stock de inventario restaurado.",
                    prestamoId = prestamo.Id,
                    fechaDevolucionReal = prestamo.FechaDevolucionReal
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}