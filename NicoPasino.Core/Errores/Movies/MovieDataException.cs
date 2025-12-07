namespace NicoPasino.Core.Errores.Movies
{
    public class MovieDataException : Exception
    {
        //public MovieDataException() : base() { }
        public MovieDataException(string message) : base(message) { }
    }
}
