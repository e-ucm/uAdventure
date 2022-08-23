using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xasu.Exceptions
{
    public class BackgroundException : System.Exception
    {
        public BackgroundException(string message) : base(message)
        {
        }

        public BackgroundException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}
