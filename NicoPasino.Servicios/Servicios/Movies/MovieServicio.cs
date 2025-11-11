using NicoPasino.Core.DTO.Movies;
using NicoPasino.Core.Interfaces;
using NicoPasino.Core.Mapper.Movies;
using NicoPasino.Core.Modelos.Movies;

namespace NicoPasino.Servicios.Servicios.Movies
{
    public class MovieServicio : IMovieServicio
    {
        private readonly IRepositorioGenerico<Movie> _repoG;
        private readonly IUnitOfWork _uow;
        public MovieServicio(IUnitOfWork uow, IRepositorioGenerico<Movie> repoG) {
            _uow = uow;
            _repoG = repoG;
        }

        public async Task<IEnumerable<MovieDto>> GetAll() {
            var entidades = await _repoG.ListarAsync(
                filtro: m => m.Activo, /*&& (idGenero == null || m.Genre.Id == idGenero),*/
                orden: q => q.OrderByDescending(m => m.FechaModificacion ?? m.FechaCreacion),
                incluir: "Genre"
            );

            if (entidades.Count() > 0)
                return MovieMapper.ConvertToDtoList(entidades);
            return null;

        }

        public async Task<MovieDto?> GetById(int id) {
            var objDb = await _repoG.GetAsync(
                filtro: m => m.IdPublica == id,
                incluir: "Genre"
            );

            if (objDb != null) return MovieMapper.ConvertToDto(objDb);
            return null;
        }

        public async Task<bool> Create(MovieDto objDto) {
            try {
                var _repoGenre = _uow.Repositorio<Genre>();
                var movie = MovieMapper.ConvertToMovie(objDto);

                // TODO: hacer en todos los ConvertToMovie
                if (objDto.genreIds != null && objDto.genreIds.Any()) {
                    var generos = await _repoGenre.ListarAsync(filtro: g => objDto.genreIds.Contains(g.Id));
                    movie.Genre = (ICollection<Genre>)generos;
                }

                movie.FechaCreacion = DateTime.Now;
                await _repoG.Add(movie);
                return true;
            } catch (Exception ex) {
                return false;
            }
        }

        public async Task<bool> Update(MovieDto obj) {
            //var objeto = await _repo.Update(obj);
            //return objeto;

            var _repoGenre = _uow.Repositorio<Genre>();

            var objDb = await _repoG.GetAsync(
                filtro: x => x.IdPublica == obj.idPublica,
                incluir: "Genre"
            );

            if (objDb != null) {
                objDb.Title = obj.title;
                objDb.Year = obj.year;
                objDb.Director = obj.director;
                objDb.Duration = obj.duration;
                objDb.Poster = obj.poster;
                objDb.FechaModificacion = DateTime.Now;
                objDb.Activo = true;

                if (obj.genreIds != null) {
                    var generos = obj.genreIds.Any()
                        ? await _repoGenre.ListarAsync(filtro: g => obj.genreIds.Contains(g.Id)) : [];

                    objDb.Genre.Clear();
                    foreach (var g in generos) {
                        objDb.Genre.Add(g);
                    }
                }
                await _repoG.Update(objDb);

                return true;
            }
            return false;
        }

        public async Task<bool> Delete(int id) {
            var objDb = await _repoG.GetAsync(
                filtro: m => m.IdPublica == id,
                incluir: "Genre"
            );

            if (objDb != null) {
                objDb.FechaModificacion = DateTime.Now;
                objDb.Activo = false;

                await _repoG.Update(objDb);

                return true;
            }
            return false;
        }

        /*public async Task<IEnumerable<MovieDto>> Search(string nombre) {
            var objetos = await _repo.Search(nombre.ToLower());
            return objetos;
        }*/

        public async Task<IEnumerable<Genre>> GetGenres() {
            var _repoGenre = _uow.Repositorio<Genre>();

            var generos = await _repoGenre.GetAll();
            if (generos != null) return generos;
            return null;
        }
    }
}
