using Microsoft.AspNetCore.Mvc;
using NicoPasino.Core.DTO.Movies;
using NicoPasino.Core.Interfaces;

namespace NicoPasino.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IMovieServicio _servicio;
        public MoviesController(IMovieServicio servicio) {
            _servicio = servicio;
        }

        [HttpGet]
        public async Task<IActionResult> Index() {
            var all = await _servicio.GetAll();
            return View(all);
        }

        [HttpGet]
        public async Task<IActionResult> MovieInfo(int id) {
            ViewBag.generos = await _servicio.GetGenres();
            var movie = await _servicio.GetById(id);
            if (movie != null) {
                return View(movie);
            }
            else {
                TempData["Msg"] = "No se encontró la Pelicula";
                TempData["Tipo"] = "warning";
                return RedirectToAction(nameof(Index));
            }
        }



        [HttpGet] // Vista CREAR
        public async Task<IActionResult> Crear() {
            ViewBag.generos = await _servicio.GetGenres();
            return View();
        }
        [HttpPost] // API
        public async Task<IActionResult> Crear(MovieDto objeto) {
            ViewBag.generos = await _servicio.GetGenres();
            if (!ModelState.IsValid) {
                return View(objeto);
            }

            var ok = await _servicio.Create(objeto);
            if (!ok) {
                TempData["Msg"] = "No se pudo crear la Pelicula";
                TempData["Tipo"] = "danger";
                ViewBag.generos = await _servicio.GetGenres();
                return RedirectToAction(nameof(MovieInfo), objeto.idPublica);
            }
            else {
                TempData["Msg"] = "✅Pelicula creada";
                TempData["Tipo"] = "success";
                return RedirectToAction(nameof(Index));
            }
        }



        [HttpGet] // Vista MODIFICAR
        public async Task<IActionResult> Modificar(int id) {
            ViewBag.generos = await _servicio.GetGenres();
            var movie = await _servicio.GetById(id);
            if (movie == null) {
                TempData["Msg"] = "No se encontró la Pelicula";
                TempData["Tipo"] = "warning";
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }
        [HttpPost] // API
        public async Task<IActionResult> Modificar(MovieDto objeto) {
            ViewBag.generos = await _servicio.GetGenres();
            if (!ModelState.IsValid) {
                return View(objeto);
            }
            var ok = await _servicio.Update(objeto);
            if (ok) {
                TempData["Msg"] = "✅Pelicula actualizada";
                TempData["Tipo"] = "success";
                return RedirectToAction(nameof(Index));
            }
            else {
                TempData["Msg"] = "No se pudo actualizar la Pelicula";
                TempData["Tipo"] = "danger";
                return View(objeto);
            }
        }



        [HttpGet]
        public async Task<IActionResult> Eliminar(int id) {
            var ok = await _servicio.Delete(id);
            if (ok) {
                TempData["Msg"] = "✅Publicacion eliminada";
                TempData["Tipo"] = "success";
            }
            else {
                TempData["Msg"] = "No se pudo eliminar la publicacion";
                TempData["Tipo"] = "danger";
            }
            return RedirectToAction(nameof(Index));
        }



        /*[HttpGet] // Vista - API
        public async Task<IActionResult> Buscar(string title) {
            var movie = await _servicio.(title);
            return View(movie);
        }*/

        /*[HttpGet] // Vista - API
        public async Task<IActionResult> Genero() {
            var all = await _servicio.GetByGenre();
            return View(all);
        }*/




    }
}
