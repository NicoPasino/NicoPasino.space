using NicoPasino.Core.DTO.Movies;
using NicoPasino.Core.Modelos.Movies;

namespace NicoPasino.Core.Interfaces
{
    public interface IMovieRepo
    {
        Task<IEnumerable<MovieDto>> GetAll();
        Task<MovieDto> GetById(int id);
        Task<bool> Create(MovieDto obj);
        Task<bool> Update(MovieDto obj);
        Task<bool> Delete(int id);
        Task<IEnumerable<Genre>> GetGenres();
        //Task<IEnumerable<MovieDto>> Search(string nombre);
    }
}
