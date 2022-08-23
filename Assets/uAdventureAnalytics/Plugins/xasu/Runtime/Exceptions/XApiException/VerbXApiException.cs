using System;

namespace Xasu.Exceptions
{
    public class VerbXApiException : XApiException{
        public VerbXApiException(string message) : base(message){
        }
    }
}