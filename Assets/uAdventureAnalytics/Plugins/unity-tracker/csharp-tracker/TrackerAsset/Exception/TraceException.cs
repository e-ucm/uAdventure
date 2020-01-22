using System;

namespace AssetPackage.Exceptions
{
    public class TraceException : TrackerException{
        public TraceException(string message) : base(message){
        }
    }
}