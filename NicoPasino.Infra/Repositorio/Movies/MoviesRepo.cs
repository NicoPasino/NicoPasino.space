using Microsoft.EntityFrameworkCore;
using NicoPasino.Core.DTO.Movies;
using NicoPasino.Core.Interfaces;
using NicoPasino.Core.Modelos.Movies;
using NicoPasino.Infra.Data;

namespace NicoPasino.Core.Repositorio.Movies
{
    public class MoviesRepo : IMovieRepo
    {
        private readonly moviesdbContext _contexto;
        public MoviesRepo(moviesdbContext contexto) {
            _contexto = contexto;
        }

        public async Task<IEnumerable<MovieDto>> GetAll() {
            var lista = await _contexto.Movie
                .Include(m => m.Genre)
                .Where(x => x.Activo)
                .ToListAsync();
            return await ConvertToDtoList(lista);
        }

        public async Task<MovieDto?> GetById(int id) {
            try {
                var movie = await _contexto.Movie
                    .Include(m => m.Genre)
                    .Where(x => x.IdPublica == id)
                    .FirstOrDefaultAsync();
                if (movie != null) {
                    var movieDto = await ConvertToDto(movie);
                    return movieDto;
                }
                return null;
            } catch (Exception ex) {
                return null;
            }
        }



        public async Task<bool> Create(MovieDto objDto) {
            try {
                var movie = await ConvertToMovie(objDto);
                movie.FechaCreacion = DateTime.Now;

                _contexto.Add(movie);
                await _contexto.SaveChangesAsync();

                return true;
            } catch (Exception) {
                return false;
            }
        }



        public async Task<bool> Update(MovieDto obj) {
            try {
                //var objDb = await _contexto.Movie.Where(x => x.IdPublica == obj.idPublica).FirstOrDefaultAsync();
                var objDb = await _contexto.Movie
                    .Include(m => m.Genre)
                    .Where(x => x.IdPublica == obj.idPublica)
                    .FirstOrDefaultAsync();

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
                            ? await _contexto.Genre.Where(g => obj.genreIds.Contains(g.Id)).ToListAsync()
                            : new List<Genre>();

                        objDb.Genre.Clear();
                        foreach (var g in generos) {
                            objDb.Genre.Add(g);
                        }
                    }
                    _contexto.Update(objDb);
                    await _contexto.SaveChangesAsync();

                    return true;
                }
                else {
                    return false;
                }
            } catch (Exception) {
                return false;
            }
        }



        public async Task<bool> Delete(int id) {
            try {
                var objDb = await _contexto.Movie.Where(x => x.IdPublica == id).FirstOrDefaultAsync();

                if (objDb != null) {
                    objDb.FechaModificacion = DateTime.Now;
                    objDb.Activo = false;

                    _contexto.Update(objDb);
                    _contexto.SaveChanges();

                    //var res = ConvertToDto(objDb);
                    return true;
                }
                else {
                    return false;
                }
            } catch (Exception) {
                return false;
            }
        }




        /*public async Task<IEnumerable<MovieDto>> Search(string nombre) {
            var movies = await _contexto.Movie.Where(x => x.Title.ToLower().Contains(nombre)).ToListAsync();
            var moviesDto = ConvertToDtoList(movies);
            return moviesDto;
        }*/


        public async Task<IEnumerable<Genre>> GetGenres() {
            var lista = await _contexto.Genre.ToListAsync(); // TODO
            return lista;
        }


        // Estos Mapper deberia estar en el .Core
        private async Task<MovieDto> ConvertToDto(Movie movieModel) {
            var objetoDTO = new MovieDto();
            try {
                objetoDTO.idPublica = movieModel.IdPublica;
                objetoDTO.title = movieModel.Title;
                objetoDTO.year = movieModel.Year;
                objetoDTO.director = movieModel.Director;
                objetoDTO.duration = movieModel.Duration;
                objetoDTO.poster = movieModel.Poster;
                objetoDTO.rate = movieModel.Rate;
                if (movieModel.Genre != null && movieModel.Genre.Any()) {
                    objetoDTO.genreIds = movieModel.Genre.Select(g => g.Id).ToList();
                    objetoDTO.genreNames = movieModel.Genre.Select(g => g.Name).ToList();
                }
            } catch (Exception ex) {
            }
            return objetoDTO;
        }

        private async Task<IEnumerable<MovieDto>> ConvertToDtoList(IEnumerable<Movie> objeto) {
            var listaDTO = new List<MovieDto>();
            try {
                foreach (var item in objeto) {
                    var dto = await ConvertToDto(item);
                    listaDTO.Add(dto);
                }
            } catch (Exception ex) {
            }

            return listaDTO;
        }

        private async Task<Movie> ConvertToMovie(MovieDto objeto) {
            var model = new Movie();
            Random random = new Random();

            try {
                model.IdPublica = objeto.idPublica ?? random.Next(1, 9999999);
                model.Title = objeto.title;
                model.Year = objeto.year;
                model.Director = objeto.director;
                model.Duration = objeto.duration;
                model.Poster = objeto.poster;
                model.Rate = objeto.rate;
                model.Activo = true;
                model.FechaModificacion = DateTime.Now;

                // Mapear géneros seleccionados (ids) a entidades Genre para la relación many-to-many
                if (objeto.genreIds != null && objeto.genreIds.Any()) {
                    var generos = await _contexto.Genre.Where(g => objeto.genreIds.Contains(g.Id)).ToListAsync();
                    model.Genre = generos;
                }

            } catch (Exception ex) {

            }

            return model;
        }
    }
}
