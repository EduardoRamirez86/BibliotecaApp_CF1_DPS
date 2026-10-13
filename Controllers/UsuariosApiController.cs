using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using BibliotecaApp.Data;
using BibliotecaApp.Models;
using BibliotecaApp.DTOs.Auth;
using BibliotecaApp.Interfaces;

namespace BibliotecaApp.Controllers
{
    // ApiController sirve para indicar que el controlador es una API y este va a devolver un json
    [ApiController]
    // Se definer la ruta de la API, la cual es /api/UsuariosApi
    [Route("api/[controller]")]
    public class UsuariosApiController : ControllerBase
    {
        private readonly BibliotecaDbContext _context;
        private readonly IAuthService _authService;

        // Se le inyecta el DbContext para poder acceder a la base de datos
        public UsuariosApiController(BibliotecaDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        // La ruta del endpoint (GET): /api/UsuariosApi
        [HttpGet]
        public async Task<IActionResult> ObtenerTodosLosUsuarios()
        {
            var usuarios = await _context.Usuarios.ToListAsync();

            var resultado = usuarios.Select(u => new
            {
                id = u.Id,
                nombre = u.Nombre,
                identificacion = u.Identificacion,
                email = u.Email,
                tipoUsuario = u.GetType().Name 
            });

            return Ok(resultado);
        }

        // La ruta del endpoint (POST): /api/UsuariosApi
        [HttpPost]
        public async Task<IActionResult> RegistrarUsuario([FromBody] RegisterDto request)
        {
            // Se debe generar el hash de la contraseña de forma segura
            var passwordHash = _authService.HashearPassword(request.Password);

            Usuario nuevoUsuario;

            // Se utiliza el Enum establecido en RolUsuario (Enums/RolUsuario)
            switch (request.Rol)
            {
                case Enums.RolUsuario.Estudiante:
                    // Se envia el estado "Pendiente" a los datos que el DTO no incluye por ahora
                    nuevoUsuario = new Estudiante(
                        request.NombreCompleto, "Pendiente", request.Email, passwordHash, "Pendiente", "Pendiente");
                    break;

                case Enums.RolUsuario.Docente:
                    nuevoUsuario = new Docente(
                        request.NombreCompleto, "Pendiente", request.Email, passwordHash, "Pendiente", "Pendiente");
                    break;

                case Enums.RolUsuario.Bibliotecario:
                    nuevoUsuario = new Bibliotecario(
                        request.NombreCompleto, "Pendiente", request.Email, passwordHash, "Pendiente");
                    break;

                default:
                    return BadRequest(new { mensaje = "Rol no válido." });
            }

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = $"Usuario registrado exitosamente como {nuevoUsuario.GetType().Name}.",
                usuarioId = nuevoUsuario.Id
            });
        }
    }
}