using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BibliotecaApp.DTOs.Libros;
using BibliotecaApp.Interfaces;
using BibliotecaApp.Models;

namespace BibliotecaApp.Controllers
{
    /// <summary>
    /// Controlador MVC delgado para vistas de catálogo y administración de libros.
    /// Toda la lógica de negocio y persistencia está encapsulada en ILibroService.
    /// </summary>
    [Authorize]
    public class LibroController : Controller
    {
        private readonly ILibroService _libroService;//inyecta mediante dependencias por constructor

        public LibroController(ILibroService libroService)
        {
            _libroService = libroService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var libros = _libroService.ObtenerTodos();
            return View(libros);
        }

        [HttpGet]
        public IActionResult Detalle(Guid id)
        {
            var libro = _libroService.ObtenerPorId(id);
            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        
        // MÓDULO DE ADMINISTRACIÓN (SOLO BIBLIOTECARIO)
       

        [HttpGet]
        [Authorize(Roles = "BIBLIOTECARIO,Bibliotecario")]
        //atributo de seguridad para restringir las acciones Crear Editar y Elimineitor
        //exclusivo del perfil bibliotecario
        public IActionResult Crear()
        {
            return View(new LibroFormDto());
        }

        [HttpPost]
        [Authorize(Roles = "BIBLIOTECARIO,Bibliotecario")]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(LibroFormDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var libro = _libroService.Crear(model);
                TempData["Mensaje"] = $"¡Libro '{libro.Titulo}' registrado exitosamente en el catálogo!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "BIBLIOTECARIO,Bibliotecario")]
        public IActionResult Editar(Guid id)
        {
            var libro = _libroService.ObtenerPorId(id);
            if (libro == null)
            {
                return NotFound();
            }

            if (libro is not LibroFisico lf)
            {
                TempData["Error"] = "Solo se pueden editar ejemplares físicos.";
                return RedirectToAction(nameof(Index));
            }

            var dto = new LibroFormDto
            {
                Id = lf.Id,
                Titulo = lf.Titulo,
                Autor = lf.Autor,
                ISBN = lf.ISBN,
                AnioPublicacion = lf.AnioPublicacion,
                Genero = lf.Genero,
                StockTotal = lf.StockTotal,
                UbicacionEstante = lf.UbicacionEstante
            };

            return View(dto);
        }

        [HttpPost]
        [Authorize(Roles = "BIBLIOTECARIO,Bibliotecario")]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Guid id, LibroFormDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                _libroService.Actualizar(id, model);
                TempData["Mensaje"] = $"¡Libro '{model.Titulo}' modificado exitosamente!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [Authorize(Roles = "BIBLIOTECARIO,Bibliotecario")]
        //deniega el acceso y el middleware de autenticación lo redirige la ruta especificada
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(Guid id)
        {
            try
            {
                _libroService.Eliminar(id);
                TempData["Mensaje"] = "¡El libro ha sido eliminado del catálogo!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "No se pudo eliminar el libro: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
            //ESTA ONDA REDIRIGE AL NAVEGADOR WEB REALIZANDO UNA NUEVA
            //PETICION HTTP HACIA EL  INDEX DEL CONTROLADOR casacontroler xd
        }
    }
}