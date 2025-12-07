using Microsoft.AspNetCore.Mvc;
using NicoPasino.Models;
using System.Diagnostics;

namespace NicoPasino.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index() {
            return View();
        }

        // Manejo de Error GLOBAL para TODOS los controladores.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string? mensaje, string? controlador) {
            /*_logger.LogError(ex, logErrorMensaje);*/ // TODO:
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Controlador = controlador, Mensaje = mensaje });
        }

        [HttpGet("Home/NotFound")]
        public IActionResult NotFound(int statusCode) { // , string? mensaje, string? controlador
            ViewData["StatusCode"] = statusCode;
            ViewData["Title"] = "Página No Encontrada";
            //ViewData["Controlador"] = controlador;
            //ViewData["Mensaje"] = mensaje;

            return View();
        }
    }
}
