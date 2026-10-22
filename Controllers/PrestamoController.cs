using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BibliotecaApp.Interfaces;

namespace BibliotecaApp.Controllers
{
    /// <summary>
    /// Controlador MVC delgado (Thin Controller) para la interacción del usuario con préstamos y devoluciones.
    /// Delega el 100% de la persistencia y reglas de negocio a IPrestamoService.
    /// </summary>
    [Authorize]
    public class PrestamoController : Controller
    {
        private readonly IPrestamoService _prestamoService;

        public PrestamoController(IPrestamoService prestamoService)
        {
            _prestamoService = prestamoService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Auth");
            }

            var prestamos = _prestamoService.ObtenerPrestamosPorEmail(email);
            return View(prestamos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Solicitar(Guid idLibro)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                _prestamoService.CrearPrestamoPorEmail(email, idLibro);
                TempData["Mensaje"] = "¡Préstamo registrado con éxito!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Detalle", "Libro", new { id = idLibro });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Devolver(Guid idPrestamo)
        {
            try
            {
                _prestamoService.RegistrarDevolucion(idPrestamo);
                TempData["Mensaje"] = "¡Libro devuelto con éxito! El inventario ha sido actualizado.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al devolver el libro: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}