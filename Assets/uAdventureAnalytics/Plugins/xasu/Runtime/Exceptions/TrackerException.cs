using System;

namespace Xasu.Exceptions
{
    public class TrackerException : Exception {
        public TrackerException(string message) : base(message)
        {
        }

        public TrackerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}