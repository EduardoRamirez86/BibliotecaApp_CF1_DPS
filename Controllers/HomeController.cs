using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApp.Controllers;

[Authorize]
//Restringir el acceso a todas las acciones del controlador 
//unicamente a usuarios con na sesion de autenticacion activa
public class HomeController : Controller
{
    public IActionResult Index()  //este es el tipo de retorno 
    //abtracto IActionResult es un tipo de retorno MVC
    {
        ViewBag.NombreUsuario = User.Identity?.Name ?? "Usuario";
        ViewBag.RolUsuario = User.FindFirst(ClaimTypes.Role)?.Value ?? "Sin Rol";//el rol del usuario
        ViewBag.EmailUsuario = User.FindFirst(ClaimTypes.Email)?.Value ?? "";

        return View();
    }
}