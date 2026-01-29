namespace NicoPasino.Core.Interfaces

{
    public interface IUnitOfWorkMovie
    {
        IRepositorioGenerico<T> Repositorio<T>() where T : class;
        Task<int> SaveChangesAsync();
    }
}
