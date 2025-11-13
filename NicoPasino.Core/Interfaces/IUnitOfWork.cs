namespace NicoPasino.Core.Interfaces

{
    public interface IUnitOfWork
    {
        IRepositorioGenerico<T> Repositorio<T>() where T : class;
        Task<int> SaveChangesAsync();
    }
}
