using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BibliotecaApp.Data; // Referencia al DbContext del proyecto (base de datos)
using BibliotecaApp.Models; // Referencia a los modelos del proyecto

namespace BibliotecaApp.Controllers
{
    // ApiController sirve para indicar que el controlador es una API y este va a devolver un json
    [ApiController]
    // Se definer la ruta de la API, la cual es /api/LibrosApi
    [Route("api/[controller]")]
    // Clase publica que hereda de ControllerBase
    public class LibrosApiController : ControllerBase
    {
        private readonly BibliotecaDbContext _context;

        // Se le inyecta el DbContext para poder acceder a la base de datos
        public LibrosApiController(BibliotecaDbContext context)
        {
            _context = context;
        }


        // ENDPOINT PARA VERIFICAR LA DISPONIBILIDAD DE UN LIBRO POR SU ID (ISBN)

        // La ruta del endpoint (GET) /api/LibrosApi/{isbn}/disponibilidad
        [HttpGet("{isbn}/disponibilidad")]
        public async Task<IActionResult> VerificarDisponibilidad(string isbn)
        {
            // Se busca el libro en la base de datos por su id (ISBN)
            var libro = await _context.LibrosFisicos.FirstOrDefaultAsync(l => l.ISBN == isbn);
            // En caso que no exista un libro, se devuelve un error (Error 404 Not Found) en formato json
            if (libro == null)
            {
                return NotFound(new { mensaje = $"No se encontró ningún libro con ese ISBN {isbn}" });
            }

            // Si el stock total es mayor a 0, entonces se marca como disponible
            bool estaDisponible = libro.StockTotal > 0;

            // Se retorna un objeto del tipo anonimo, ya que asp.net se encarga de transformarlo de manera automatica a json (gracias a Ok(),
            // ya que ASP.NET Core activa automaticamente su motor interno para convertir objetos a texto)
            return Ok(new
            {
                /* Un objeto de tipo anonimo es crear un objeto sin tener que crear un archivo con una clase nueva que
            solo se use una vez Su estructura es new {}  sin poner el nombre de ninguna clase */
                id = libro.Id,
                titulo = libro.Titulo,
                isbn = libro.ISBN,
                stockDisponible = libro.StockTotal,
                disponible = estaDisponible,
                mensaje = estaDisponible ? "El libro se encuentra disponible para prestamos." : "Lo sentimos, el libro no se encuentra disponible en este momento."
            });
        }


        // ENDPOINT PARA VERIFICAR TODO EL CATALOGO DE LIBROS

        // La ruta del endpoint (GET): /api/LibrosApi
        [HttpGet]
        public async Task<IActionResult> ObtenerCatalogo()
        {
            // Se obtienen todos los registros de la tabla LibrosFisicos, utilizando Entity Framework
            var libros = await _context.LibrosFisicos.ToListAsync();
            // Si no hay errores, se muestra la lista completa en formato json, con un codigo 200 (success)
            return Ok(libros);
        }



        // ENDPOINT PARA REGISTRAR (INSERTAR) UN NUEVO LIBRO

        // La ruta del endpoint (POST): /api/LibrosApi
        [HttpPost]
        public async Task<IActionResult> RegistrarLibro([FromBody] LibroFisico libro)
        {
            // [FromBody] sirve para indicar que los datos del libro vienen en el cuerpo de la petición HTTP
            _context.LibrosFisicos.Add(libro);
            // Se realiza el insert en la base de datos
            await _context.SaveChangesAsync();

            // Si no hay errores, entonces se devuelve un codigo 201 (created) mostrando la nueva ruta para poder consultar el registro del libro recien creado
            return CreatedAtAction(nameof(VerificarDisponibilidad), new { isbn = libro.ISBN }, libro);
        }

    }
}