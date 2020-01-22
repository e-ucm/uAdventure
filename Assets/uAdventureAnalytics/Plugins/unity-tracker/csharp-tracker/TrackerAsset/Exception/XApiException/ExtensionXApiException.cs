using System;

namespace AssetPackage.Exceptions
{
    public class ExtensionXApiException : XApiException{
        public ExtensionXApiException(string message) : base(message){
        }
    }
}