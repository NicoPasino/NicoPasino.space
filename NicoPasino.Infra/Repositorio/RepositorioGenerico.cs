using Microsoft.EntityFrameworkCore;
using NicoPasino.Core.Interfaces;
using NicoPasino.Infra.Data;
using System.Linq.Expressions;

namespace NicoPasino.Infra.Repositorio
{
    public class RepositorioGenerico<T> : IRepositorioGenerico<T> where T : class
    {
        private readonly moviesdbContext _context;
        private readonly DbSet<T> _dbSet;

        public RepositorioGenerico(moviesdbContext context) {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T?>> GetAll() {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetById(int id) {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> Add(T entity) {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task AddRange(IEnumerable<T> entities) {
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            return;
        }

        public async Task<int> Update(T entity) {
            _dbSet.Update(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task Delete(T entity) {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return;
        }

        public async Task DeleteRange(IEnumerable<T> entities) {
            _dbSet.RemoveRange(entities);
            await _context.SaveChangesAsync();
            return;
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>>? filtro = null, string incluir = "") {
            IQueryable<T> query = _dbSet;

            if (filtro != null)
                query = query.Where(filtro);

            if (!string.IsNullOrWhiteSpace(incluir)) {
                foreach (var inc in incluir.Split(',', StringSplitOptions.RemoveEmptyEntries)) {
                    query = query.Include(inc.Trim());
                }
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> ListarAsync(Expression<Func<T, bool>>? filtro = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orden = null, string incluir = "") {
            IQueryable<T> query = _dbSet;

            if (filtro != null)
                query = query.Where(filtro);

            if (!string.IsNullOrWhiteSpace(incluir)) {
                foreach (var inc in incluir.Split(',', StringSplitOptions.RemoveEmptyEntries)) {
                    query = query.Include(inc.Trim());
                }
            }

            if (orden != null)
                query = orden(query);

            return await query.ToListAsync();
        }
    }
}