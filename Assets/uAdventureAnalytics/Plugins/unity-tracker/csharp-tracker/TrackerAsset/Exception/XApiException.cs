using System;

namespace AssetPackage.Exceptions
{
    public class XApiException : TrackerException{

        public XApiException(string message) : base(message){
        }
    }
}