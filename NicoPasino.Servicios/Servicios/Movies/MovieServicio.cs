using NicoPasino.Core.DTO.Movies;
using NicoPasino.Core.Interfaces;
using NicoPasino.Core.Mapper.Movies;
using NicoPasino.Core.Modelos.MoviesMySql;

namespace NicoPasino.Servicios.Servicios.Movies
{
    public class MovieServicio : IMovieServicio
    {
        private readonly IRepositorioGenerico<Movie> _repoG;
        private readonly IRepositorioGenerico<Moviegenres> _repoMovieGenres;
        private readonly IUnitOfWork _uow;

        public MovieServicio(IUnitOfWork uow, IRepositorioGenerico<Movie> repoG, IRepositorioGenerico<Moviegenres> repoMovieGenres) {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _repoG = repoG ?? throw new ArgumentNullException(nameof(repoG));
            _repoMovieGenres = repoMovieGenres ?? throw new ArgumentNullException(nameof(repoMovieGenres));
        }

        public async Task<IEnumerable<MovieDto>> GetAll(bool? activo = true) {
            try {
                var moviesDb = await _repoG.ListarAsync(
                    filtro: m => m.Activo == activo,
                    orden: q => q.OrderByDescending(m => m.FechaModificacion ?? m.FechaCreacion),
                    incluir: "Moviegenres.Genre"
                );

                // si hay peliculas...
                if (moviesDb != null && moviesDb.Any()) {
                    var moviesDto = MovieMapper.ConvertToDtoList(moviesDb);
                    return moviesDto;
                }
                return Enumerable.Empty<MovieDto>();
            } catch (Exception ex) {
                return Enumerable.Empty<MovieDto>();
            }
        }

        public async Task<IEnumerable<MovieDto>> GetAll(string? titulo = "") {
            try {
                titulo = titulo?.Trim();
                if (!string.IsNullOrEmpty(titulo) && titulo.Length > 200) {
                    titulo = titulo.Substring(0, 200);
                }

                var moviesDb = await _repoG.ListarAsync(
                    filtro: m => m.Activo && (string.IsNullOrEmpty(titulo) || m.Title.Contains(titulo)),
                    orden: q => q.OrderByDescending(m => m.FechaModificacion ?? m.FechaCreacion),
                    incluir: "Moviegenres.Genre"
                );

                // si hay peliculas...
                if (moviesDb != null && moviesDb.Any()) {
                    var moviesDto = MovieMapper.ConvertToDtoList(moviesDb);
                    return moviesDto;
                }

                return Enumerable.Empty<MovieDto>();
            } catch (Exception ex) {
                return Enumerable.Empty<MovieDto>();
            }
        }

        public async Task<IEnumerable<MovieDto>> GetAll(int? idGenero = null) {
            try {
                if (idGenero.HasValue && idGenero.Value <= 0) {
                    idGenero = null;
                }

                var moviesDb = await _repoG.ListarAsync(
                    filtro: m => m.Activo && (!idGenero.HasValue || (m.Moviegenres != null && m.Moviegenres.Any(g => g.GenreId == idGenero.Value))),
                    orden: q => q.OrderByDescending(m => m.FechaModificacion ?? m.FechaCreacion),
                    incluir: "Moviegenres.Genre"
                );

                // si hay peliculas...
                if (moviesDb != null && moviesDb.Any()) {
                    var moviesDto = MovieMapper.ConvertToDtoList(moviesDb);
                    return moviesDto;
                }

                return Enumerable.Empty<MovieDto>();
            } catch (Exception ex) {
                return Enumerable.Empty<MovieDto>();
            }
        }

        public async Task<MovieDto> GetById(int id) {
            if (id <= 0) return null;
            try {
                var objDb = await _repoG.GetAsync(
                    filtro: m => m.IdPublica == id,
                    incluir: "Moviegenres.Genre"
                );

                if (objDb != null) {
                    var movieDto = MovieMapper.ConvertToDto(objDb);
                    return movieDto;
                }

                return null;
            } catch (Exception ex) {
                return null;
            }
        }

        public async Task<bool> Create(MovieDto objDto) {
            Random random = new Random();
            if (objDto == null) throw new ArgumentNullException(nameof(objDto));

            try {
                objDto.idPublica = random.Next(1, 9999999);
                var movie = MovieMapper.ConvertToMovie(objDto);

                movie.FechaCreacion = DateTime.UtcNow;
                movie.Activo = true;

                var res = await _repoG.Add(movie);

                // Agregar géneros si se añadió la pelicula correctamente
                var genreIds = objDto.genreIds;
                if (genreIds != null && genreIds.Any() && res != null) {
                    await GuardarMovieGenre(movie.Id, genreIds);
                    await _uow.SaveChangesAsync();
                    return true;
                }
                else
                    return false;
            } catch (Exception ex) {
                return false;
            }
        }

