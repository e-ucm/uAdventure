using System;

namespace AssetPackage.Exceptions
{
    public class TrackerException : Exception{
        public TrackerException(string message) : base(message){
        }
    }
}