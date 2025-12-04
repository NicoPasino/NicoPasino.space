using NicoPasino.Core.DTO.Movies;
using NicoPasino.Core.Modelos.MoviesMySql;

namespace NicoPasino.Core.Interfaces
{
    public interface IGeneroServicio
    {
        Task<IEnumerable<Genre>> GetAll();
        Task<Genre> GetById(int id);
        Task<bool> Create(GeneroDto dto);
        Task<bool> Update(GeneroDto dto);
        Task<bool> Delete(int id);
    }
}