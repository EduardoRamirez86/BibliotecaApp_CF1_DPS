using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BibliotecaApp.DTOs.Auth;
using BibliotecaApp.Interfaces;

namespace BibliotecaApp.Controllers
{
    /// <summary>
    /// Controlador Web API delgado para la consulta y registro de usuarios.
    /// Delega completamente en IUsuarioService e IAuthService.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosApiController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IAuthService _authService;

        public UsuariosApiController(IUsuarioService usuarioService, IAuthService authService)
        {
            _usuarioService = usuarioService;
            _authService = authService;
        }

        // GET: /api/UsuariosApi
        [HttpGet]
        public IActionResult ObtenerTodosLosUsuarios()
        {
            var usuarios = _usuarioService.ObtenerResumenTodos();
            return Ok(usuarios);
        }

        // POST: /api/UsuariosApi
        [HttpPost]
        public async Task<IActionResult> RegistrarUsuario([FromBody] RegisterDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resultado = await _authService.RegistrarUsuarioAsync(request);

            if (!resultado.Exitoso)
            {
                return Conflict(new { mensaje = resultado.Mensaje });
            }

            return Ok(new
            {
                mensaje = resultado.Mensaje,
                usuarioId = resultado.Usuario?.Id,
                rol = resultado.Usuario?.ObtenerRol()
            });
        }
    }
}