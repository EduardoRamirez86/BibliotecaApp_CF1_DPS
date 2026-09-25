using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Esto le dice a MVC que busque un archivo HTML (Vista) llamado Index.cshtml
            return View();
        }
    }
}