using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using BibliotecaApp.Data;
using BibliotecaApp.Models;
using BibliotecaApp.DTOs;

namespace BibliotecaApp.Controllers
{
    // ApiController sirve para indicar que el controlador es una API y este va a devolver un json
    [ApiController]
    // Se definer la ruta de la API, la cual es /api/PrestamosApi
    [Route("api/[controller]")]
    public class PrestamosApiController : ControllerBase
    {
        private readonly BibliotecaDbContext _context;

        // Se le inyecta el DbContext para poder acceder a la base de datos
        public PrestamosApiController(BibliotecaDbContext context)
        {
            _context = context;
        }

        // La ruta del endpoint (GET): /api/PrestamosApi
        [HttpGet]
        public async Task<IActionResult> ObtenerTodosLosPrestamos()
        {
            // Se usa Include() para hacer el JOIN con Usuarios y Libros
            // y se usa Select() para asi poder crear un objeto del tipo anonimo con solo los datos seguros
            var prestamos = await _context.Prestamos
                .Include(p => p.Usuario)
                .Include(p => p.Libro)
                .Select(p => new
                {
                    id = p.Id,
                    fechaPrestamo = p.FechaPrestamo,
                    fechaDevolucionEsperada = p.FechaDevolucionEsperada,
                    fechaDevolucionReal = p.FechaDevolucionReal,
                    estaActivo = p.EstaActivo,
                    estaVencido = p.EstaVencido,
                    // Se extrae solamente el nombre del usuario y el titulo del libro, evitando asi enviar
                    // todos los datos del objeto ( datos no necesarios, como la contraseña)
                    usuarioNombre = p.Usuario.Nombre,
                    libroTitulo = p.Libro.Titulo
                })
                .ToListAsync();

            return Ok(prestamos);
        }

        // La ruta del endpoint (POST): /api/PrestamosApi
        [HttpPost]
        public async Task<IActionResult> RegistrarPrestamo([FromBody] CrearPrestamoDto request)
        {

            // Se buscan los datos utilizando FirstOrDefaultAsync para comparar los strings
            // la propiedad en la clase Usuario debe llamarse "Identificacion"
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Identificacion == request.IdentificacionUsuario);
            var libro = await _context.LibrosFisicos.FirstOrDefaultAsync(l => l.ISBN == request.IsbnLibro);

            // Si alguno no existe, se devuelve un error (400)
            if (usuario == null) return BadRequest("El usuario especificado no existe.");
            if (libro == null) return BadRequest("El libro especificado no existe.");

            // Se hace uso del constructor del modelo de Prestamos (Models/Prestamo.cs) para seguir la logica establecida en este constructor
            var nuevoPrestamo = new Prestamo(usuario, libro, request.DiasPrestamo);

            // Se guarda el registro en la base de datos
            _context.Prestamos.Add(nuevoPrestamo);
            await _context.SaveChangesAsync();

            // Se retorna un objeto del tipo anonimo, ya que asp.net se encarga de transformarlo de manera automatica a json (gracias a Ok(),
            // ya que ASP.NET Core activa automaticamente su motor interno para convertir objetos a texto)
            return Ok(new
            {
                /* Un objeto de tipo anonimo es crear un objeto sin tener que crear un archivo con una clase nueva que
            solo se use una vez Su estructura es new {}  sin poner el nombre de ninguna clase */
                mensaje = "Préstamo registrado exitosamente.",
                prestamoId = nuevoPrestamo.Id,
                fechaDevolucion = nuevoPrestamo.FechaDevolucionEsperada
            });
        }
    }
}