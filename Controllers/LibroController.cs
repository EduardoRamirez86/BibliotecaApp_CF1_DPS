using BibliotecaApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BibliotecaApp.Controllers
{
    [Authorize]
    public class LibroController : Controller
    {
        private readonly LibroService _libroService;

        public LibroController(LibroService libroService)
        {
            _libroService = libroService;
        }

        public IActionResult Index()
        {
            var libros = _libroService.ObtenerTodos();
            return View(libros);
        }
        public IActionResult Detalle(Guid id)
        {
            var libro = _libroService.ObtenerPorId(id);
            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }
    }
}