using NicoPasino.Core.DTO.Movies;
using NicoPasino.Core.Modelos.Movies;

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
                if (movieModel.Genre != null && movieModel.Genre.Any()) {
                    objetoDTO.genreIds = movieModel.Genre.Select(g => g.Id).ToList();
                    objetoDTO.genreNames = movieModel.Genre.Select(g => g.Name).ToList();
                }
            } catch (Exception ex) {
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

                // TODO: hacer manualmente en todos los ConvertToMovie
                /*
                // Mapear géneros seleccionados (ids) a entidades Genre para la relación many-to-many
                if (objeto.genreIds != null && objeto.genreIds.Any()) {
                    var generos = await _contexto.Genre.Where(g => objeto.genreIds.Contains(g.Id)).ToListAsync();
                    model.Genre = generos;
                }*/

            } catch (Exception ex) {

            }

            return model;
        }
    }
}
