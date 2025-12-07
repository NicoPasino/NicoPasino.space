using NicoPasino.Core.DTO.Movies;

namespace NicoPasino.Core.Interfaces
{
    public interface IMovieServicio
    {
        Task<IEnumerable<MovieDto>> GetAll(bool activo);
        Task<IEnumerable<MovieDto>> GetAll(string titulo);
        Task<IEnumerable<MovieDto>> GetAll(int idGenero);
        Task<MovieDto> GetById(int id);
        Task<bool> Create(MovieDto obj);
        Task<bool> Update(MovieDto obj);
        Task<bool> Enable(int id, bool estado);
        //Task<IEnumerable<Genre>> GetGenres();
        //Task<IEnumerable<MovieDto>> GetAllActive(int? idGenero = null);
        //Task<IEnumerable<MovieDto>> Search(string nombre);
    }
}
