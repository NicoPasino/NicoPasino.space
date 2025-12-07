using Microsoft.AspNetCore.Mvc;
using NicoPasino.Core.DTO.Movies;
using NicoPasino.Core.Errores.Movies;
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
                var all = await _servicio.GetAll(true);
                if (all != null) return View(all);

                // 404
                TempData["Msg"] = "No se encontró ninguna película.";
                TempData["Tipo"] = "info";
                return View();
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error en MoviesController.Index"); // TODO:
                return RedirectToAction("Error", "Home", new { mensaje = "Ocurrió un error al cargar las películas." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Movie(int id) {
            try {
                var movie = await _servicio.GetById(id);
                if (movie != null) return View("MovieInfo", movie);
                else return NotFound(404, "No se encontró la película."); // 404
            }
            catch (ArgumentException ex) {
                return NotFound(400, ex.Message); // 400 Bad Request
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error en MoviesController.MovieInfo id={Id}", id); // log
                return ErrorMovie("Ocurrió un error al obtener la información de la película.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Buscar(string titulo) {
            try {
                var movies = await _servicio.GetAll(titulo);

                if (movies != null && movies.Any())
                    return View("Index", movies);

                TempData["Msg"] = "No se encontró ninguna película.";
                TempData["Tipo"] = "info";
                return View("Index", Enumerable.Empty<MovieDto>());
            }
            catch (ArgumentException ex) {
                return NotFound(400, ex.Message);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error en MoviesController.Buscar titulo={Titulo}", titulo); // log
                return ErrorMovie("Ocurrió un error al buscar las películas.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Filtrar(int idGenero) {
            try {
                var movies = await _servicio.GetAll(idGenero);

                if (movies != null && movies.Any())
                    return View("Index", movies);

                TempData["Msg"] = "No se encontró ninguna película.";
                TempData["Tipo"] = "info";
                return View("Index", Enumerable.Empty<MovieDto>());
            }
            catch (ArgumentException ex) {
                return NotFound(400, ex.Message);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error en MoviesController.Filtrar idGenero={IdGenero}", idGenero); // log
                return ErrorMovie("Ocurrió un error al buscar las películas.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Crear() => View();

        [HttpPost] // API
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(MovieDto objeto) {
            try {
                if (!ModelState.IsValid) throw new MovieDataException("Datos inválidos, por favor revisar.");

                var ok = await _servicio.Create(objeto);
                if (ok) {
                    TempData["Msg"] = "✅ Película creada";
                    TempData["Tipo"] = "success";
                    return RedirectToAction(nameof(Index));
                }
                else throw new Exception();
            }
            catch (MovieDataException ex) { // 400
                TempData["Msg"] = ex.Message;
                TempData["Tipo"] = "danger";
                var viewReturn = View(objeto);
                viewReturn.StatusCode = StatusCodes.Status400BadRequest;
                return viewReturn;
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error en MoviesController.Crear");
                return ErrorMovie("Ocurrió un error al crear la película.");
            }
        }

        [HttpGet] // Vista MODIFICAR
        public async Task<IActionResult> Editar(int id) {
            try {
                var movie = await _servicio.GetById(id);
                if (movie != null) return View(movie);

                else return NotFound(404, "No se encontró la película.");
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error en MoviesController.Modificar GET id={Id}", id);
                return ErrorMovie("Ocurrió un error al modificar la película.");
            }
        }

        [HttpPost] // API
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(MovieDto objeto) {
            try {
                if (!ModelState.IsValid) throw new MovieDataException("Datos inválidos, por favor revisar.");

                // Actualizar...
                var res = await _servicio.Update(objeto);
                if (res) {
                    TempData["Msg"] = "✅ Película actualizada";
                    TempData["Tipo"] = "success";
                    return RedirectToAction(nameof(Index));
                }
                else throw new Exception();
            }
            catch (MovieDataException ex) { // 400
                TempData["Msg"] = ex.Message;
                TempData["Tipo"] = "danger";
                var viewReturn = View(objeto);
                viewReturn.StatusCode = StatusCodes.Status400BadRequest;
                return viewReturn;
            }
            catch (Exception ex) { // 500
                _logger.LogError(ex, "Error en MoviesController.Editar POST idPublica={Id}", objeto?.idPublica);
                return ErrorMovie("Error al actualizar la película, no se pudo guardar en la base de datos.");
            }
        }


        //[HttpGet]
        [HttpPost]
        public async Task<IActionResult> Desactivar(int id) {
            // TODO: poner delay según la última eliminacion + incrementar delay
            try {
                var ok = await _servicio.Enable(id, false);
                if (ok) {
                    TempData["Msg"] = "✅ Película eliminada";
                    TempData["Tipo"] = "success";
                    return RedirectToAction(nameof(Index));
                }
                else throw new Exception();
            }
            catch (ArgumentException ex) {
                return NotFound(400, ex.Message);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error en MoviesController.Eliminar id={Id}", id);
                return ErrorMovie("Error al actualizar la película, no se pudo guardar en la base de datos.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Activar(int id) {
            try {
                var ok = await _servicio.Enable(id, true);
                if (ok) {
                    TempData["Msg"] = "✅ Película activada";
                    TempData["Tipo"] = "success";
                    return RedirectToAction(nameof(Index));
                }
                else throw new Exception();
            }
            catch (ArgumentException ex) {
                return NotFound(400, ex.Message);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error en MoviesController.Activar id={Id}", id);
                return ErrorMovie("Error al actualizar la película, no se pudo guardar en la base de datos.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Desactivados() {
            try {
                var all = await _servicio.GetAll(false);
                return View(nameof(Index), (all ?? Enumerable.Empty<MovieDto>()));
            }
            catch (Exception ex) {
                return ErrorMovie("Ocurrió un error al cargar las películas Desactivadas.");
            }
        }



        private IActionResult ErrorMovie(string? mensaje) {
            return RedirectToAction("Error", "Home", new { mensaje, controlador = "Movies" });
        }


        private IActionResult NotFound(int statusCode, string? mensaje) {
            ViewData["StatusCode"] = statusCode;
            ViewData["Title"] = "Página No Encontrada";

            ViewData["Controlador"] = "Movies";
            ViewData["Mensaje"] = mensaje;

            var view = View("NotFound");
            view.StatusCode = statusCode;
            return view;
        }
    }
}
