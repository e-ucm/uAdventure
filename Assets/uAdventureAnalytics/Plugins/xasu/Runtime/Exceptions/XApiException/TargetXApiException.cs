using System;

namespace Xasu.Exceptions
{
    public class TargetXApiException : XApiException{
        public TargetXApiException(string message) : base(message){
        }
    }
}