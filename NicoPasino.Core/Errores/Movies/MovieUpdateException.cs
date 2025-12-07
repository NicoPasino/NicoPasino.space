using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NicoPasino.Core.Errores.Movies
{
    public class MovieUpdateException : Exception
    {
        //public MovieUpdateException() : base() { }
        public MovieUpdateException(string message) : base(message) { }
    }
}
