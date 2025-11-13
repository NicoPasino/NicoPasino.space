using Microsoft.Extensions.Logging;
using NicoPasino.Core.DTO.Movies;
using NicoPasino.Core.Interfaces;
using NicoPasino.Core.Mapper.Movies;
using NicoPasino.Core.Modelos.Movies;
using System.Linq.Expressions;

namespace NicoPasino.Servicios.Servicios.Movies
{
    public class MovieServicio : IMovieServicio
    {
        private readonly IRepositorioGenerico<Movie> _repoG;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<MovieServicio> _logger;

        public MovieServicio(IUnitOfWork uow, IRepositorioGenerico<Movie> repoG, ILogger<MovieServicio> logger) {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _repoG = repoG ?? throw new ArgumentNullException(nameof(repoG));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<MovieDto>> GetAll(string? titulo = "", int? idGenero = null) {
            try {
                titulo = titulo?.Trim();
                if (!string.IsNullOrEmpty(titulo) && titulo.Length > 200) {
                    titulo = titulo.Substring(0, 200);
                }

                if (idGenero.HasValue && idGenero.Value <= 0) {
                    idGenero = null;
                }

                // Construcción del filtro de forma segura: se combinan las condiciones opcionales
                Expression<Func<Movie, bool>> filtro = m =>
                    m.Activo
                    && (string.IsNullOrEmpty(titulo) || m.Title.Contains(titulo))
                    && (!idGenero.HasValue || (m.Genre != null && m.Genre.Any(g => g.Id == idGenero.Value)));

                var entidades = await _repoG.ListarAsync(
                    filtro: filtro,
                    orden: q => q.OrderByDescending(m => m.FechaModificacion ?? m.FechaCreacion),
                    incluir: "Genre"
                );

                if (entidades != null && entidades.Any())
                    return MovieMapper.ConvertToDtoList(entidades);

                return Enumerable.Empty<MovieDto>();
            } catch (Exception ex) {
                // Registro seguro del error sin exponer detalles sensibles
                _logger.LogError(ex, "MovieServicio.GetAll fallo. titulo={Titulo} idGenero={IdGenero}", titulo, idGenero);
                return Enumerable.Empty<MovieDto>();
            }
        }

        public async Task<MovieDto> GetById(int id) {
            try {
                var objDb = await _repoG.GetAsync(
                    filtro: m => m.IdPublica == id,
                    incluir: "Genre"
                );

                if (objDb != null) return MovieMapper.ConvertToDto(objDb);

                return null;
            } catch (Exception ex) {
                _logger.LogError(ex, "MovieServicio.GetById fallo. id={Id}", id);
                return null;
            }
        }

        public async Task<bool> Create(MovieDto objDto) {
            if (objDto == null) throw new ArgumentNullException(nameof(objDto));

            try {
                var _repoGenre = _uow.Repositorio<Genre>();
                var movie = MovieMapper.ConvertToMovie(objDto) ?? new Movie();

                // Mapear géneros de forma segura
                if (objDto.genreIds != null && objDto.genreIds.Any()) {
                    var generos = await _repoGenre.ListarAsync(filtro: g => objDto.genreIds.Contains(g.Id));
                    movie.Genre = generos?.ToList() ?? new List<Genre>();
                }
                else {
                    movie.Genre = movie.Genre ?? new List<Genre>();
                }

                movie.FechaCreacion = DateTime.UtcNow;
                movie.Activo = true;

                await _repoG.Add(movie);

                // Guardar cambios si la unidad de trabajo lo requiere
                try {
                    await _uow.SaveChangesAsync();
                } catch (Exception saveEx) {
                    _logger.LogWarning(saveEx, "SaveChangesAsync falló después de Add en Create. Se intentó añadir la película pero no se guardaron los cambios.");
                }

                return true;
            } catch (Exception ex) {
                _logger.LogError(ex, "MovieServicio.Create fallo. Title={Title}", objDto?.title);
                return false;
            }
        }

        public async Task<bool> Update(MovieDto obj) {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            try {
                var _repoGenre = _uow.Repositorio<Genre>();

                var objDb = await _repoG.GetAsync(
                    filtro: x => x.IdPublica == obj.idPublica,
                    incluir: "Genre"
                );

                if (objDb == null) return false;

                objDb.Title = obj.title ?? objDb.Title;
                objDb.Year = obj.year;
                objDb.Director = obj.director ?? objDb.Director;
                objDb.Duration = obj.duration;
                objDb.Poster = obj.poster ?? objDb.Poster;
                objDb.FechaModificacion = DateTime.UtcNow;
                objDb.Activo = true;

                if (obj.genreIds != null) {
                    var generos = obj.genreIds.Any()
                        ? (await _repoGenre.ListarAsync(filtro: g => obj.genreIds.Contains(g.Id))).ToList()
                        : new List<Genre>();

                    objDb.Genre = objDb.Genre ?? new List<Genre>();
                    objDb.Genre.Clear();
                    foreach (var g in generos) {
                        objDb.Genre.Add(g);
                    }
                }

                await _repoG.Update(objDb);

                try {
                    await _uow.SaveChangesAsync();
                } catch (Exception saveEx) {
                    _logger.LogWarning(saveEx, "SaveChangesAsync falló después de Update en Update. idPublica={Id}", obj.idPublica);
                }

                return true;
            } catch (Exception ex) {
                _logger.LogError(ex, "MovieServicio.Update fallo. idPublica={Id}", obj?.idPublica);
                return false;
            }
        }

        public async Task<bool> Delete(int id) {
            try {
                var objDb = await _repoG.GetAsync(
                    filtro: m => m.IdPublica == id,
                    incluir: "Genre"
                );

                if (objDb != null) {
                    objDb.FechaModificacion = DateTime.UtcNow;
                    objDb.Activo = false;

                    await _repoG.Update(objDb);

                    try {
                        await _uow.SaveChangesAsync();
                    } catch (Exception saveEx) {
                        _logger.LogWarning(saveEx, "SaveChangesAsync falló después de Update en Delete. idPublica={Id}", id);
                    }

                    return true;
                }

                return false;
            } catch (Exception ex) {
                _logger.LogError(ex, "MovieServicio.Delete fallo. id={Id}", id);
                return false;
            }
        }

        public async Task<IEnumerable<Genre>> GetGenres() {
            try {
                var _repoGenre = _uow.Repositorio<Genre>();
                var generos = await _repoGenre.GetAll();
                return generos ?? Enumerable.Empty<Genre>();
            } catch (Exception ex) {
                _logger.LogError(ex, "MovieServicio.GetGenres fallo.");
                return Enumerable.Empty<Genre>();
            }
        }
    }
}
