namespace NicoPasino.Core.Interfaces
{
    public interface IServicioGenerico<T, TDto> where T : class where TDto : class
    {
        Task<IEnumerable<TDto>> GetAll(bool activo);
        Task<IEnumerable<TDto>> GetAll(string campo, string? valor); // búsqueda por campo
        Task<TDto> GetById(int id);
        Task<bool> Create(TDto obj);
        Task<bool> Update(TDto obj);
        Task<bool> Enable(int id, bool estado);
    }
}
