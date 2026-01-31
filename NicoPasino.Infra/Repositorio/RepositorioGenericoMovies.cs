using NicoPasino.Infra.Data;

namespace NicoPasino.Infra.Repositorio
{
    public class RepositorioGenericoMovies<T> : RepositorioGenericoBase<T> where T : class
    {
        public RepositorioGenericoMovies(moviesdbContext context) : base(context) { }
    }
}