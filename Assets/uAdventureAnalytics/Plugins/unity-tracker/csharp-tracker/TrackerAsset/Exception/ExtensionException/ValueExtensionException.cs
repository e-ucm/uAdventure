using System;

namespace AssetPackage.Exceptions
{
    public class ValueExtensionException : ExtensionException{
        public ValueExtensionException(string message) : base(message){
        }
    }
}