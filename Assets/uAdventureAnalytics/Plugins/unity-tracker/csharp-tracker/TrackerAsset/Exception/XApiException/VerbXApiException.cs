using System;

namespace AssetPackage.Exceptions
{
    public class VerbXApiException : XApiException{
        public VerbXApiException(string message) : base(message){
        }
    }
}