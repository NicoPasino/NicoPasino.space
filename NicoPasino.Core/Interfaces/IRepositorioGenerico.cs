using System.Linq.Expressions;

namespace NicoPasino.Core.Interfaces
{
    public interface IRepositorioGenerico<T> where T : class
    {
        public Task<T> Add(T entity);
        public Task AddRange(IEnumerable<T> entity);
        public Task<int> Update(T entity);
        public Task Delete(T entity);
        public Task DeleteRange(IEnumerable<T> entity);
        public Task<T?> GetById(int id);
        public Task<IEnumerable<T?>> GetAll();
        public Task<T> GetAsync(Expression<Func<T, bool>>? filtro = null, string incluir = "");
        public Task<IEnumerable<T>> ListarAsync(Expression<Func<T, bool>>? filtro = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orden = null, string incluir = "");
    }
}