using System;

namespace Xasu.Exceptions
{
    public class ValueExtensionException : ExtensionException{
        public ValueExtensionException(string message) : base(message){
        }
    }
}