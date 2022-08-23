using System;

namespace Xasu.Exceptions
{
    public class ExtensionXApiException : XApiException{
        public ExtensionXApiException(string message) : base(message){
        }
    }
}