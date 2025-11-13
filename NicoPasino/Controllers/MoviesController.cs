using Microsoft.AspNetCore.Mvc;
using NicoPasino.Core.DTO.Movies;
using NicoPasino.Core.Interfaces;

namespace NicoPasino.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IMovieServicio _servicio;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(IMovieServicio servicio, ILogger<MoviesController> logger) {
            _servicio = servicio;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index() {
            try {
                var generos = await _servicio.GetGenres();
                ViewBag.generos = generos ?? Enumerable.Empty<object>();

                var all = await _servicio.GetAll();
                return View(all ?? Enumerable.Empty<MovieDto>());
            } catch (Exception ex) {
                _logger.LogError(ex, "Error en MoviesController.Index");
                TempData["Msg"] = "Ocurrió un error al cargar las películas.";
                TempData["Tipo"] = "danger";
                return View(Enumerable.Empty<MovieDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> MovieInfo(int id) {
            try {
                var generos = await _servicio.GetGenres();
                ViewBag.generos = generos ?? Enumerable.Empty<object>();

                var movie = await _servicio.GetById(id);
                if (movie != null) {
                    return View(movie);
                }

                TempData["Msg"] = "No se encontró la película.";
                TempData["Tipo"] = "warning";
                return RedirectToAction(nameof(Index));
            } catch (Exception ex) {
                _logger.LogError(ex, "Error en MoviesController.MovieInfo id={Id}", id);
                TempData["Msg"] = "Ocurrió un error al obtener la información de la película.";
                TempData["Tipo"] = "danger";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Buscar(string? titulo = "", int? idGenero = null) {
            try {
                var generos = await _servicio.GetGenres();
                ViewBag.generos = generos ?? Enumerable.Empty<object>();

                // Normalizar inputs básicos
                titulo = titulo?.Trim();
                if (!string.IsNullOrEmpty(titulo) && titulo.Length > 200) titulo = titulo[..200];
                if (idGenero.HasValue && idGenero.Value <= 0) idGenero = null;

                var movies = await _servicio.GetAll(titulo, idGenero);
                if (movies == null || !movies.Any()) {
                    TempData["Msg"] = "No se encontró ninguna película con los criterios indicados.";
                    TempData["Tipo"] = "info";
                    return View("Index", Enumerable.Empty<MovieDto>());
                }

                return View("Index", movies);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error en MoviesController.Buscar titulo={Titulo} idGenero={IdGenero}", titulo, idGenero);
                TempData["Msg"] = "Ocurrió un error al buscar películas.";
                TempData["Tipo"] = "danger";
                return View("Index", Enumerable.Empty<MovieDto>());
            }
        }

        [HttpGet] // Vista CREAR
        public async Task<IActionResult> Crear() {
            try {
                var generos = await _servicio.GetGenres();
                ViewBag.generos = generos ?? Enumerable.Empty<object>();
                return View();
            } catch (Exception ex) {
                _logger.LogError(ex, "Error en MoviesController.Crear GET");
                TempData["Msg"] = "Ocurrió un error al preparar la vista de creación.";
                TempData["Tipo"] = "danger";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost] // API
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(MovieDto objeto) {
            try {
                var generos = await _servicio.GetGenres();
                ViewBag.generos = generos ?? Enumerable.Empty<object>();

                if (!ModelState.IsValid) {
                    return View(objeto);
                }

                var ok = await _servicio.Create(objeto);
                if (!ok) {
                    _logger.LogWarning("No se pudo crear la película (Create returned false). Title={Title}", objeto?.title);
                    TempData["Msg"] = "No se pudo crear la película.";
                    TempData["Tipo"] = "danger";
                    return View(objeto);
                }

                TempData["Msg"] = "✅ Película creada";
                TempData["Tipo"] = "success";
                return RedirectToAction(nameof(Index));
            } catch (Exception ex) {
                _logger.LogError(ex, "Error en MoviesController.Crear POST");
                TempData["Msg"] = "Ocurrió un error al crear la película.";
                TempData["Tipo"] = "danger";
                return View(objeto);
            }
        }

        [HttpGet] // Vista MODIFICAR
        public async Task<IActionResult> Modificar(int id) {
            try {
                var generos = await _servicio.GetGenres();
                ViewBag.generos = generos ?? Enumerable.Empty<object>();

                var movie = await _servicio.GetById(id);
                if (movie == null) {
                    TempData["Msg"] = "No se encontró la película.";
                    TempData["Tipo"] = "warning";
                    return RedirectToAction(nameof(Index));
                }

                return View(movie);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error en MoviesController.Modificar GET id={Id}", id);
                TempData["Msg"] = "Ocurrió un error al preparar la edición.";
                TempData["Tipo"] = "danger";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost] // API
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modificar(MovieDto objeto) {
            try {
                var generos = await _servicio.GetGenres();
                ViewBag.generos = generos ?? Enumerable.Empty<object>();

                if (!ModelState.IsValid) {
                    return View(objeto);
                }

                var ok = await _servicio.Update(objeto);
                if (ok) {
                    TempData["Msg"] = "✅ Película actualizada";
                    TempData["Tipo"] = "success";
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogWarning("No se pudo actualizar la película. idPublica={Id}", objeto?.idPublica);
                TempData["Msg"] = "No se pudo actualizar la película.";
                TempData["Tipo"] = "danger";
                return View(objeto);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error en MoviesController.Modificar POST idPublica={Id}", objeto?.idPublica);
                TempData["Msg"] = "Ocurrió un error al actualizar la película.";
                TempData["Tipo"] = "danger";
                return View(objeto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id) {
            var generos = await _servicio.GetGenres();
            ViewBag.generos = generos ?? Enumerable.Empty<object>();
            try {
                var ok = await _servicio.Delete(id);
                if (ok) {
                    TempData["Msg"] = "✅ Pelicula eliminada";
                    TempData["Tipo"] = "success";
                }
                else {
                    TempData["Msg"] = "No se pudo eliminar la pelicula";
                    TempData["Tipo"] = "danger";
                }
                return RedirectToAction(nameof(Index));
            } catch (Exception ex) {
                _logger.LogError(ex, "Error en MoviesController.Eliminar id={Id}", id);
                TempData["Msg"] = "Ocurrió un error al eliminar la pelicula.";
                TempData["Tipo"] = "danger";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
