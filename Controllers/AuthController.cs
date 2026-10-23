using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BibliotecaApp.DTOs.Auth;
using BibliotecaApp.Interfaces;

namespace BibliotecaApp.Controllers;

/// <summary>
/// Controlador MVC para el flujo de autenticación, registro y cierre de sesión.
/// </summary>
public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger; 

    //estos gestionan la autenticacion  yel registro de usuarios

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)//exrae el name del user autenticado
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginDto());
    }

    [HttpPost]
    [AllowAnonymous]
    //
    [ValidateAntiForgeryToken]
    //este valida osea previene ataques de falsificación de solicitudes entre sitios (CSRF)
    //verificando el token de seguridad eviado en el formulario
    public async Task<IActionResult> Login(LoginDto model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        //propiedad nos permite verificar si el estado de
        //las validaciones definidas en las anotaciones de un
        //DTO fue satisfactorio
        {
            return View(model);
        }

        var resultado = await _authService.IniciarSesionAsync(model, HttpContext);

        if (!resultado.Exitoso)
        {
            ModelState.AddModelError(string.Empty, resultado.Mensaje);
            return View(model);
        }

        _logger.LogInformation("Usuario autenticado correctamente: {Email}", model.Email);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        //validacion de seguridad para evitar redirecciones
        //maliciosas a sitios externos
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [AllowAnonymous]
    //permite que invitados puedan acceder a la vista de registro
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new RegisterDto());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var resultado = await _authService.RegistrarUsuarioAsync(model);

        if (!resultado.Exitoso)
        {
            ModelState.AddModelError(string.Empty, resultado.Mensaje);
            return View(model);
        }

        TempData["SuccessMessage"] = "¡Cuenta creada exitosamente! Por favor inicia sesión.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _authService.CerrarSesionAsync(HttpContext);
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
