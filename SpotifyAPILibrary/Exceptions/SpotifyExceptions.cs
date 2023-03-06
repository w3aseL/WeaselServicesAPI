using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyAPILibrary.Exceptions
{
    public class SpotifyAccountNotFoundException : ApplicationException
    {
        public SpotifyAccountNotFoundException(string message) : base(message) { } 
    }

    public class SpotifyAccountExpiredException : ApplicationException
    {
        public SpotifyAccountExpiredException(string message) : base(message) { }
    }
}