        public async Task<bool> Update(MovieDto obj) {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            try {
                var objDb = await _repoG.GetAsync(filtro: x => x.IdPublica == obj.idPublica, incluir: "Moviegenres");
                if (objDb == null) return false;

                var movie = MovieMapper.ConvertToMovie(obj);

                movie.Id = objDb.Id;
                movie.FechaCreacion = objDb.FechaCreacion;

                var res = _repoG.Update(movie);

                try {
                    // Agregar géneros si se añadió la pelicula correctamente
                    var genreIds = obj.genreIds;
                    if (genreIds != null && genreIds.Any() /*&& res > 0*/) { // TODO
                        await GuardarMovieGenre(objDb.Id, genreIds);
                        await _uow.SaveChangesAsync();
                        return true;
                    }
                    else return false;
                } catch (Exception ex) {
                    return false;
                }

            } catch (Exception ex) {
                Console.WriteLine(ex);

                return false;
            }
        }

/*
        public async Task<bool> Update2(MovieDto obj) {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            try {
                var _repoGenre = _uow.Repositorio<Genre>();

                var objDb = await _repoG.GetAsync(
                    filtro: x => x.IdPublica == obj.idPublica,
                    incluir: "Genre"
                );
                if (objDb == null) return false;

                objDb.Title = obj.title ?? objDb.Title;
                ... MAPEAR

                if (obj.genreIds != null) {
                    var generos = obj.genreIds.Any() ? (await _repoGenre.ListarAsync(filtro: g => obj.genreIds.Contains(g.Id))).ToList() : new List<Genre>();

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
        }*/

        public async Task<bool> Enable(int id, bool estado) {
            try {
                var objDb = await _repoG.GetAsync(
                    filtro: m => m.IdPublica == id
                );

                if (objDb != null) {
                    objDb.FechaModificacion = DateTime.UtcNow;
                    objDb.Activo = estado;

                    await _repoG.Update(objDb);

                    try {
                        await _uow.SaveChangesAsync();
                    } catch (Exception saveEx) {
                    }

                    return true;
                }

                return false;
            } catch (Exception ex) {
                return false;
            }
        }

        public async Task<IEnumerable<Genre>> GetGenres() {
            try {
                var _repoGenres = _uow.Repositorio<Genre>();
                var generos = await _repoGenres.GetAll();
                return generos ?? Enumerable.Empty<Genre>();
            } catch (Exception ex) {
                return Enumerable.Empty<Genre>();
            }
        }

        public async Task GuardarMovieGenre(int movieId, IEnumerable<int> genreIds) {
            // borrar generos anteriores si existen
            var movieGenres = await _repoMovieGenres.ListarAsync(filtro: mg => mg.MovieId == movieId);
            if (movieGenres != null) {
                await _repoMovieGenres.DeleteRange(movieGenres);
                await _uow.SaveChangesAsync();
            }

            // agregar nuevos géneros
            foreach (var id in genreIds) {
                var gen = new Moviegenres { MovieId = movieId, GenreId = id };
                await _repoMovieGenres.Add(gen);
            }
            await _uow.SaveChangesAsync();
        }

        /*public async Task<IEnumerable<string>> GetGenreNamesByMovieIdPublica(int idPublica) {
            try {
                if (idPublica <= 0) return Enumerable.Empty<string>();

                var movie = await _repoG.GetAsync(filtro: m => m.IdPublica == idPublica);
                if (movie == null) return Enumerable.Empty<string>();

                // obtener las relaciones Moviegenres incluyendo la entidad Genre para tomar su nombre.
                var movieGenres = await _repoMovieGenres.ListarAsync(filtro: mg => mg.MovieId == movie.Id, incluir: "Genre");
                if (movieGenres == null || !movieGenres.Any()) return Enumerable.Empty<string>();

                var names = movieGenres
                    .Select(mg => mg.Genre?.Name)
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .Distinct()
                    .ToList();

                return names;
            } catch (Exception) {
                return Enumerable.Empty<string>();
            }
        }*/
    }
}



// TODO:
// TODO:
// TODO:
// TODO:

// FIXME: no se puede agregar solo UN Género, (no se agrega pero si se crea la pelicula)

// TODO:
// TODO:
// TODO:
// TODO: