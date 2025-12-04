using NicoPasino.Core.DTO.Movies;
using NicoPasino.Core.Modelos.MoviesMySql;

namespace NicoPasino.Core.Mapper.Movies
{
    public static class MovieMapper
    {

        public static MovieDto ConvertToDto(Movie movieModel) {
            var objetoDTO = new MovieDto();
            try {
                objetoDTO.idPublica = movieModel.IdPublica;
                objetoDTO.title = movieModel.Title;
                objetoDTO.year = movieModel.Year;
                objetoDTO.director = movieModel.Director;
                objetoDTO.duration = movieModel.Duration;
                objetoDTO.poster = movieModel.Poster;
                objetoDTO.rate = movieModel.Rate;

                var mg = movieModel.Moviegenres ?? Enumerable.Empty<Moviegenres>();
                objetoDTO.genreIds = mg.Select(g => g.GenreId).ToList();
                objetoDTO.genreNames = mg
                    .Select(g => g.Genre?.Name)
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .Distinct()
                    .ToList();
            } catch (Exception) {
                // no propagar
            }
            return objetoDTO;
        }

        public static IEnumerable<MovieDto> ConvertToDtoList(IEnumerable<Movie> objeto) {
            var listaDTO = new List<MovieDto>();
            try {
                foreach (var item in objeto) {
                    var dto = ConvertToDto(item);
                    listaDTO.Add(dto);
                }
            } catch (Exception ex) {
            }

            return listaDTO;
        }

        public static Movie ConvertToMovie(MovieDto objeto) {
            var model = new Movie();

            try {
                model.IdPublica = objeto.idPublica;
                model.Title = objeto.title;
                model.Year = objeto.year;
                model.Director = objeto.director;
                model.Duration = objeto.duration;
                model.Poster = objeto.poster;
                model.Rate = objeto.rate;
                model.Activo = true;
                model.FechaModificacion = DateTime.Now;

                // NOTA: el mapeo de genre a entidades se realiza en el servicio (MovieServicio),
                // porque el mapper no tiene acceso al contexto/repositorio.
            } catch (Exception ex) {

            }

            return model;
        }
    }
}
