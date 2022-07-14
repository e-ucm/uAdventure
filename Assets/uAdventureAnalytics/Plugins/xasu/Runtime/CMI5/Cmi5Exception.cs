using System;
using System.Runtime.Serialization;

namespace Xasu.Exceptions
{
    public class Cmi5Exception : Exception
    {
        public Cmi5Exception()
        {
        }

        public Cmi5Exception(string message) : base(message)
        {
        }

        public Cmi5Exception(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected Cmi5Exception(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
