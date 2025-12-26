namespace NicoPasino.Core.Interfaces.Ventas
{
    public interface IServicioGenerico<T, TDto> where T : class where TDto : class
    {
        Task<IEnumerable<TDto>> GetAll(bool activo);
        Task<IEnumerable<TDto>> GetAll(string Busqueda);
        Task<IEnumerable<TDto>> GetAll(int id); // categoría
        Task<TDto> GetById(int id);
        Task<bool> Create(TDto obj);
        Task<bool> Update(TDto obj);
        Task<bool> Enable(int id, bool estado);
    }
}
