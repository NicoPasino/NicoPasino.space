using NicoPasino.Core.DTO.Ventas;

namespace NicoPasino.Core.Interfaces.Ventas
{
    public interface IProductoServicio
    {
        Task<IEnumerable<ProductoDto>> GetAll(bool activo);
        Task<IEnumerable<ProductoDto>> GetAll(string Busqueda);
        Task<IEnumerable<ProductoDto>> GetAll(int idCategoria);
        Task<ProductoDto> GetById(int id);
        Task<bool> Create(ProductoDto obj);
        Task<bool> Update(ProductoDto obj);
        Task<bool> Enable(int id, bool estado);
    }
}
