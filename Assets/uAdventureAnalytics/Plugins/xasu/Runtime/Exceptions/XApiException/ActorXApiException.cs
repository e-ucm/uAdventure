using System;

namespace Xasu.Exceptions
{
    public class ActorXApiException : XApiException {
        public ActorXApiException(string message) : base(message){
        }
    }
}