using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NicoPasino.Core.DTO.Movies;
using NicoPasino.Core.Interfaces;
using NicoPasino.Core.Modelos.MoviesMySql;

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
                var vr = View(all ?? Enumerable.Empty<MovieDto>());
                vr.StatusCode = StatusCodes.Status200OK;
                return vr;
            } catch (Exception ex) {
                _logger.LogError(ex, "Error en MoviesController.Index");
                TempData["Msg"] = "Ocurrió un error al cargar las películas.";
                TempData["Tipo"] = "danger";
                var vr = View(Enumerable.Empty<MovieDto>());
                vr.StatusCode = StatusCodes.Status500InternalServerError;
                return vr;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Movie(int id) {
            try {
                var movie = await _servicio.GetById(id);
                if (movie != null) {
                    var vr = View("MovieInfo", movie);
                    vr.StatusCode = StatusCodes.Status200OK;
                    return vr;
                }

                TempData["Msg"] = "No se encontró la película.";
                TempData["Tipo"] = "warning";
                var notFoundVr = View("NotFound");
                notFoundVr.StatusCode = StatusCodes.Status404NotFound;
                return notFoundVr;
            } catch (Exception ex) {
                _logger.LogError(ex, "Error en MoviesController.MovieInfo id={Id}", id);
                TempData["Msg"] = "Ocurrió un error al obtener la información de la película.";
                TempData["Tipo"] = "danger";
                var vr = RedirectToAction(nameof(Index));
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return vr;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Buscar(string? titulo = "", int? idGenero = null) {
            try {
                var movies = Enumerable.Empty<MovieDto>();

                titulo = titulo?.Trim();
                if (!string.IsNullOrEmpty(titulo)) {
                    if (titulo.Length > 200) titulo = titulo[..200];
                    movies = await _servicio.GetAll(titulo);

                }
                if (idGenero.HasValue && idGenero.Value > 0) {
                    movies = await _servicio.GetAll(idGenero);
                }


                if (movies == null || !movies.Any()) {
                    TempData["Msg"] = "No se encontró ninguna película con los criterios indicados.";
                    TempData["Tipo"] = "info";
                    var vr = View("Index", Enumerable.Empty<MovieDto>());
                    vr.StatusCode = StatusCodes.Status404NotFound;
                    return vr;
                }

                var okVr = View("Index", movies);
                okVr.StatusCode = StatusCodes.Status200OK;
                return okVr;
            } catch (Exception ex) {
                _logger.LogError(ex, "Error en MoviesController.Buscar titulo={Titulo} idGenero={IdGenero}", titulo, idGenero);
                TempData["Msg"] = "Ocurrió un error al buscar películas.";
                TempData["Tipo"] = "danger";
                var vr = View("Index", Enumerable.Empty<MovieDto>());
                vr.StatusCode = StatusCodes.Status500InternalServerError;
                return vr;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Crear() => View();

        [HttpPost] // API
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(MovieDto objeto) {
            try {

                if (!ModelState.IsValid) {
                    var vr = View(objeto);
                    vr.StatusCode = StatusCodes.Status400BadRequest;
                    return vr;
                }

                var ok = await _servicio.Create(objeto);
                if (!ok) {
                    _logger.LogWarning("No se pudo crear la película (Create returned false). Title={Title}", objeto?.title);
                    TempData["Msg"] = "No se pudo crear la película.";
                    TempData["Tipo"] = "danger";
                    var vr = View(objeto);
                    vr.StatusCode = StatusCodes.Status500InternalServerError;
                    return vr;
                }

                // Creado
                TempData["Msg"] = "✅ Película creada";
                TempData["Tipo"] = "success";

                var vrOk = View(nameof(Index));
                vrOk.StatusCode = StatusCodes.Status201Created;
                return vrOk;
            } catch (Exception ex) {
                _logger.LogError(ex, "Error en MoviesController.Crear POST");
                TempData["Msg"] = "Ocurrió un error al crear la película.";
                TempData["Tipo"] = "danger";
                var vr = View(objeto);
                vr.StatusCode = StatusCodes.Status500InternalServerError;
                return vr;
            }
        }

        [HttpGet] // Vista MODIFICAR
        public async Task<IActionResult> Editar(int id) {
            try {
                var movie = await _servicio.GetById(id);
                if (movie == null) {
                    TempData["Msg"] = "No se encontró la película.";
                    TempData["Tipo"] = "warning";
                    var notFoundVr = View("NotFound");
                    notFoundVr.StatusCode = StatusCodes.Status404NotFound;
                    return notFoundVr;
                }

                var vr = View("Modificar", movie);
                vr.StatusCode = StatusCodes.Status200OK;
                return vr;
            } catch (Exception ex) {
                _logger.LogError(ex, "Error en MoviesController.Modificar GET id={Id}", id);
                TempData["Msg"] = "Ocurrió un error al preparar la edición.";
                TempData["Tipo"] = "danger";
                var vr = RedirectToAction(nameof(Index));
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return vr;
            }
        }

        [HttpPost] // API
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(MovieDto objeto) {
            try {

                if (!ModelState.IsValid) {
                    var vr = View("Modificar", objeto);
                    vr.StatusCode = StatusCodes.Status400BadRequest;
                    return vr;
                }

                var ok = await _servicio.Update(objeto); // actualizar
                if (ok) {
                    TempData["Msg"] = "✅ Película actualizada";
                    TempData["Tipo"] = "success";
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogWarning("No se pudo actualizar la película. idPublica={Id}", objeto?.idPublica);
                TempData["Msg"] = "No se pudo actualizar la película.";
                TempData["Tipo"] = "danger";
                var failVr = View("Modificar", objeto);
                failVr.StatusCode = StatusCodes.Status500InternalServerError;
                return failVr;
            } catch (Exception ex) {
                _logger.LogError(ex, "Error en MoviesController.Modificar POST idPublica={Id}", objeto?.idPublica);
                TempData["Msg"] = "Ocurrió un error al actualizar la película.";
                TempData["Tipo"] = "danger";
                var vr = View("Modificar", objeto);
                vr.StatusCode = StatusCodes.Status500InternalServerError;
                return vr;
            }
        }


        //[HttpGet]
        [HttpPost]
        public async Task<IActionResult> Desactivar(int id) {
            // TODO: poner delay según la última eliminacion + incrementar delay
            try {
                var ok = await _servicio.Enable(id, false);
                if (ok) {
                    TempData["Msg"] = "✅ Pelicula eliminada";
                    TempData["Tipo"] = "success";
                    return RedirectToAction(nameof(Index));
                }
                else {
                    TempData["Msg"] = "No se pudo eliminar la pelicula";
                    TempData["Tipo"] = "danger";
                    var vr = RedirectToAction(nameof(Index));
                    Response.StatusCode = StatusCodes.Status500InternalServerError;
                    return vr;
                }
            } catch (Exception ex) {
                _logger.LogError(ex, "Error en MoviesController.Eliminar id={Id}", id);
                TempData["Msg"] = "Ocurrió un error al eliminar la pelicula.";
                TempData["Tipo"] = "danger";
                var vr = RedirectToAction(nameof(Index));
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return vr;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Activar(int id) {
            try {
                var ok = await _servicio.Enable(id, true);
                if (ok) {
                    TempData["Msg"] = "✅ Pelicula activada";
                    TempData["Tipo"] = "success";
                    return RedirectToAction(nameof(Index));
                }
                else {
                    TempData["Msg"] = "No se pudo activar la pelicula";
                    TempData["Tipo"] = "danger";
                    var vr = RedirectToAction(nameof(Index));
                    Response.StatusCode = StatusCodes.Status500InternalServerError;
                    return vr;
                }
            } catch (Exception ex) {
                _logger.LogError(ex, "Error en MoviesController.Activar id={Id}", id);
                TempData["Msg"] = "Ocurrió un error al activar la pelicula.";
                TempData["Tipo"] = "danger";
                var vr = RedirectToAction(nameof(Index));
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return vr;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Desactivados() {
            try {
                var all = await _servicio.GetAll(false);
                var vr = View(nameof(Index), (all ?? Enumerable.Empty<MovieDto>()));
                vr.StatusCode = StatusCodes.Status200OK;
                return vr;
            } catch (Exception ex) {
                _logger.LogError(ex, "Error en MoviesController.Index");
                TempData["Msg"] = "Ocurrió un error al cargar las películas.";
                TempData["Tipo"] = "danger";
                var vr = RedirectToAction(nameof(Index), (Enumerable.Empty<MovieDto>()));
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return vr;
            }
        }
    }
}
