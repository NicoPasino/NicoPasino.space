using NicoPasino.Core.DTO.Movies;
using NicoPasino.Core.Interfaces;
using NicoPasino.Core.Modelos.Movies;

namespace NicoPasino.Servicios.Servicios.Movies
{
    public class MovieServicio : IMovieServicio
    {
        private readonly IMovieRepo _repo;
        public MovieServicio(IMovieRepo repo) {
            _repo = repo;
        }

        // Lógica de negocio (validaciones de datos)

        public async Task<IEnumerable<MovieDto>> GetAll() {
            var objeto = await _repo.GetAll();
            return objeto;
        }

        public async Task<MovieDto?> GetById(int id) {
            var objeto = await _repo.GetById(id);
            if (objeto != null) return objeto;
            return null;
        }

        public async Task<bool> Create(MovieDto objeto) {
            await _repo.Create(objeto);
            return true;
        }

        public async Task<bool> Update(MovieDto obj) {
            var objeto = await _repo.Update(obj);
            return objeto;
        }

        public async Task<bool> Delete(int id) {
            var objeto = await _repo.Delete(id);
            return objeto;
        }

        /*public async Task<IEnumerable<MovieDto>> Search(string nombre) {
            var objetos = await _repo.Search(nombre.ToLower());
            return objetos;
        }*/

        public async Task<IEnumerable<Genre>> GetGenres() {
            var generos = await _repo.GetGenres();
            return generos;
        }
    }
}
