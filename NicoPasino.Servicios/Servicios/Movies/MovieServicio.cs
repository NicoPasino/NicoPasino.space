using NicoPasino.Core.DTO.Movies;
using NicoPasino.Core.Errores.Movies;
using NicoPasino.Core.Interfaces;
using NicoPasino.Core.Mapper.Movies;
using NicoPasino.Core.Modelos.MoviesMySql;

namespace NicoPasino.Servicios.Servicios.Movies
{
    public class MovieServicio : IMovieServicio
    {
        private readonly IRepositorioGenerico<Movie> _repoG;
        private readonly IRepositorioGenerico<Moviegenres> _repoMovieGenres;
        private readonly IUnitOfWorkMovie _uow;

        public MovieServicio(IUnitOfWorkMovie uow, IRepositorioGenerico<Movie> repoG, IRepositorioGenerico<Moviegenres> repoMovieGenres) {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _repoG = repoG ?? throw new ArgumentNullException(nameof(repoG));
            _repoMovieGenres = repoMovieGenres ?? throw new ArgumentNullException(nameof(repoMovieGenres));
        }

        public async Task<IEnumerable<MovieDto>> GetAll(bool activo) {
            try {
                var moviesDb = await _repoG.ListarAsync(
                    filtro: m => m.Activo == activo,
                    orden: q => q.OrderByDescending(m => m.FechaModificacion ?? m.FechaCreacion),
                    incluir: "Moviegenres.Genre"
                );

                // si hay películas...
                if (moviesDb != null && moviesDb.Any()) {
                    var moviesDto = MovieMapper.ConvertToDtoList(moviesDb);
                    return moviesDto;
                }
                return Enumerable.Empty<MovieDto>();
            }
            catch (Exception ex) {
                throw new Exception();
            }
        }

        public async Task<IEnumerable<MovieDto>> GetAll(string titulo) {
            if (titulo == null) throw new ArgumentException("Texto vacío o no válido.");
            // TODO: otras validaciones

            try {
                titulo = titulo.Trim();
                titulo = titulo.Length > 30 ? titulo = titulo.Substring(0, 30) : titulo;

                var moviesDb = await _repoG.ListarAsync(
                    filtro: m => m.Activo && (string.IsNullOrEmpty(titulo) || m.Title.Contains(titulo)),
                    orden: q => q.OrderByDescending(m => m.FechaModificacion ?? m.FechaCreacion),
                    incluir: "Moviegenres.Genre"
                );

                // si hay películas...
                if (moviesDb != null && moviesDb.Any()) {
                    var moviesDto = MovieMapper.ConvertToDtoList(moviesDb);
                    return moviesDto;
                }
                return Enumerable.Empty<MovieDto>();
            }
            catch (Exception ex) {
                throw new Exception();
            }
        }

        public async Task<IEnumerable<MovieDto>> GetAll(int idGenero) {
            try {
                if (!ValidateGenre(idGenero)) throw new ArgumentException("El ID del Género no es válido.");

                var moviesDb = await _repoG.ListarAsync(
                    filtro: m => m.Activo && (m.Moviegenres != null && m.Moviegenres.Any(g => g.GenreId == idGenero)),
                    orden: q => q.OrderByDescending(m => m.FechaModificacion ?? m.FechaCreacion),
                    incluir: "Moviegenres.Genre"
                );

                // si hay películas...
                if (moviesDb != null && moviesDb.Any()) {
                    var moviesDto = MovieMapper.ConvertToDtoList(moviesDb);
                    return moviesDto;
                }

                return Enumerable.Empty<MovieDto>();
            }
            catch (ArgumentException ex) {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex) {
                throw new Exception();
            }
        }

        public async Task<MovieDto> GetById(int id) {
            if (id <= 0) throw new ArgumentException("Id no válido");
            var objDb = await _repoG.GetAsync(
                filtro: m => m.IdPublica == id,
                incluir: "Moviegenres.Genre"
            );

            if (objDb != null) {
                var movieDto = MovieMapper.ConvertToDto(objDb);
                return movieDto;
            }

            return new MovieDto();
        }

        public async Task<bool> Create(MovieDto obj) {
            Random random = new Random();
            if (obj == null) throw new MovieDataException("No se recibió ningún dato.");
            // TODO: otras validaciones
            // TODO: validaciones de géneros

            obj.idPublica = random.Next(1, 9999999);
            var movie = MovieMapper.ConvertToMovie(obj);

            movie.FechaCreacion = DateTime.UtcNow;
            movie.Activo = true;

            var res = await _repoG.Add(movie);

            // Agregar géneros si se añadió la película correctamente
            var genreIds = obj.genreIds;
            if (genreIds != null && genreIds.Any() && res != null) {
                await GuardarMovieGenre(movie.Id, genreIds);
                await _uow.SaveChangesAsync();
                return true;
            }
            else
                return false;
        }

        public async Task<bool> Update(MovieDto obj) {
            // OPTIMIZE: usar NicoPasino.Core.Validate.Movies -> MovieDataValidation
            if (obj == null) throw new MovieDataException("No se recibió ningún dato.");
            else if (obj.idPublica == null) throw new MovieDataException("No se recibió ningún ID.");
            // TODO: comparar con datos originales (ojo con genreNames y genreId)
            //var movieOriginal = await _servicio.GetById((int)objeto.idPublica); 
            //if (objeto == movieOriginal) throw new MovieDataException("Se recibieron datos sin cambios, No se actualizó."); // FIXME:

            // TODO: otras validaciones
            // TODO: validaciones de géneros

            // obtener movie original
            var objDb = await _repoG.GetAsync(filtro: x => x.IdPublica == obj.idPublica, incluir: "Moviegenres");
            if (objDb == null) throw new MovieDataException("Película original no encontrada.");

            // mapear
            var movie = MovieMapper.ConvertToMovie(obj);
            movie.Id = objDb.Id;
            movie.FechaModificacion = objDb.FechaModificacion;

            // subir
            var res = _repoG.Update(movie);
            //if (res == 0) throw new MovieNotUpdatedException("La Película no pudo actualizar en la base de datos.");

            // Agregar los géneros (si se añadió la Película correctamente)
            var genreIds = obj.genreIds;
            if (genreIds != null && genreIds.Any() && res != null) { // TODO:
                await GuardarMovieGenre(objDb.Id, genreIds);
                await _uow.SaveChangesAsync();
                return true;
            }
            else throw new MovieUpdateException("La Película no pudo actualizar en la base de datos.");
            //else throw new MovieUpdateException("No se pudieron actualizar los géneros.");

        }

        public async Task<bool> Enable(int id, bool estado) {
            if (id <= 0) throw new ArgumentException("Id no válido");
            var objDb = await _repoG.GetAsync(filtro: m => m.IdPublica == id);

            if (objDb != null) {
                objDb.FechaModificacion = DateTime.UtcNow;
                objDb.Activo = estado;

                await _repoG.Update(objDb);
                await _uow.SaveChangesAsync();
                return true;
            }
            else throw new ArgumentException("Id no válido");
        }

        /// <summary>
        ///  Return bool
        /// </summary>
        public bool ValidateGenre(int idGenero) {
            var _repoGenres = _uow.Repositorio<Genre>();
            var AllIds = _repoGenres.GetAll();
            if (idGenero <= 0) return false;
            if (!AllIds.Result.Any(g => g.Id == idGenero)) return false;
            return true;
        }

        public async Task GuardarMovieGenre(int movieId, IEnumerable<int> genreIds) {
            // borrar géneros anteriores si existen
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
    }
}
