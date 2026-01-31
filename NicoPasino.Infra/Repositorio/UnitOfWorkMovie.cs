using NicoPasino.Core.Interfaces;
using NicoPasino.Infra.Data;

namespace NicoPasino.Infra.Repositorio
{
    public class UnitOfWorkMovie : IUnitOfWorkMovie
    {
        private readonly moviesdbContext _context;
        private readonly Dictionary<Type, object> _repositorios;

        public UnitOfWorkMovie(moviesdbContext context) {
            _context = context;
            _repositorios = new Dictionary<Type, object>();
        }

        public IRepositorioGenerico<T> Repositorio<T>() where T : class {
            var type = typeof(T);
            if (_repositorios.ContainsKey(type)) {
                return (IRepositorioGenerico<T>)_repositorios[type]!;
            }

            var repo = new RepositorioGenericoMovies<T>(_context);
            _repositorios[type] = repo;
            return repo;
        }

        public Task<int> SaveChangesAsync() {
            return _context.SaveChangesAsync();
        }
    }
}