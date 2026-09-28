using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApp.Controllers;

[Authorize]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        ViewBag.NombreUsuario = User.Identity?.Name ?? "Usuario";
        ViewBag.RolUsuario = User.FindFirst(ClaimTypes.Role)?.Value ?? "Sin Rol";
        ViewBag.EmailUsuario = User.FindFirst(ClaimTypes.Email)?.Value ?? "";

        return View();
    }
}