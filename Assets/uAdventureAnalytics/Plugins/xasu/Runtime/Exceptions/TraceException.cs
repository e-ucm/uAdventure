using System;

namespace Xasu.Exceptions
{
    public class TraceException : TrackerException{
        public TraceException(string message) : base(message){
        }
    }
}