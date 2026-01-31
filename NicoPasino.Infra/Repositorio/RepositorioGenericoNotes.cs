using NicoPasino.Infra.Data;

namespace NicoPasino.Infra.Repositorio
{
    public class RepositorioGenericoNotes<T> : RepositorioGenericoBase<T> where T : class
    {
        public RepositorioGenericoNotes(notasdbContext context) : base(context) { }
    }
}