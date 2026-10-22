using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BibliotecaApp.Interfaces;
using BibliotecaApp.Data;
using System;
using System.Linq;

namespace BibliotecaApp.Controllers
{
    [Authorize]
    public class PrestamoController : Controller
    {
        private readonly IPrestamoService _prestamoService;
        private readonly BibliotecaDbContext _context;

        public PrestamoController(IPrestamoService prestamoService, BibliotecaDbContext context)
        {
            _prestamoService = prestamoService;
            _context = context;
        }

        private Models.Usuario? ObtenerUsuarioAutenticado()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? User.Identity?.Name;
            return _context.Usuarios.FirstOrDefault(u => u.Email == email!);
        }

        public IActionResult Index()
        {
            var usuario = ObtenerUsuarioAutenticado();

            // Si por alguna razón la sesión es inválida, lo mandamos al login
            if (usuario == null) return RedirectToAction("Login", "Auth");

            var prestamos = _prestamoService.ObtenerPrestamosPorUsuario(usuario.Id);
            return View(prestamos);
        }

        [HttpPost]
        public IActionResult Solicitar(Guid idLibro)
        {
            var usuario = ObtenerUsuarioAutenticado();
            var libro = _context.LibrosFisicos.FirstOrDefault(l => l.Id == idLibro);

            if (usuario == null || libro == null)
            {
                TempData["Error"] = "No se pudo procesar la solicitud. Verifique su sesión y la disponibilidad del libro.";
                return RedirectToAction("Detalle", "Libro", new { id = idLibro });
            }

            try
            {
                _prestamoService.CrearPrestamo(usuario, libro);
                TempData["Mensaje"] = "¡Préstamo registrado con éxito!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Detalle", "Libro", new { id = idLibro });
        }

        [HttpPost]
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

            return RedirectToAction("Index");
        }
    }
}